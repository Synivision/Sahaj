using Assets.Code.DataPipeline;
using Assets.Code.Messaging;
using Assets.Code.Ui;
using UnityEngine;
using System.Collections.Generic;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Ui.CanvasControllers;
using Assets.Code.Messaging.Messages;
using System.Collections;
using Assets.Code.UnityBehaviours;
namespace Assets.Code.States
{
	public class ShipBaseState : BaseState
	{
		
		private Canvas _shipBaseCanvas;
			
		private CanvasProvider _canvasProvider;
		private PoolingObjectManager _poolingObjectManager;
		private UiManager _uiManager;
		ShipLevelManager shipLevelManager;
		private readonly Messager _messager;
		readonly GameDataProvider _gameDataProvider;
		private readonly PrefabProvider _prefabProvider;
		private SpriteProvider _spriteProvider;
		private MessagingToken _onBuildingMove;
		private MessagingToken _onBuildingStopMovement;
		private MessagingToken _onChangeStateToAttack;
		private MessagingToken _onInventoryOpen;
		private MessagingToken _onBuildingInfoOpen;
		private MessagingToken _onOpenShopMessage;
		private MessagingToken _onCreateBuildingMessage;
		private MessagingToken _onOpenCreatePirateCanvasMessage;

		private MessagingToken _onOpenBuildingMenuMessage;
        public static bool buildingsMoving = false;

        MapLayout _map;
		private float _time = 5;
		private GameObject tile;
		int pointerId = -1;
		private bool _mouseState;
		private GameObject target;
		public Vector3 screenSpace;
		public Vector3 offset;
		public GameObject newBuilding;
		private GameObject inspectorCanvas;
		private GameObject _rowbBoatParent;
		private PlayerManager _playerManager;
        private UnityReferenceMaster _unityReferenceMaster;
        private CameraController _camera;
        private ShopCanvasController _shopCanvasController;
		private InspectorCanvasController _buildingMenuCanvasController;

		private CreatePirateCanvasController _createPirateCanvasController;
        Vector3 curPosition;
		Vector3 selectedgameObjectPosition = new Vector3(0,0,0);
		
		public ShipBaseState (IoCResolver resolver, MapLayout map) : base(resolver){
			
			
			_resolver.Resolve (out _canvasProvider);
			_resolver.Resolve (out _messager);
			_resolver.Resolve (out _poolingObjectManager);
			_resolver.Resolve(out _gameDataProvider);
			_resolver.Resolve(out _prefabProvider);
			_resolver.Resolve(out _spriteProvider);
			_resolver.Resolve(out _playerManager);
            _resolver.Resolve(out _unityReferenceMaster);
			
			_map = map;
		}

        public override void Initialize()
        {

            _uiManager = new UiManager();
            _uiManager.RegisterUi(new ShipBaseCanvasController(_resolver, _canvasProvider.GetCanvas("ShipBaseCanvas")));


            shipLevelManager = new ShipLevelManager(_resolver, _map);
	            _onChangeStateToAttack = _messager.Subscribe<StartGameMessage>(OnChangeStateToAttack);
            _onInventoryOpen = _messager.Subscribe<OpenInventory>(OpenInventoryBuilding);
            _onBuildingInfoOpen = _messager.Subscribe<OpenBuildingInfoCanvas>(OnOpenBuildingInfoCanvas);
            _onOpenShopMessage = _messager.Subscribe<OpenShopMessage>(OnOpenShop);
            _onCreateBuildingMessage = _messager.Subscribe<CreateBuildingMessage>(onCreateBuilding);
			_onOpenCreatePirateCanvasMessage = _messager.Subscribe<OpenCreatePirateCanvasMessage> (onOpenCreatePirateCanvas);
			_onBuildingMove = _messager.Subscribe<MoveBuildingmessage> (OnMoveBuilding);
			_onBuildingStopMovement = _messager.Subscribe<StopMovingBuildingMessage> (OnStopMovingBuilding);
			_onOpenBuildingMenuMessage = _messager.Subscribe<OpenBuildingMenuMessage> (OnOpenBuildingMenu);

            //generate tile and disable it
            var tileo = _poolingObjectManager.Instantiate("tile");
            tile = tileo.gameObject;
            tile.SetActive(false);

            if (_playerManager.Model != null)
            {
                _rowbBoatParent = Object.Instantiate(_prefabProvider.GetPrefab("empty1"));
                _rowbBoatParent.transform.position = new Vector3(0, 0, 0);
                _rowbBoatParent.gameObject.name = "RowBoatParent";

                //instantiate boats
                int x = 0;
                foreach (var boat in _playerManager.Model.RowBoatCountDict)
                {
                    x++;
                    var rowBoat = _poolingObjectManager.Instantiate("row_boat").gameObject;
                    rowBoat.transform.position = new Vector3(-110, 11.5f, -25 * x);
                    var boatController = rowBoat.GetComponent<RowBoatController>();
                    boatController.Initialize(_resolver, false, boat.Key, null);

                    rowBoat.transform.SetParent(_rowbBoatParent.transform);

                }
                x = 0;
            }
            _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

			saveMapLayoutToFile ();

        }

		void OnMoveBuilding(MoveBuildingmessage message){
            if (message.BuildingName == target.name)
            {
                _mouseState = true;
            }
		}

		void OnStopMovingBuilding(StopMovingBuildingMessage message){
			_mouseState = false;

		}


        public void onCreateBuilding(CreateBuildingMessage message) {
			
			//generate a building from shop
			newBuilding =   shipLevelManager.CreateBuilding(message.BuildingName,new Vector3(0,11,0));
            newBuilding.name = message.BuildingName;
            //find a new position for generated building with tile empty
            var bluePrint = shipLevelManager.GetBluePrint();
            Vector3 newPosition =  new Vector3(0, 11, 0) ;
            for (int i = 0; i<25; i++) {
                if (bluePrint[i,i].Equals("empty")) {
                    //calculate Vector3 for new building
                    newPosition = new Vector3(10*i,11,10*i) - new Vector3(125,0,125);
                    break;
                }
            }
            newBuilding.transform.position = newPosition;

            //bring tile under new building
            tile.SetActive(true);
            tile.GetComponent<Renderer>().material.color = Color.green;
            tile.transform.position = newBuilding.transform.position + new Vector3(0,-10,0);

            shipLevelManager.AddBuildingToBlueprint(newBuilding.name, newBuilding.transform.position + new Vector3(125, 0, 125));
        }
		
		public void OnOpenShop (OpenShopMessage message)
		{
           
            if (_shopCanvasController != null)
            {
                _shopCanvasController.enableCanvas();
            }
            else
            {
                _shopCanvasController = new ShopCanvasController(_resolver, _canvasProvider.GetCanvas("ShopCanvas"));
                _uiManager.RegisterUi(_shopCanvasController);
            }
		}
		
		public void OnOpenBuildingInfoCanvas(OpenBuildingInfoCanvas message) {
			
			_uiManager.RegisterUi(new BuildingInfoCanvasController(_resolver, _canvasProvider.GetCanvas("BuildingInfoCanvas"), message.buildingName));
		}
		
		public void OpenInventoryBuilding(OpenInventory message){

			var controller  = new InventoryCanvasController (_resolver, _canvasProvider.GetCanvas ("InventoryCanvas"));
			_uiManager.RegisterUi (controller);
		}
		
		public void OnChangeStateToAttack(StartGameMessage message){
			
			SwitchState (new PlayState(_resolver,message.MapLayout));
			
		}

		public void OnOpenBuildingMenu(OpenBuildingMenuMessage message){
		
			if (_buildingMenuCanvasController == null) {

				_buildingMenuCanvasController = new InspectorCanvasController (_resolver, _canvasProvider.GetCanvas ("BuildingMenuCanvas"), message.Model, message.Position);
				_uiManager.RegisterUi (_buildingMenuCanvasController);
				_buildingMenuCanvasController.enableCanvas ();
				Debug.Log ("Open Building canvas");

			} else {
				_buildingMenuCanvasController.enableCanvas ();
				_buildingMenuCanvasController = new InspectorCanvasController (_resolver, _canvasProvider.GetCanvas ("BuildingMenuCanvas"), message.Model, message.Position);
			}
		
		}

		public void onOpenCreatePirateCanvas(OpenCreatePirateCanvasMessage message){

			if (_createPirateCanvasController == null) {

                var _newCreatepiratecanvascontroller = new NewCreatePirateCanvasController(_resolver, _canvasProvider.GetCanvas("CreatePirateCanvas"));
                //_createpiratecanvascontroller = new CreatePirateCanvasController (_resolver, _canvasProvider.GetCanvas ("CreatePirateCanvas"));
                //_createPirateCanvasController.BuildingModel = message.BuildingModel;
                //_createPirateCanvasController.Initialize();
                _newCreatepiratecanvascontroller.Initialize();
                _newCreatepiratecanvascontroller.PlatoonBuildingLevel = message.BuildingModel.Level;

                _uiManager.RegisterUi (_newCreatepiratecanvascontroller);
			
			} else {
			
				
				//_createPirateCanvasController.BuildingModel = null;
				_createPirateCanvasController.BuildingModel = message.BuildingModel;
				_createPirateCanvasController.Initialize();
				_createPirateCanvasController.enableCanvas();
			
			}

		}
		
		public override void Update (){
			
			_uiManager.Update ();
			
			Touch[] touch = Input.touches;
			if (Application.platform == RuntimePlatform.Android)
				pointerId = touch[0].fingerId;
			
			//used to move cube around 
			if (Input.GetMouseButtonDown (0)  && !isButton()) {
				RaycastHit hitInfo;

                if (!_mouseState) {
                    target = GetClickedObject(out hitInfo);
                }
				//turn off any inspector canvas visible on clicking any place other than that canvas button
				if(inspectorCanvas != null){
					inspectorCanvas.SetActive(false);
				}
				
				if (target != null && (target.gameObject.tag == "Cube" )) {

                    if (_mouseState) {

                        if (SaveLocationOfBuildingInMap(selectedgameObjectPosition))
                        {
                            _mouseState = false;
                            tile.SetActive(false);

                            saveMapLayoutToFile();
                        }
                        else {

                            target.transform.position = selectedgameObjectPosition;
                            _mouseState = false;
                            tile.SetActive(false);
                        }
                       
                    }
                    else {
                        selectedgameObjectPosition = target.transform.position;

                        _messager.Publish(new OpenBuildingMenuMessage {
                            BuildingName = target.name,
                            Model = target.GetComponent<BuildingController>().Model,
                            Position = selectedgameObjectPosition
                        });
                    }
                    Debug.Log("_mouseState" + _mouseState.ToString());
				}


                if (target != null && (target.gameObject.tag == "RowBoat")) {

                    Debug.Log("Show RowBoat Status Canvas");
                    target = null;
					
				}
				if (target != null && (target.gameObject.tag == "water")) {
					target = null;
					Debug.Log("water");
				}
				
				if(target != null && target.gameObject.tag != "Cube"){

					if(_buildingMenuCanvasController!=null){
						_buildingMenuCanvasController.disableCanvas ();

					}
					tile.SetActive(false);
				}
			}
			
			if (Input.GetMouseButtonUp (0) ) {
                
                _camera.canMove = true;
                saveMapLayoutToFile();
            }
			
			if (_mouseState && (target.gameObject.tag == "Cube")) {

                screenSpace = Camera.main.WorldToScreenPoint (target.transform.position);
				var curScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
				curPosition = Camera.main.ScreenToWorldPoint (curScreenSpace) + offset;
				curPosition.y = target.transform.position.y;

				//move the building to where the mouse is
				target.transform.position = curPosition;

                _camera.canMove = false;
				//Get grid on curposition
				//instantiate red or green according to grid
				tile.SetActive(true);
				tile.transform.position = curPosition+new Vector3(0,-10,0);
				
				
				if(shipLevelManager.GetCoordinatePassability(curPosition + new Vector3(125,0,125)) == ShipLevelManager.PassabilityType.Passible){
					tile.GetComponent<Renderer>().material.color = Color.green;
				}else{
					
					//if the position is clicked and not moved.. then also for that object the tile is available
					if(shipLevelManager.GetTileAt(curPosition + new Vector3(125,0,125)) == target.gameObject.name){
						tile.GetComponent<Renderer>().material.color = Color.green;
					}
					else{
						tile.GetComponent<Renderer>().material.color = Color.red;
					}
				}
			}
		}

        bool SaveLocationOfBuildingInMap(Vector3 buildingOrignalPosition) {

            bool result = false;
            screenSpace = Camera.main.WorldToScreenPoint(target.transform.position);
            var curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
            curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;
            curPosition.y = target.transform.position.y;

            
            if (shipLevelManager.GetCoordinatePassability(curPosition + new Vector3(125, 0, 125)) == ShipLevelManager.PassabilityType.Passible)
            {
                shipLevelManager.UpdateBlueprint(buildingOrignalPosition + new Vector3(125, 0, 125), curPosition + new Vector3(125, 0, 125));
                result = true;
            }

                return result;
        }

        public void saveMapLayoutToFile() {
            MapLayout layout = shipLevelManager.bluePrintToMapLayout();
            Serializer.Save<MapLayout>("MapLayout1", layout);
        }

		public GameObject GetClickedObject (out RaycastHit hit)
		{
			GameObject target = null;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray.origin, ray.direction * 10, out hit)) {
				target = hit.collider.gameObject;
			}
			return target;
		}
		
		public override void HandleInput (){
			
		}
		
		public override void TearDown (){

            _messager.CancelSubscription(_onBuildingInfoOpen,_onInventoryOpen,
				_onChangeStateToAttack, _onOpenShopMessage, _onCreateBuildingMessage,_onOpenBuildingMenuMessage,_onBuildingMove,_onBuildingStopMovement);
			_uiManager.TearDown();
			Object.Destroy (tile.gameObject);
			shipLevelManager.TearDown();
			if (_rowbBoatParent !=null) {
				
				Object.Destroy(_rowbBoatParent.gameObject);
			}
			
			
		}
		
		private bool isButton()
		{
			bool result = true;
			UnityEngine.EventSystems.EventSystem ct
				= UnityEngine.EventSystems.EventSystem.current;
			
			if (! ct.IsPointerOverGameObject() ) result = false;
			if (! ct.currentSelectedGameObject ) result = false;
			
			
			return result;
		}
	}
	
}

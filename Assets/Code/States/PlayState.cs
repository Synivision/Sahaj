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
namespace Assets.Code.States
{
	public class PlayState : BaseState
	{
		public PrefabProvider _prefabProvider;
		/* REFERENCES */
		private readonly Messager _messager;

		/* PROPERTIES */
		private UiManager _uiManager;

		/* UiControllers */
		private MainCanvasController _mainCanvasController;
		private CanvasProvider _canvasProvider;

		//tokens
		private MessagingToken _onQuitGame;
		private MessagingToken _onTearDownLevel;
		private PoolingObjectManager _poolingObjectManager;
		private MessagingToken _onPlayStateToShipBase;
		//level Manager
		private Dictionary<string,int> _pirateCountDict;
		private Dictionary<string,bool> _unlockedPirates;

		LevelManager levelManager;
		PlayerManager _playerManager;
		MapLayout _mapLayout;
		//Input Controller requirements

		private bool _mouseState;
		private GameObject target;
		public Vector3 screenSpace;
		public Vector3 offset;
		private float _time = 5;
		private InputSession _inputSession;
		private InputSessionData _inputSessionData;

		private GameObject tile;
		int pointerId = -1;

		Vector3 curPosition;
		Vector3 selectedgameObjectPosition = new Vector3(0,0,0);
		public PlayState (IoCResolver resolver, MapLayout mapLayout) : base(resolver)
		{
			_mapLayout = mapLayout;
			_resolver.Resolve (out _messager);
			_resolver.Resolve (out _prefabProvider);
			_resolver.Resolve (out _canvasProvider);
			_resolver.Resolve (out _poolingObjectManager);
			_resolver.Resolve (out _playerManager);
			_resolver.Resolve (out _inputSession);

		}

		public override void Initialize ()
		{
			_uiManager = new UiManager ();

			//_uiManager.RegisterUi (new MainCanvasController (_resolver, _canvasProvider.GetCanvas ("MainCanvas")));
			_uiManager.RegisterUi (new PirateInfoCanvasController (_resolver, _canvasProvider.GetCanvas ("PirateInfoCanvas")));

			_unlockedPirates = new Dictionary<string, bool>();
			_unlockedPirates.Add("Captain",true);
			_unlockedPirates.Add("Quarter Master",true);
			_unlockedPirates.Add("Gunner",true);
			_unlockedPirates.Add("Bomber",true);
			_unlockedPirates.Add("Surgeon",true);
			_unlockedPirates.Add("Carpenter",false);
			_unlockedPirates.Add ("Chef",false);
			_unlockedPirates.Add ("Pirate4",false);
			_unlockedPirates.Add ("EnemyPirate5",false);

			_pirateCountDict = new Dictionary<string, int>();
			_pirateCountDict.Add("Captain",1);
			_pirateCountDict.Add("Quarter Master",2);
			_pirateCountDict.Add("Gunner",5);
			_pirateCountDict.Add("Bomber",3);
			_pirateCountDict.Add("Surgeon",4);
			_pirateCountDict.Add("Carpenter",2);
			_pirateCountDict.Add ("Chef",2);
			_pirateCountDict.Add ("Pirate4",1);
			_pirateCountDict.Add ("EnemyPirate3",10);



			//Initialize level manager
			levelManager = new LevelManager (_resolver, _mapLayout);

			PlayerModel playerModel = new PlayerModel();
			playerModel.Name = "User";
			playerModel.Email = "user@gmail.com";
			playerModel.Gold = 0;
			playerModel.ExperiencePoints = 0;
			playerModel.UserLevel =  0;
			playerModel.UserRank = 0;
			playerModel.Wins = 0;
			playerModel.Gems = 100;
			playerModel.MaxGoldCapacity = 2000;
			playerModel.UnlockedPirates = _unlockedPirates;
			playerModel.PirateCountDict = _pirateCountDict;


			_playerManager.Initialize(_resolver,playerModel,levelManager);
			levelManager.PirateCountDict = _pirateCountDict;
			_uiManager.RegisterUi (new GamePlayCanvasController (_resolver, _canvasProvider.GetCanvas ("GamePlayCanvas"), _playerManager));
			//Debug.Log ("Play state initialized.");

			//Message tokens
			_onQuitGame = _messager.Subscribe<QuitGameMessage> (OnQuitGame);
			_onTearDownLevel = _messager.Subscribe<TearDownLevelMessage> (OnTearDownLevel);
			_onPlayStateToShipBase = _messager.Subscribe<PlayStateToShipBaseMessage>(OnPlayStateToShipBase);

			_inputSessionData = new InputSessionData();
			_inputSessionData.Name = "None";
			_inputSession.Initialize(_inputSessionData);


			var tileo = _poolingObjectManager.Instantiate("tile");
			tile = tileo.gameObject;
			tile.SetActive(false);
		}

		public void OnPlayStateToShipBase(PlayStateToShipBaseMessage message){

			SwitchState(new ShipBaseState(_resolver));
		}

		public override void Update ()
		{
			_uiManager.Update ();
            
			// input controller update
			_time += Time.deltaTime;
			if (_time > 3) {
				_messager.Publish (new PirateInfoCanvasMessage{
					toggleValue = false
				});
			}
			
			
			Touch[] touch = Input.touches;
			if (Application.platform == RuntimePlatform.Android)
				pointerId = touch[0].fingerId;
			
			//used to move cube around 
			if (Input.GetMouseButtonDown (0)) {
				RaycastHit hitInfo;
				target = GetClickedObject (out hitInfo);


				if (target != null && (target.gameObject.tag == "Cube" )) {
					_mouseState = true;
					//get position of object selected 
					selectedgameObjectPosition = target.transform.position;


				}

				Ray ray;
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if( UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(pointerId)== false){
					if(Physics.Raycast(ray,out hitInfo)&& target.gameObject.tag!="Cube" )
					{
						Vector3 spawnPosition = new Vector3(hitInfo.point.x,5.2f,hitInfo.point.z);
	
						//Debug.Log ("Spawn Point from Input Controller = " + spawnPosition.ToString()
						levelManager.CreatePirate(_inputSession.CurrentlySelectedPirateName,spawnPosition);
					}
					
				}
			}
			
			if (Input.GetMouseButtonUp (0)) {

				//update the grid tile of moved object if grid tile is empty

				if(_mouseState && levelManager.GetCoordinatePassability(curPosition + new Vector3(125,0,125)) == LevelManager.PassabilityType.Passible){
					levelManager.UpdateBlueprint(selectedgameObjectPosition + new Vector3(125,0,125),curPosition + new Vector3(125,0,125));
				}
				else{
					if(target!=null && target.gameObject.tag != "Plane"){
						target.transform.position = selectedgameObjectPosition;
					}

					
				}
				_mouseState = false;
				tile.SetActive(false);
			}
			
			if (_mouseState) {
				screenSpace = Camera.main.WorldToScreenPoint (target.transform.position);
				var curScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
				curPosition = Camera.main.ScreenToWorldPoint (curScreenSpace) + offset;
				curPosition.y = target.transform.position.y;
				target.transform.position = curPosition;
				//Get grid on curposition
				//Debug.Log(levelManager.GetCoordinatePassability(curPosition + new Vector3(125,0,125)));
				//instantiate red or green according to grid
				tile.SetActive(true);
				tile.transform.position = curPosition+new Vector3(0,-10,0);
				if(levelManager.GetCoordinatePassability(curPosition + new Vector3(125,0,125)) == LevelManager.PassabilityType.Passible){
					tile.GetComponent<Renderer>().material.color = Color.green;
				}else{
					tile.GetComponent<Renderer>().material.color = Color.red;
				}
			}


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

		private void OnQuitGame (QuitGameMessage message)
		{
			SwitchState (new MenuState (_resolver));

		}

		public override void HandleInput ()
		{

		}

		private void OnTearDownLevel (TearDownLevelMessage message)
		{

			levelManager.TearDownLevel ();
		}

		public override void TearDown ()
		{
			_messager.CancelSubscription (_onQuitGame, _onTearDownLevel, _onPlayStateToShipBase);
			levelManager.TearDownLevel ();
			_uiManager.TearDown ();

			_poolingObjectManager.TearDown ();

		}
	}
}

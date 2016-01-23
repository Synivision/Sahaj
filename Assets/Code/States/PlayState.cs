using Assets.Code.DataPipeline;
using Assets.Code.Messaging;
using Assets.Code.Ui;
using UnityEngine;
using System.Linq;
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
        InventoryCanvasController _inventoryCanvasController;
        /* UiControllers */
        private MainCanvasController _mainCanvasController;
        private CanvasProvider _canvasProvider;
        
        //tokens
        private MessagingToken _onQuitGame;
        private MessagingToken _onTearDownLevel;
        private MessagingToken _onPlayStateToShipBase;
        private MessagingToken _openShipBaseMessage;
        private MessagingToken _onOpenRowBoatSelectedMessage;
        private MessagingToken _onAddPirateToRowBoatMessage;
        private MessagingToken _onWin;
        private PoolingObjectManager _poolingObjectManager;
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
        
        //ship lerp
        public float speed = 1.0F;
        private float startTime;
        private float journeyLength;
        GameObject shipPrefab;
        List<GameObject> rowBoatList;
        private GameObject tile;
        int pointerId = -1;
        bool rowBoatAttackStarted;

        private int rowBoatCount;
        private GameObject _arrowGameObject;
        public Dictionary<string, Dictionary<int, string>> _tempRowBoatCountDict;
        //TODO: this should be taken from the base controller in future..

        Vector3 curPosition;
        Vector3 selectedgameObjectPosition = new Vector3(0, 0, 0);
        public PlayState(IoCResolver resolver, MapLayout mapLayout) : base(resolver)
        {
            _mapLayout = mapLayout;
            _resolver.Resolve(out _messager);
            _resolver.Resolve(out _prefabProvider);
            _resolver.Resolve(out _canvasProvider);
            _resolver.Resolve(out _poolingObjectManager);
            _resolver.Resolve(out _playerManager);
            _resolver.Resolve(out _inputSession);
            
        }
        
        public override void Initialize()
        {
            _uiManager = new UiManager();
            
            //_uiManager.RegisterUi (new MainCanvasController (_resolver, _canvasProvider.GetCanvas ("MainCanvas")));
            _uiManager.RegisterUi(new PirateInfoCanvasController(_resolver, _canvasProvider.GetCanvas("PirateInfoCanvas")));
            
            rowBoatCount = _playerManager.Model.RowBoatCountDict.Count;
            _tempRowBoatCountDict = _playerManager.Model.RowBoatCountDict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            //Initialize level manager
            levelManager = new LevelManager(_resolver, _mapLayout);
            levelManager.PirateCountDict = _playerManager.Model.PirateCountDict;
            _uiManager.RegisterUi(new GamePlayCanvasController(_resolver, _canvasProvider.GetCanvas("GamePlayCanvas"), _playerManager));
            //Debug.Log ("Play state initialized.");
            
            //Message tokens
            _onQuitGame = _messager.Subscribe<QuitGameMessage>(OnQuitGame);
            _onTearDownLevel = _messager.Subscribe<TearDownLevelMessage>(OnTearDownLevel);
            _onPlayStateToShipBase = _messager.Subscribe<OpenShipBaseMessage>(OnOpenShipBaseMessage);
            _onOpenRowBoatSelectedMessage = _messager.Subscribe<RowBoatSelectedMessage>(onOpenRowBoatSelected);
            _onAddPirateToRowBoatMessage = _messager.Subscribe<AddPirateToRowBoatMessage>(onAddPirateToRowBoat);
            _onWin = _messager.Subscribe<WinMessage>(OnWin);
            //Ship lerp into level
            startTime = Time.time;
            shipPrefab = _poolingObjectManager.Instantiate("revolutionaryship").gameObject;
            journeyLength = Vector3.Distance(shipPrefab.transform.position, new Vector3(-120, 15, -120));
            shipPrefab.GetComponent<ShipBehaviour>().Initialize(_resolver, levelManager, shipPrefab.transform.position,_playerManager);
            //shipPrefab.gameObject.transform.position.lerp
            rowBoatList = new List<GameObject>();
            rowBoatAttackStarted = false;

            _arrowGameObject = _poolingObjectManager.Instantiate("rowboat_tile").gameObject;
        }

        public void onAddPirateToRowBoat(AddPirateToRowBoatMessage message)
        {
         
            _uiManager.RegisterUi(new InventoryCanvasController(_resolver, _canvasProvider.GetCanvas("InventoryCanvas"), message.BoatName));
        }

        public void onOpenRowBoatSelected(RowBoatSelectedMessage message)
        {
            _uiManager.RegisterUi(new RowBoatCanvasController(_resolver, _canvasProvider.GetCanvas("RowBoatCanvas"), message.BoatName));
            //message.onCancelled();
        }


        private void OnWin(WinMessage message)
        {
            _uiManager.RegisterUi(new WinLooseCanvasController(_resolver, _canvasProvider.GetCanvas("WinCanvas")));

        }

        public override void Update()
        {

            if (_inputSession.CurrentlySelectedRowBoatName != null)
            {
                _arrowGameObject.SetActive(true);
                var pos1 = new Vector3(-200,20,0);
                var pos2 = new Vector3(-150, 20, 0);
                _arrowGameObject.transform.position = Vector3.Lerp(pos1, pos2, Mathf.PingPong(Time.time * speed, 1.0f));

            }
            else {
                _arrowGameObject.SetActive(false);

            }

            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            shipPrefab.transform.position = Vector3.Lerp(shipPrefab.transform.position, new Vector3(-120, 15, -120), fracJourney);
            
            _uiManager.Update();
            
            // input controller update
            _time += Time.deltaTime;
            if (_time > 3)
            {
                _messager.Publish(new PirateInfoCanvasMessage
                                  {
                    toggleValue = false
                });
            }
            
            Touch[] touch = Input.touches;
            if (Application.platform == RuntimePlatform.Android)
                pointerId = touch[0].fingerId;


            //used to move cube around 
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo;
                target = GetClickedObject(out hitInfo);

                Ray ray;
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(pointerId) == false)
                {
                    if (Physics.Raycast(ray, out hitInfo) && target.gameObject.tag != "Cube")
                    {
                        Vector3 spawnPosition = new Vector3(hitInfo.point.x, 5.2f, hitInfo.point.z);

                        //Debug.Log ("Spawn Point from Input Controller = " + spawnPosition.ToString()
                        //levelManager.CreatePirate(_inputSession.CurrentlySelectedPirateName, spawnPosition);
                    }
                    if (!rowBoatAttackStarted && _inputSession.CurrentlySelectedShipAttackName == null) {
                        if (target != null && (target.gameObject.tag == "Cube"))
                        {
                            var buildingName = target.GetComponent<BuildingController>().name;
                            _uiManager.RegisterUi(new BuildingInfoCanvasController(_resolver, _canvasProvider.GetCanvas("BuildingInfoCanvas"), buildingName));
                            target = null;
                        }
                    }

                    if ( target != null   &&   target.gameObject.tag != null )
                    {
                        Vector3 fireBulletAtPos = new Vector3(hitInfo.point.x, 5.2f, hitInfo.point.z);
                        
                        if (_playerManager.Model.ShipBulletsAvailable > 0 && _inputSession.CurrentShipAttackCost != 0 && _playerManager.Model.ShipBulletsAvailable >= _inputSession.CurrentShipAttackCost)
                            
                        {
                            if (rowBoatAttackStarted || _inputSession.CurrentlySelectedShipAttackName != null) {
                                shipPrefab.GetComponent<ShipBehaviour>().shoot(fireBulletAtPos);
                                //damage building behaviour
                                if (target.gameObject.tag == "Cube")
                                {
                                    target.GetComponent<BuildingController>().Stats.CurrentHealth -= 50;
                                }
                            }
                        }
                    }
                    if (hitInfo.collider != null && rowBoatCount > 0 && hitInfo.collider.gameObject.tag == "water")
                    {
                        //Now Generating the row boat only on click.
                        //Max row boats created = rowBoatList count that should depend on the boats that the user have
                        
                        Vector3 boatSpawnPoint = new Vector3();
                        
                        //Get tile position to decide boat spawn point
                        Vector2 tilePosition = GetTileAt(hitInfo.point + new Vector3(125,0,125));
                        int x = (int) tilePosition.x;
                        int y = (int) tilePosition.y;
                        
                        if(x != 26 && y!= 26){
                            
                            if(x == 0 || x == 1){
                                boatSpawnPoint = new Vector3(-240,11.5f,25);
                                SpawnBoatAt(boatSpawnPoint,hitInfo.point );
                            }
                            
                            else if(x != 0 && x != 1 && x != 23 && x != 24 && (y == 0 || y == 1 )){
                                boatSpawnPoint = new Vector3(-60,11.5f,-220);
                                SpawnBoatAt(boatSpawnPoint,hitInfo.point );
                            }
                            
                            else if( x == 23 || x == 24 ){
                                boatSpawnPoint = new Vector3(200,11.5f,0);
                                SpawnBoatAt(boatSpawnPoint,hitInfo.point );
                            }
                            
                            else if(x != 0 && x != 1 && x != 23 && x != 24 && (y == 23 || y == 24)){
                                boatSpawnPoint = new Vector3(-30,11.5f,250);
                                SpawnBoatAt(boatSpawnPoint,hitInfo.point );
                            }
                            
                            
                        }
                        
                    }
                    
                    
                }
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                
            }
        }
        
        public void SpawnBoatAt(Vector3 spawn,Vector3 point){

            rowBoatAttackStarted = true;
            //Initialize row boat and controller
            if (_inputSession.CurrentlySelectedRowBoatName!=null && !_inputSession.CurrentlySelectedRowBoatName.Equals("")
                && _tempRowBoatCountDict.ContainsKey(_inputSession.CurrentlySelectedRowBoatName))
            {
                string boatName = _inputSession.CurrentlySelectedRowBoatName;

                GameObject boat = _poolingObjectManager.Instantiate("row_boat").gameObject;
                boat.transform.position = spawn;
                RowBoatController boatController = boat.GetComponent<RowBoatController>();
                //get name of rowboat from player manager
                //var boatName = _playerManager.Model.RowBoatCountDict.Keys.ElementAt(rowBoatCount - 1);

                boatController.Initialize(_resolver, true, boatName, levelManager);

                rowBoatCount--;

                boatController.destinationPosition = point + new Vector3(0, 10, 0);
                boatController.journeyLength = Vector3.Distance(boat.transform.position, boatController.destinationPosition);
                boatController.startTime = Time.time;
                rowBoatList.Add(boat);

                _tempRowBoatCountDict.Remove(boatName);


                Dictionary<int, string> tempDictionary = new Dictionary<int, string>();
                tempDictionary.Add(0, "");
                tempDictionary.Add(1, "");
                tempDictionary.Add(2, "");
                tempDictionary.Add(3, "");
                tempDictionary.Add(4, "");
                tempDictionary.Add(5, "");

                _playerManager.Model.RowBoatCountDict[_inputSession.CurrentlySelectedRowBoatName] = tempDictionary;

                _inputSession.CurrentlySelectedRowBoatName = null;
                //send message to gameplaycanvas about rowboat sent to attack to disable rowboat button
                _messager.Publish(new RowBoatSentToAttackMessage {
                    BoatName = boatName

                });

            }

        }

        public GameObject GetClickedObject(out RaycastHit hit)
        {
            GameObject target = null;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
            {
                target = hit.collider.gameObject;
            }
            return target;
        }
        
        private void OnQuitGame(QuitGameMessage message)
        {
            TearDown();
            SwitchState(new MenuState(_resolver));
            
        }
        
        public override void HandleInput()
        {
            
        }
        
        private void OnTearDownLevel(TearDownLevelMessage message)
        {
            
            levelManager.TearDownLevel();
        }
        
        private void OnOpenShipBaseMessage(OpenShipBaseMessage message)
        {

            MapLayout loadedMap = Serializer.Load<MapLayout>("MapLayout1");
            if (loadedMap != null)
            {

                
                SwitchState(new ShipBaseState(_resolver, loadedMap));

            }
            else {

                Debug.Log("Problem Loading Map");
            }
            
        }
        
        public override void TearDown()
        {
           
            for (int i=0; i < rowBoatList.Count; i++) {
                
                Object.Destroy(rowBoatList[i]);
                
            }
            
            Object.Destroy(shipPrefab);
            GameObject.Destroy( _arrowGameObject);
            levelManager.TearDownLevel();
            
            
            _uiManager.TearDown();
            
            //	_poolingObjectManager.TearDown ();
            _messager.CancelSubscription(_onQuitGame, _onTearDownLevel, _onPlayStateToShipBase,
                _onOpenRowBoatSelectedMessage, _onAddPirateToRowBoatMessage, _onWin);
            
        }
        
        // Return tile position of current point
        public Vector2 GetTileAt(Vector3 point){
            
            Vector2 result = new Vector2(26,26);
            
            //TODO : 10 = Gridsize and should be taken from LevelManager
            int x = (int)(point.x/10);
            int z = (int)(point.z/10);
            
            //Debug.Log(z);
            if( (x >= 0 && x < 25) && (z >= 0 && z < 25) ){
                
                result = new Vector2(x,z);
                
            }
            //Debug.Log("Tilename = " +tileName);
            return result;
        }
    }
}
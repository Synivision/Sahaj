﻿using Assets.Code.DataPipeline;
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
        
        /* UiControllers */
        private MainCanvasController _mainCanvasController;
        private CanvasProvider _canvasProvider;
        
        //tokens
        private MessagingToken _onQuitGame;
        private MessagingToken _onTearDownLevel;
        private MessagingToken _onPlayStateToShipBase;
        private MessagingToken _openShipBaseMessage;
        
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
        
        private int rowBoatCount;
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
            
            // ship  lerp into level
            startTime = Time.time;
            shipPrefab = _poolingObjectManager.Instantiate("revolutionaryship").gameObject;
            journeyLength = Vector3.Distance(shipPrefab.transform.position, new Vector3(-120, 15, -120));
            shipPrefab.GetComponent<ShipBehaviour>().Initialize(_resolver, levelManager, shipPrefab.transform.position,_playerManager);
            //shipPrefab.gameObject.transform.position.lerp
            rowBoatList = new List<GameObject>();
        }
        
        
        
        public override void Update()
        {
            
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
                    
                    if ( target != null   &&   target.gameObject.tag != null )
                    {
                        Vector3 fireBulletAtPos = new Vector3(hitInfo.point.x, 5.2f, hitInfo.point.z);
                        
                        if (_playerManager.Model.ShipBulletsAvailable > 0 && _inputSession.CurrentShipAttackCost != 0 && _playerManager.Model.ShipBulletsAvailable >= _inputSession.CurrentShipAttackCost)
                            
                        {
                            shipPrefab.GetComponent<ShipBehaviour>().shoot(fireBulletAtPos);
                            //damage building behaviour
                            if (target.gameObject.tag == "Cube")
                            {
                                target.GetComponent<BuildingController>().Stats.CurrentHealth -= 50;
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
            
            MapLayout map = new MapLayout();
            
            for (var x = 0; x < 25; x++)
            {
                for (var z = 0; z < 25; z++)
                {
                    MapItemSpawn mapItem = new MapItemSpawn();
                    mapItem.xGridCoord = x;
                    mapItem.Name = "empty";
                    mapItem.zGridCoord = z;
                    map.mapItemSpawnList.Add(mapItem);
                }
            }
            
            BuildingSpawn goldbuilding = new BuildingSpawn();
            goldbuilding.Name = "gold_storage";
            goldbuilding.xGridCoord = 10;
            goldbuilding.zGridCoord = 10;
            
            BuildingSpawn goldbuilding2 = new BuildingSpawn();
            goldbuilding2.Name = "gold_storage";
            goldbuilding2.xGridCoord = 5;
            goldbuilding2.zGridCoord = 5;
            
            BuildingSpawn gunner_tower = new BuildingSpawn();
            gunner_tower.Name = "gunner_tower";
            gunner_tower.xGridCoord = 10;
            gunner_tower.zGridCoord = 5;
            
            BuildingSpawn gunner_tower2 = new BuildingSpawn();
            gunner_tower2.Name = "gunner_tower";
            gunner_tower2.xGridCoord = 5;
            gunner_tower2.zGridCoord = 10;
            
            BuildingSpawn platoons = new BuildingSpawn();
            platoons.Name = "platoons";
            platoons.xGridCoord = 15;
            platoons.zGridCoord = 5;
            
            BuildingSpawn platoons2 = new BuildingSpawn();
            platoons2.Name = "platoons";
            platoons2.xGridCoord = 20;
            platoons2.zGridCoord = 15;
            
            BuildingSpawn water_cannon = new BuildingSpawn();
            water_cannon.Name = "water_cannon";
            water_cannon.xGridCoord = 20;
            water_cannon.zGridCoord = 20;
            
            
            map.buildingSpawnList.Add(goldbuilding);
            map.buildingSpawnList.Add(goldbuilding2);
            map.buildingSpawnList.Add(gunner_tower);
            map.buildingSpawnList.Add(gunner_tower2);
            map.buildingSpawnList.Add(platoons);
            map.buildingSpawnList.Add(platoons2);
            map.buildingSpawnList.Add(water_cannon);
            
            //add river
            
            for (int z = 0; z < 25; z++)
            {
                MapItemSpawn mapItem = new MapItemSpawn();
                mapItem.xGridCoord = 0;
                mapItem.Name = "river";
                mapItem.zGridCoord = z;
                map.mapItemSpawnList.Add(mapItem);
            }
            for (int z = 0; z < 25; z++)
            {
                MapItemSpawn mapItem = new MapItemSpawn();
                mapItem.xGridCoord = 1;
                mapItem.Name = "river";
                mapItem.zGridCoord = z;
                map.mapItemSpawnList.Add(mapItem);
            }
            for (int z = 0; z < 25; z++)
            {
                MapItemSpawn mapItem = new MapItemSpawn();
                mapItem.xGridCoord = z;
                mapItem.Name = "river";
                mapItem.zGridCoord = 0;
                map.mapItemSpawnList.Add(mapItem);
            }   
            SwitchState(new ShipBaseState(_resolver, map));
            
        }
        
        public override void TearDown()
        {
            
            
            for (int i=0; i < rowBoatList.Count; i++) {
                
                Object.Destroy(rowBoatList[i]);
                
            }
            
            Object.Destroy(shipPrefab);
            levelManager.TearDownLevel();
            
            
            _uiManager.TearDown();
            
            //	_poolingObjectManager.TearDown ();
            _messager.CancelSubscription(_onQuitGame, _onTearDownLevel, _onPlayStateToShipBase);
            
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
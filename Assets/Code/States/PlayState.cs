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
		private MessagingToken _onPlayStateToShipBase;

		private PoolingObjectManager _poolingObjectManager;

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

        //ship lerp
        public float speed = 1.0F;
        private float startTime;
        private float journeyLength;
        GameObject shipPrefab;

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
			_onPlayStateToShipBase = _messager.Subscribe<OpenShipBaseMessage>(OnOpenShipBaseMessage);

			_inputSessionData = new InputSessionData();
			_inputSessionData.Name = "None";
			_inputSession.Initialize(_inputSessionData);


            // ship  lerp into level
            startTime = Time.time;
            shipPrefab = _poolingObjectManager.Instantiate("revolutionaryship").gameObject;
            journeyLength = Vector3.Distance(shipPrefab.transform.position, new Vector3(-120,15,-120));
            shipPrefab.GetComponent<ShipBehaviour>().Initialize(_resolver,levelManager, shipPrefab.transform.position);
            //shipPrefab.gameObject.transform.position.lerp

			GameObject rowBoat = _poolingObjectManager.Instantiate ("row_boat").gameObject;
			rowBoat.GetComponent<RowBoatController> ().Initialize (_resolver);
		}


	
		public override void Update ()
		{

            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            shipPrefab.transform.position = Vector3.Lerp(shipPrefab.transform.position, new Vector3(-120, 15, -120), fracJourney);


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


				Ray ray;
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if( UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(pointerId)== false){
					if(Physics.Raycast(ray,out hitInfo)&& target.gameObject.tag!="Cube" )
					{
						Vector3 spawnPosition = new Vector3(hitInfo.point.x,5.2f,hitInfo.point.z);
	
						//Debug.Log ("Spawn Point from Input Controller = " + spawnPosition.ToString()
						levelManager.CreatePirate(_inputSession.CurrentlySelectedPirateName,spawnPosition);
					}

                    if (target.gameObject.tag == "Cube")
                    {
                        Vector3 fireBulletAtPos = new Vector3(hitInfo.point.x, 5.2f, hitInfo.point.z);
                        shipPrefab.GetComponent<ShipBehaviour>().shoot(fireBulletAtPos);
                        //damage building behaviour
                        target.GetComponent<BuildingController>().Stats.CurrentHealth -= 50;
                    }

                }
			}
			
			if (Input.GetMouseButtonUp (0)) {

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
			TearDown();
			SwitchState (new MenuState (_resolver));

		}

		public override void HandleInput ()
		{

		}

		private void OnTearDownLevel (TearDownLevelMessage message)
		{

			levelManager.TearDownLevel ();
		}

		private void OnOpenShipBaseMessage(OpenShipBaseMessage message){
			
			MapLayout map = new MapLayout();
			
			for (var x=0; x<25; x++){
				for (var z=0; z<25; z++){
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
			
			for(int z=0; z<25; z++){
				MapItemSpawn mapItem = new MapItemSpawn();
				mapItem.xGridCoord = 0;
				mapItem.Name = "river";
				mapItem.zGridCoord = z;
				map.mapItemSpawnList.Add(mapItem);
			}
			for(int z=0; z<25; z++){
				MapItemSpawn mapItem = new MapItemSpawn();
				mapItem.xGridCoord = 1;
				mapItem.Name = "river";
				mapItem.zGridCoord = z;
				map.mapItemSpawnList.Add(mapItem);
			}
			for(int z=0; z<25; z++){
				MapItemSpawn mapItem = new MapItemSpawn();
				mapItem.xGridCoord = z;
				mapItem.Name = "river";
				mapItem.zGridCoord = 0;
				map.mapItemSpawnList.Add(mapItem);
			}
			SwitchState(new ShipBaseState(_resolver,map));

		}

		public override void TearDown ()
		{
			levelManager.TearDownLevel ();


			_uiManager.TearDown ();

		//	_poolingObjectManager.TearDown ();
			_messager.CancelSubscription (_onQuitGame, _onTearDownLevel, _onPlayStateToShipBase);

		}
	}
}

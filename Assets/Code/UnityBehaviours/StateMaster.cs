using System;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Loading;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Extensions;
using Assets.Code.Logic.Pooling;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.Models;
using Assets.Code.States;
using Assets.Code.Utilities;
//using UnityEditor;
using UnityEngine;
using Assets.Code.Logic.Logging;


namespace Assets.Code.UnityBehaviours
{
    [RequireComponent(typeof(UnityReferenceMaster))]
    public class StateMaster : MonoBehaviour
    {
        /* REFERENCES */
        private UnityReferenceMaster _unityReference;
		private GameDataProvider _gameDataProvider;
        /* PROPERTIES */
        private Messager _messager;
        private Logger _logger;
        private IoCResolver _resolver;

        private BaseState _currentState;

        /* TOKENS */
        private MessagingToken _onExitGame;

        public void Start()
        {
            _resolver = new IoCResolver();
			//AssetDatabase.Refresh();

            /* RESOURCE LIST CREATION */
#if UNITY_EDITOR
			//AssetDatabase.Refresh();
            FileServices.CreateResourcesList("Assets/Resources/resourceslist.txt");

#endif
            FileServices.LoadResourcesList("resourceslist");

            #region LOAD RESOURCES
            // messager
            _messager = new Messager();
            _resolver.RegisterItem(_messager);

            _logger = new Logger("info.log", false);
            _resolver.RegisterItem(_logger);

            // unity reference master
            _unityReference = GetComponent<UnityReferenceMaster>();
            _unityReference.DebugModeActive = false;
            _resolver.RegisterItem(_unityReference);

            // material provider
            var materialProvider = new MaterialProvider();
			MaterialLoader.LoadMaterial(materialProvider,"Materials");
            _resolver.RegisterItem(materialProvider);

            // texture provider
            var textureProvider = new TextureProvider();
            var spriteProvider = new SpriteProvider();

            TextureLoader.LoadTextures(textureProvider, spriteProvider, "Textures");
            _resolver.RegisterItem(textureProvider);
            _resolver.RegisterItem(spriteProvider);

            // sound provider
            var soundProvider = new SoundProvider();
            SoundLoader.LoadSounds(soundProvider, "Sounds");
            _resolver.RegisterItem(soundProvider);

            // prefab provider
            var prefabProvider = new PrefabProvider();
            PrefabLoader.LoadPrefabs(prefabProvider);
            _resolver.RegisterItem(prefabProvider);

            // pooling
            var poolingObjectManager = new PoolingObjectManager(prefabProvider);
            _resolver.RegisterItem(poolingObjectManager);

            var soundPoolManager = new PoolingAudioPlayer(prefabProvider.GetPrefab("sound_source_prefab"));
            _resolver.RegisterItem(soundPoolManager);

            var particlePoolManager = new PoolingParticleManager(_resolver);
            _resolver.RegisterItem(particlePoolManager);

            // data provider
            var gameDataProvider = new GameDataProvider();
            _resolver.RegisterItem(gameDataProvider);

            // canvas provider
            var canvasProvider = new CanvasProvider();
            _unityReference.LoadCanvases(canvasProvider);
            _resolver.RegisterItem(canvasProvider);


			//Player manager
			var playerManager = new PlayerManager();
            //initialize dummy player model
            playerManager.Initialize(_resolver, InitializeDummyPlayerModel());
			_resolver.RegisterItem(playerManager);


            //input session
			var inputSession  =  new InputSession();
            InputSessionData data = new InputSessionData();
            data.Name = "None";
            inputSession.Initialize(data);
            _resolver.RegisterItem(inputSession);

            #endregion

            // lock the resolver (stop any new items being registered)
            _resolver.Lock();
			_resolver.Resolve (out _gameDataProvider);

            /* BEGIN STATE */
            _currentState = new MenuState(_resolver);
            _currentState.Initialize();

            /* SUBSCRIBE FOR GAME END */
            _onExitGame = _messager.Subscribe<ExitGameMessage>(OnExitGame);

            #region DATA DEFINITIONS (eventually to be moved to data)
            _gameDataProvider.AddData (new BuildingModel
            {
				Name = "gunner_tower",
                BuildingColor = Color.gray,
                Range = 100,
                Stats = new StatBlock
                {
                    MaximumHealth = 200,
                    MaximumDamage = 5,
                    MaximumCourage = -1
                },
				Type = BuildingModel.BuildingType.Defence_Gunner_Towers
            });
		
			_gameDataProvider.AddData (new BuildingModel
			                           {
				Name = "gold_storage",
				BuildingColor = Color.red,
				Range = 50,
				GoldAmount = 1000,
				Stats = new StatBlock
				{
					MaximumHealth = 300,
					MaximumDamage = 1,
					MaximumCourage = -1
				},
				Type = BuildingModel.BuildingType.Gold_Locker
			});
			
			_gameDataProvider.AddData (new BuildingModel
			                           {
				Name = "platoons",
				BuildingColor = Color.red,
				Range = 150,
				Stats = new StatBlock
				{
					MaximumHealth = 100,
					MaximumDamage = 1,
					MaximumCourage = -1
				},
				Type = BuildingModel.BuildingType.Defence_Platoons
			});
			_gameDataProvider.AddData (new BuildingModel
			                           {
				Name = "water_cannon",
				BuildingColor = Color.blue,
				Range = 150,
				Stats = new StatBlock
				{
					MaximumHealth = 100,
					MaximumDamage = 1,
					MaximumCourage = -1
				},
				Type = BuildingModel.BuildingType.Defence_Water_Cannons
			});

            /* PLAYER PIRATES */
            _gameDataProvider.AddData( new PirateModel {
                Name = "Captain",
                Stats = new StatBlock
                {
                    MaximumHealth = 200,
                    MaximumDamage = 25,
                    MaximumCourage = 5
                },
                Descipriton = "Control the crew and gives orders",
                PirateName = "Captain" + UnityEngine.Random.Range (0, 100),
                PirateNature = PirateNature.Player,
                PirateRange = (int)PirateModel.Range.Gunner3,
                PirateColor = Color.blue,
                TrainingCost = 2000
            });

            _gameDataProvider.AddData(new PirateModel
            {
				Name = "Quarter Master",
                Stats = new StatBlock
                {
                    MaximumHealth = 150,
                    MaximumDamage = 20,
                    MaximumCourage = 4
                },
                Descipriton = "Second in command : swords man",
                PirateName = "Quarter Master" + UnityEngine.Random.Range (0, 100),
                PirateNature = (int)PirateNature.Player ,
                PirateRange = (int)PirateModel.Range.Gunner3,
                PirateColor = Color.green,
                TrainingCost = 1000
            });
            _gameDataProvider.AddData(new PirateModel
            {
				Name = "Gunner",
                Stats = new StatBlock
                {
                    MaximumHealth = 100,
                    MaximumDamage = 15,
                    MaximumCourage = 3
                },
                Descipriton = "Attacks and defends the ship from the gun port on deck",
                PirateName = "Gunner" + UnityEngine.Random.Range (0, 100),
                PirateRange = (int)PirateModel.Range.Gunner2,
                PirateNature = PirateNature.Player,
                PirateColor = Color.yellow,
                TrainingCost = 500
            });

			_gameDataProvider.AddData(new PirateModel
			                          {
				Name = "Bomber",
				Stats = new StatBlock
				{
					MaximumHealth = 100,
					MaximumDamage = 15,
					MaximumCourage = 3
				},
				Descipriton = "Crew member : attacks and defends ship from cannon ports",
				PirateName = "Bomber" + UnityEngine.Random.Range (0, 100),
				PirateRange = (int)PirateModel.Range.Gunner2,
				PirateNature = PirateNature.Player,
				PirateColor = Color.white,
                TrainingCost = 1000
            });

			_gameDataProvider.AddData(new PirateModel
			                          {
				Name = "Surgeon",
				Stats = new StatBlock
				{
					MaximumHealth = 100,
					MaximumDamage = 15,
					MaximumCourage = 3
				},
				Descipriton = "Doctor : heals the troops after there return",
				PirateName = "Surgeon" + UnityEngine.Random.Range (0, 100),
				PirateRange = (int)PirateModel.Range.Gunner2,
				PirateNature = PirateNature.Player,
				PirateColor = Color.magenta,
                TrainingCost = 500
            });
		
			_gameDataProvider.AddData(new PirateModel
			                          {
				Name = "Carpenter",
				Stats = new StatBlock
				{
					MaximumHealth = 100,
					MaximumDamage = 15,
					MaximumCourage = 3
				},
				Descipriton = "Builder : builds/repairs the cabin of ship",
				PirateName = "Carpenter" + UnityEngine.Random.Range (0, 100),
				PirateRange = (int)PirateModel.Range.Gunner2,
				PirateNature = PirateNature.Player,
				PirateColor = Color.yellow,
                TrainingCost = 3000
            });
			_gameDataProvider.AddData(new PirateModel
			                          {
				Name = "Chef",
				Stats = new StatBlock
				{
					MaximumHealth = 100,
					MaximumDamage = 15,
					MaximumCourage = 3
				},
				Descipriton = "Cook : creates food for the crew",
				PirateName = "Chef" + UnityEngine.Random.Range (0, 100),
				PirateRange = (int)PirateModel.Range.Gunner2,
				PirateNature = PirateNature.Player,
				PirateColor = Color.yellow,
                TrainingCost = 1000
            });

            /* ENEMY PIRATES */
            _gameDataProvider.AddData(new PirateModel
            {
                Name = "EnemyPirate1",
                Stats = new StatBlock
                {
                    MaximumHealth = 120,
                    MaximumDamage = 10,
                    MaximumCourage = 3
                },
                Descipriton = "Milee Enemy Pirate",
                PirateName = "Enemy " + UnityEngine.Random.Range (0, 100),
                PirateRange = (int)PirateModel.Range.Milee,
                PirateNature = PirateNature.Enemy,
                PirateColor = Color.red,
            });
            _gameDataProvider.AddData(new PirateModel
            {
                Name = "EnemyPirate2",
                Stats = new StatBlock
                {
                    MaximumHealth = 150,
                    MaximumDamage = 8,
                    MaximumCourage = 4
                },
                Descipriton = "Gunner1 Enemy Pirate",
                PirateName = "Enemy " + UnityEngine.Random.Range (0, 100),
                PirateRange = (int)PirateModel.Range.Gunner1,
                PirateNature = PirateNature.Enemy,
                PirateColor = Color.grey
            });
            _gameDataProvider.AddData(new PirateModel
            {
                Name = "EnemyPirate3",
                Stats = new StatBlock
                {
                    MaximumHealth = 100,
                    MaximumDamage = 10,
                    MaximumCourage = 3
                },
                Descipriton = "Gunner2 Enemy Pirate",
                PirateName = "Enemy " + UnityEngine.Random.Range (0, 100),
                PirateRange = (int)PirateModel.Range.Gunner2,
                PirateNature = PirateNature.Enemy,
                PirateColor = Color.black
            });

            #endregion
        }

        private void OnExitGame(ExitGameMessage message)
        {
            Application.Quit();
        }

        public void OnApplicationQuit()
        {
            // save things that need saving here
        }

        public void Update()
        {
            if (_currentState == null) return;

            /* SWITCH STATE IF NEEDED */
            if (_currentState.IsReadyForStateSwitch)
            {
                var previousState = _currentState;
                _currentState = _currentState.TargetSwitchState;

                // probably save important data here too

                previousState.TearDown();
                _unityReference.ResetDelayedActions();
                _currentState.Initialize();
            }

            /* UPDATE STATE */
            _currentState.Update ();
            _currentState.HandleInput();
        }

        public PlayerModel InitializeDummyPlayerModel() {

            PlayerModel playerModel = new PlayerModel();

            Dictionary<int, string> seatsDictionary;
            //level Manager
            Dictionary<string, int> _pirateCountDict;
            Dictionary<string, bool> _unlockedPirates;
            Dictionary<string, int> _shipAttacksCountDict;
            Dictionary<string, int> _shipAttackCostDict;
            Dictionary<string, bool> _unlockedShipAttacks;
            Dictionary<string, Dictionary<int, string>> _rowBoatDict;

        _unlockedPirates = new Dictionary<string, bool>();
            _unlockedPirates.Add("Captain", true);
            _unlockedPirates.Add("Quarter Master", true);
            _unlockedPirates.Add("Gunner", true);
            _unlockedPirates.Add("Bomber", true);
            _unlockedPirates.Add("Surgeon", true);
            _unlockedPirates.Add("Carpenter", false);
            _unlockedPirates.Add("Chef", false);
            _unlockedPirates.Add("Pirate4", false);
            _unlockedPirates.Add("EnemyPirate5", false);

            _pirateCountDict = new Dictionary<string, int>();
            _pirateCountDict.Add("Captain", 1);
            _pirateCountDict.Add("Quarter Master", 2);
            _pirateCountDict.Add("Gunner", 5);
            _pirateCountDict.Add("Bomber", 3);
            _pirateCountDict.Add("Surgeon", 4);
            _pirateCountDict.Add("Carpenter", 2);
            _pirateCountDict.Add("Chef", 2);
            _pirateCountDict.Add("Pirate4", 1);
            _pirateCountDict.Add("EnemyPirate3", 10);

            _shipAttacksCountDict = new Dictionary<string, int>();
            _shipAttacksCountDict.Add("Gas", 2);
            _shipAttacksCountDict.Add("Fire", 2);
            _shipAttacksCountDict.Add("Bomb", 2);
            _shipAttacksCountDict.Add("Gun", 2);

            _shipAttackCostDict = new Dictionary<string, int>();
            _shipAttackCostDict.Add("Gas", 2);
            _shipAttackCostDict.Add("Fire", 3);
            _shipAttackCostDict.Add("Bomb", 4);
            _shipAttackCostDict.Add("Gun", 11);

            _unlockedShipAttacks = new Dictionary<string, bool>();
            _unlockedShipAttacks.Add("Gas", true);
            _unlockedShipAttacks.Add("Fire", true);
            _unlockedShipAttacks.Add("Bomb", true);
            _unlockedShipAttacks.Add("Gun", true);


            _rowBoatDict = new Dictionary<string, Dictionary<int, string>>();

            //boat1
            seatsDictionary = new Dictionary<int, string>();
            seatsDictionary.Add(0, "Quarter Master");
            seatsDictionary.Add(1, "Captain");
            seatsDictionary.Add(2, "Gunner");
            seatsDictionary.Add(3, "Bomber");
            seatsDictionary.Add(4, "");
            seatsDictionary.Add(5, "");

            _rowBoatDict.Add("Boat1", seatsDictionary);

            //boat2
            seatsDictionary = new Dictionary<int, string>();
            seatsDictionary.Add(0, "Quarter Master");
            seatsDictionary.Add(1, "Captain");
            seatsDictionary.Add(2, "Gunner");
            seatsDictionary.Add(3, "Bomber");
            seatsDictionary.Add(4, "Surgeon");
            seatsDictionary.Add(5, "Gunner");


            _rowBoatDict.Add("Boat2", seatsDictionary);

            //boat 3
            seatsDictionary = new Dictionary<int, string>();
            seatsDictionary.Add(0, "Bomber");
            seatsDictionary.Add(1, "Captain");
            seatsDictionary.Add(2, "");
            seatsDictionary.Add(3, "");
            seatsDictionary.Add(4, "");
            seatsDictionary.Add(5, "");

            _rowBoatDict.Add("Boat3", seatsDictionary);

            //boat4
            seatsDictionary = new Dictionary<int, string>();
            seatsDictionary.Add(0, "");
            seatsDictionary.Add(1, "");
            seatsDictionary.Add(2, "");
            seatsDictionary.Add(3, "");
            seatsDictionary.Add(4, "");
            seatsDictionary.Add(5, "");
            _rowBoatDict.Add("Boat4", seatsDictionary);

            playerModel.Name = "User";
            playerModel.Email = "user@gmail.com";
            playerModel.Gold = 0;
            playerModel.ExperiencePoints = 0;
            playerModel.UserLevel = 0;
            playerModel.UserRank = 0;
            playerModel.Wins = 0;
            playerModel.Gems = 100;
            playerModel.MaxGoldCapacity = 2000;
            playerModel.UnlockedPirates = _unlockedPirates;
            playerModel.PirateCountDict = _pirateCountDict;
            playerModel.ShipBulletsAvailable = 10;
            playerModel.ShipAttacksCountDict = _shipAttacksCountDict;
            playerModel.UnlockedShipAttacks = _unlockedShipAttacks;
            playerModel.ShipAttackCostDict = _shipAttackCostDict;
            playerModel.RowBoatCountDict = _rowBoatDict;

            return playerModel;

        }



    }
}
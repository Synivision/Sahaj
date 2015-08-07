using System;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Loading;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Extensions;
using Assets.Code.Logic.Pooling;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.States;
using Assets.Code.Utilities;
using UnityEngine;


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
        private IoCResolver _resolver;

        private BaseState _currentState;

        /* TOKENS */
        private MessagingToken _onExitGame;

        public void Start()
        {
            _resolver = new IoCResolver();

            /* RESOURCE LIST CREATION */
#if UNITY_EDITOR
            FileServices.CreateResourcesList("Assets/Resources/resourceslist.txt");
#endif
            FileServices.LoadResourcesList("resourceslist");

            #region LOAD RESOURCES
            // messager
            _messager = new Messager();
            _resolver.RegisterItem(_messager);

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
			_resolver.RegisterItem(playerManager);
            #endregion

            // lock the resolver (stop any new items being registered)
            _resolver.Lock();
			_resolver.Resolve (out _gameDataProvider);

            /* BEGIN STATE */
            _currentState = new MenuState(_resolver);
            _currentState.Initialize();

            /* SUBSCRIBE FOR GAME END */
            _onExitGame = _messager.Subscribe<ExitGameMessage>(OnExitGame);


			//add data to dame data provider

			_gameDataProvider.AddData<PirateModel>(GeneratePirate1());
			_gameDataProvider.AddData<PirateModel>(GeneratePirate2());
			_gameDataProvider.AddData<PirateModel>(GeneratePirate3());
			_gameDataProvider.AddData<PirateModel>(GenerateEnemy1());
			_gameDataProvider.AddData<PirateModel>(GenerateEnemy2());
			_gameDataProvider.AddData<PirateModel>(GenerateEnemy3());

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

		public IoCResolver Resolver{get{

				return _resolver;
			}set{

				_resolver = value;
			}}

		public PirateModel GeneratePirate1(){
			
			PirateModel pirateModel=new PirateModel();
			pirateModel.Name = "Pirate1";
			pirateModel.Health = 200;
			pirateModel.Descipriton = "Control the crew and gives orders";
			pirateModel.AttackDamage = 25;
			pirateModel.PirateName = "Captain" + UnityEngine.Random.Range (0, 100).ToString ();
			pirateModel.Courage = 5;
			pirateModel.PirateNature = (int)PirateModel.Nature.Player ;
			pirateModel.PirateRange = (int)PirateModel.Range.Gunner3;
			pirateModel.PirateColor = Color.blue;
			
			return pirateModel;
		}
		public PirateModel GeneratePirate2(){
			
			//Quarter Master
			PirateModel pirateModel=new PirateModel();
			pirateModel.Name = "Pirate2";
			pirateModel.Health = 150;
			pirateModel.Descipriton = "Second in command : swords man";
			pirateModel.AttackDamage = 20;
			pirateModel.PirateName = "Quarter Master" + UnityEngine.Random.Range (0, 100).ToString();
			pirateModel.Courage = 4;
			pirateModel.PirateNature = (int)PirateModel.Nature.Player ;
			pirateModel.PirateRange = (int)PirateModel.Range.Gunner3;
			pirateModel.PirateColor = Color.green;
			
			return pirateModel;
		}
		
		public PirateModel GeneratePirate3(){
			
			//Gunner
			PirateModel pirateModel=new PirateModel();
			pirateModel.Name = "Pirate3";
			pirateModel.Health = 100;
			pirateModel.Descipriton = "Attacks and defends the ship from the gun port on deck";
			pirateModel.AttackDamage = 15;
			pirateModel.PirateName = "Gunner" + UnityEngine.Random.Range (0, 100).ToString ();
			pirateModel.Courage = 3;
			pirateModel.PirateRange = (int)PirateModel.Range.Gunner2;
			pirateModel.PirateNature = (int)PirateModel.Nature.Player ;
			pirateModel.PirateColor = Color.yellow;
			
			return pirateModel;
		}
		
		private PirateModel GenerateEnemy1(){
			
			PirateModel pirateModel=new PirateModel();
			pirateModel.Name = "EnemyPirate1";
			pirateModel.Health = 120;
			pirateModel.Descipriton = "Milee Enemy Pirate";
			pirateModel.AttackDamage = 10;
			pirateModel.PirateName = "Enemy " + UnityEngine.Random.Range (0, 100).ToString ();
			pirateModel.Courage = 3;
			pirateModel.PirateRange = (int)PirateModel.Range.Milee;
			pirateModel.PirateNature = (int)PirateModel.Nature.Enemy ;
			pirateModel.PirateColor = Color.red;
			
			return pirateModel;
		}
		
		private PirateModel GenerateEnemy2(){
			
			PirateModel pirateModel=new PirateModel();
			pirateModel.Name = "EnemyPirate2";
			pirateModel.Health = 150;
			pirateModel.Descipriton = "Gunner1 Enemy Pirate";
			pirateModel.AttackDamage = 8;
			pirateModel.PirateName = "Enemy " + UnityEngine.Random.Range (0, 100).ToString ();
			pirateModel.Courage = 4;
			pirateModel.PirateRange = (int)PirateModel.Range.Gunner1;
			pirateModel.PirateNature = (int)PirateModel.Nature.Enemy ;
			pirateModel.PirateColor = Color.grey;
			
			return pirateModel;
		}
		
		private PirateModel GenerateEnemy3(){
			
			PirateModel pirateModel=new PirateModel();
			pirateModel.Name = "EnemyPirate3";
			pirateModel.Health = 100;
			pirateModel.Descipriton = "Gunner2 Enemy Pirate";
			pirateModel.AttackDamage = 10;
			pirateModel.PirateName = "Enemy " + UnityEngine.Random.Range (0, 100).ToString ();
			pirateModel.Courage = 3;
			pirateModel.PirateRange = (int)PirateModel.Range.Gunner2;
			pirateModel.PirateNature = (int)PirateModel.Nature.Enemy ;
			pirateModel.PirateColor = Color.black;
			
			return pirateModel;
		}

    }

}
using Assets.Code.DataPipeline;
using Assets.Code.Messaging;
using Assets.Code.Ui;
using UnityEngine;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Ui.CanvasControllers;
using Assets.Code.Messaging.Messages;

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
		private GameDataProvider _gameDataProvider;
		//tokens
		private MessagingToken _onQuitGame;
		private MessagingToken _onCreatePirate;
		private MessagingToken _onTearDownLevel;
		private PoolingObjectManager _poolingObjectManager;
		//level Manager

		LevelManager levelManager;

		public PlayState (IoCResolver resolver) : base(resolver)
		{
			_resolver.Resolve (out _messager);
			_resolver.Resolve (out _prefabProvider);
			_resolver.Resolve (out _canvasProvider);
			_resolver.Resolve (out _poolingObjectManager);
			_resolver.Resolve (out _gameDataProvider);
		}

		public override void Initialize ()
		{
			_uiManager = new UiManager ();

			_uiManager.RegisterUi (new MainCanvasController (_resolver, _canvasProvider.GetCanvas ("MainCanvas")));
			_uiManager.RegisterUi (new PirateInfoCanvasController (_resolver, _canvasProvider.GetCanvas ("PirateInfoCanvas")));

			_uiManager.RegisterUi (new GamePlayCanvasController (_resolver, _canvasProvider.GetCanvas ("GamePlayCanvas")));
			Debug.Log ("Play state initialized.");

			//Instantiate CameraController and InputController
			var controller = _poolingObjectManager.Instantiate ("Controller");
			controller.gameObject.GetComponent<InputController> ().Initialize (_resolver);

			//Initialize level manager
			levelManager = new LevelManager (_resolver);

			//Message tokens
			_onQuitGame = _messager.Subscribe<QuitGameMessage> (OnQuitGame);
			_onCreatePirate = _messager.Subscribe<CreatePirateMessage> (OnCreatePirate);
			_onTearDownLevel = _messager.Subscribe<TearDownLevelMessage> (OnTearDownLevel);


			_gameDataProvider.AddData<PirateModel>(GeneratePirate1());
			_gameDataProvider.AddData<PirateModel>(GeneratePirate2());
			_gameDataProvider.AddData<PirateModel>(GeneratePirate3());
			_gameDataProvider.AddData<PirateModel>(GenerateEnemy1());
			_gameDataProvider.AddData<PirateModel>(GenerateEnemy2());
			_gameDataProvider.AddData<PirateModel>(GenerateEnemy3());
		}

		public void OnCreatePirate (CreatePirateMessage message)
		{
			switch(message.PirateName){
			
			case "Pirate1":
				levelManager.CreatePirate(message.PirateName,message.SpawnPosition);
				break;

			case "Pirate2":
				levelManager.CreatePirate(message.PirateName,message.SpawnPosition);
				break;

			case "Pirate3":
				levelManager.CreatePirate(message.PirateName,message.SpawnPosition);
				break;

			case "EnemyPirate1":
				levelManager.CreatePirate(message.PirateName,message.SpawnPosition);
				break;

			case "EnemyPirate2":
				levelManager.CreatePirate(message.PirateName,message.SpawnPosition);
				break;

			case "EnemyPirate3":
				levelManager.CreatePirate(message.PirateName,message.SpawnPosition);
				break;

			}
		}

		public override void Update ()
		{
			_uiManager.Update ();
            
			// super general input goes here
		}




		private void OnQuitGame (QuitGameMessage message)
		{
			SwitchState (new MenuState (_resolver));

		}

		public override void HandleInput ()
		{

		}

		public PirateModel GeneratePirate1(){
			
			PirateModel pirateModel=new PirateModel();
			pirateModel.Name = "Pirate1";
			pirateModel.Health = 200;
			pirateModel.Descipriton = "Control the crew and gives orders";
			pirateModel.AttackDamage = 25;
			pirateModel.PirateName = "Captain" + Random.Range (0, 100).ToString ();
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
			pirateModel.PirateName = "Quarter Master" + Random.Range (0, 100).ToString();
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
			pirateModel.PirateName = "Gunner" + Random.Range (0, 100).ToString ();
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
			pirateModel.PirateName = "Enemy " + Random.Range (0, 100).ToString ();
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
			pirateModel.PirateName = "Enemy " + Random.Range (0, 100).ToString ();
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
			pirateModel.PirateName = "Enemy " + Random.Range (0, 100).ToString ();
			pirateModel.Courage = 3;
			pirateModel.PirateRange = (int)PirateModel.Range.Gunner2;
			pirateModel.PirateNature = (int)PirateModel.Nature.Enemy ;
			pirateModel.PirateColor = Color.black;
			
			return pirateModel;
		}

		private void OnTearDownLevel (TearDownLevelMessage message)
		{

			levelManager.TearDownLevel ();
		}

		public override void TearDown ()
		{
			_messager.CancelSubscription (_onQuitGame, _onTearDownLevel, _onCreatePirate);
			levelManager.TearDownLevel ();
			_uiManager.TearDown ();
			_poolingObjectManager.TearDown ();

		}
	}
}

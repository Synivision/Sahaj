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
		private MessagingToken _onCreatePirate;
		private MessagingToken _onTearDownLevel;
		private PoolingObjectManager _poolingObjectManager;
		//level Manager

		private Dictionary<string,bool> _unlockedPirates;

		LevelManager levelManager;
		PlayerManager _playerManager;
		public PlayState (IoCResolver resolver) : base(resolver)
		{
			_resolver.Resolve (out _messager);
			_resolver.Resolve (out _prefabProvider);
			_resolver.Resolve (out _canvasProvider);
			_resolver.Resolve (out _poolingObjectManager);
			_resolver.Resolve (out _playerManager);

		}

		public override void Initialize ()
		{
			_uiManager = new UiManager ();

			//_uiManager.RegisterUi (new MainCanvasController (_resolver, _canvasProvider.GetCanvas ("MainCanvas")));
			_uiManager.RegisterUi (new PirateInfoCanvasController (_resolver, _canvasProvider.GetCanvas ("PirateInfoCanvas")));

			_unlockedPirates = new Dictionary<string, bool>();
			_unlockedPirates.Add("Pirate1",true);
			_unlockedPirates.Add("Pirate2",true);
			_unlockedPirates.Add("Pirate3",true);
			_unlockedPirates.Add("EnemyPirate1",true);
			_unlockedPirates.Add("EnemyPirate2",true);
			_unlockedPirates.Add("EnemyPirate3",true);
			_unlockedPirates.Add ("EnemyPirate4",false);
			_unlockedPirates.Add ("Pirate4",false);
			_unlockedPirates.Add ("EnemyPirate5",false);

			//Instantiate CameraController and InputController
			var controller = _poolingObjectManager.Instantiate ("Controller");
			controller.gameObject.GetComponent<InputController> ().Initialize (_resolver);

			//Initialize level manager
			levelManager = new LevelManager (_resolver);


			PlayerModel playerModel = new PlayerModel();
			playerModel.Name = "User";
			playerModel.Email = "user@gmail.com";
			playerModel.Gold = 350;
			playerModel.ExperiencePoints = 50;
			playerModel.UserLevel =  34;
			playerModel.UserRank = 89;
			playerModel.Wins = 84;
			playerModel.Gems = 895;
			playerModel.UnlockedPirates = _unlockedPirates;



			_playerManager.Initialize(_resolver,playerModel,levelManager);

			_uiManager.RegisterUi (new GamePlayCanvasController (_resolver, _canvasProvider.GetCanvas ("GamePlayCanvas"), _playerManager));
			//Debug.Log ("Play state initialized.");

			//Message tokens
			_onQuitGame = _messager.Subscribe<QuitGameMessage> (OnQuitGame);
			_onCreatePirate = _messager.Subscribe<CreatePirateMessage> (OnCreatePirate);
			_onTearDownLevel = _messager.Subscribe<TearDownLevelMessage> (OnTearDownLevel);


		
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

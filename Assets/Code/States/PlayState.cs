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

		//tokens
		private MessagingToken _onQuitGame;
		private MessagingToken _onCreatePirate;
		private MessagingToken _onTearDownLevel;
		//level Manager

		LevelManager levelManager;

		public PlayState (IoCResolver resolver) : base(resolver)
		{
			_resolver.Resolve (out _messager);
			_resolver.Resolve (out _prefabProvider);
			_resolver.Resolve (out _canvasProvider);
		}

		public override void Initialize ()
		{
			_uiManager = new UiManager ();
			//_uiManager.RegisterUi( ... );

			_uiManager.RegisterUi (new MainCanvasController (_resolver, _canvasProvider.GetCanvas ("MainCanvas")));
			_uiManager.RegisterUi (new PirateInfoCanvasController (_resolver, _canvasProvider.GetCanvas ("PirateInfoCanvas")));

			Debug.Log ("Play state initialized.");

			//initialize level manager
			levelManager = new LevelManager (_resolver);

			//message tokens

			_onQuitGame = _messager.Subscribe<QuitGameMessage> (OnQuitGame);
			_onCreatePirate = _messager.Subscribe<CreatePirateMessage> (OnCreatePirate);
			_onTearDownLevel = _messager.Subscribe<TearDownLevelMessage> (OnTearDownLevel);
		}

		public void OnCreatePirate (CreatePirateMessage message)
		{

			levelManager.CreatePirate ();

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
			levelManager.TearDownLevel();
			_uiManager.TearDown ();

		}
	}
}

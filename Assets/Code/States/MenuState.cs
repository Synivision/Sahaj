using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.Messaging;
using Assets.Code.Ui;
using UnityEngine;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Ui.CanvasControllers;
using Assets.Code.States;
using Assets.Code.Messaging.Messages;

public class MenuState : BaseState{

	public PrefabProvider _prefabProvider;
	/* REFERENCES */
	private readonly Messager _messager;
	private CanvasProvider _canvasProvider;
	private UiManager _uiManager;
	private MessagingToken _onStartGame;
	private PoolingObjectManager _poolingObjectManager;

	
	/* PROPERTIES */
	
	public MenuState (IoCResolver resolver) : base(resolver)
	{
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _prefabProvider);
		_resolver.Resolve (out _canvasProvider);
		_resolver.Resolve (out _poolingObjectManager);
	}
	public override void Initialize ()
	{
		//Debug.Log ("Menu state initialized.");
		//message tokens
		_onStartGame = _messager.Subscribe<StartGameMessage>(OnStartGame);
		_uiManager = new UiManager ();

		_uiManager.RegisterUi(new MenuCanvasController(_resolver, _canvasProvider.GetCanvas("MenuCanvas")));


	}

	private void OnStartGame(StartGameMessage message)
	{
		SwitchState(new PlayState(_resolver, message.MapLayout));
	}

	public override void Update ()
	{
		_uiManager.Update ();
		
		// super general input goes here
	}
	
	public override void HandleInput ()
	{

	}
	public override void TearDown ()
	{
		_messager.CancelSubscription (_onStartGame);
		_uiManager.TearDown ();
		_poolingObjectManager.TearDown();

	}

}

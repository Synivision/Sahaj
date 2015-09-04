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
	public class ShipBaseState : BaseState
	{
		
		private Canvas _shipBaseCanvas;
		
		private CanvasProvider _canvasProvider;
		private UiManager _uiManager;
		ShipLevelManager shipLevelManager;
		private readonly Messager _messager;
		private MessagingToken _onChangeStateToAttack;
		MapLayout _map;

		public ShipBaseState (IoCResolver resolver) : base(resolver){
			
			_resolver.Resolve (out _canvasProvider);
			_resolver.Resolve (out _messager);

		}
		
		public override void Initialize (){

			_uiManager = new UiManager ();
			_uiManager.RegisterUi(new ShipBaseCanvasController(_resolver,_canvasProvider.GetCanvas("ShipBaseCanvas")));
		
			_map = new MapLayout();

			shipLevelManager = new ShipLevelManager(_resolver,_map);
			_onChangeStateToAttack = _messager.Subscribe<ShipBaseToAttackStateMessage>(OnChangeStateToAttack);	
		
		}

		public void OnChangeStateToAttack(ShipBaseToAttackStateMessage message){

			SwitchState (new PlayState(_resolver,message.MapLayout));
		}

		public override void Update (){
			
		}
		
		public override void HandleInput (){
			
		}
		
		public override void TearDown (){

			_uiManager.TearDown();
			shipLevelManager.TearDown();
		}


		
	}
	
}

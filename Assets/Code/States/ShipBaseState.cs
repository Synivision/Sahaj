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

		private readonly Messager _messager;

		public ShipBaseState (IoCResolver resolver) : base(resolver){
			
			_resolver.Resolve (out _canvasProvider);
			_resolver.Resolve (out _messager);

		}
		
		public override void Initialize (){

			_uiManager = new UiManager ();
			_uiManager.RegisterUi(new ShipBaseCanvasController(_resolver,_canvasProvider.GetCanvas("ShipBaseCanvas")));
		}
		
		public override void Update (){
			
		}
		
		public override void HandleInput (){
			
		}
		
		public override void TearDown (){
			
		}


		
	}
	
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.UnityBehaviours;
using Assets.Code.States;

namespace Assets.Code.Ui.CanvasControllers
{
	public class ShipBaseCanvasController : BaseCanvasController {
		
		private Button _attackButton;
		private Button _shopButton;
		private Text _fps;
		
		private Canvas _canvasView;
		private IoCResolver _resolver;
		private readonly Messager _messager;
		
		public ShipBaseCanvasController (IoCResolver resolver, Canvas canvasView) : base(canvasView)
		{
			_canvasView = canvasView;
			_resolver = resolver;
			_resolver.Resolve (out _messager);
			
			ResolveElement (out _attackButton, "AttackButton");
			ResolveElement (out _shopButton, "ShopButton");
			ResolveElement (out _fps,"FpsText");
			
		}
		
	}
	
}

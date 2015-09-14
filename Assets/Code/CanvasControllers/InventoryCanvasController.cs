using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.UnityBehaviours;
using Assets.Code.States;
namespace Assets.Code.Ui.CanvasControllers{

	public class InventoryCanvasController : BaseCanvasController {
		
		private readonly Messager _messager;
		private readonly PrefabProvider _prefabProvider;
		private IoCResolver _resolver;
		private UiManager _uiManager;
		private CanvasProvider _canvasProvider;
		private Button _closeButton;
		private MessagingToken _onFindInventoryItem;
		
		public InventoryCanvasController (IoCResolver resolver, Canvas canvasView) : base(canvasView)
		{
			
			_resolver = resolver;
			_resolver.Resolve (out _messager);
			_resolver.Resolve (out _canvasProvider);
			_uiManager = new UiManager ();
			
			
			ResolveElement(out _closeButton, "CloseButton");
			_canvasView.gameObject.SetActive (true);
			// subscriptions
			//_onFindInventoryItem = _messager.Subscribe<FindInventoryItemMessage>(OnFindInventoryItem);

			_closeButton.onClick.AddListener(onCloseClicked);
		}

		void onCloseClicked(){


		}
		
		public void OnFindInventoryItem(FindInventoryItemMessage message){
			
			
			
		}
		
		public override void TearDown()
		{


		}
		
	}
}

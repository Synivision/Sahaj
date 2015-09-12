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
namespace Assets.Code.Ui.CanvasControllers
{
public class FindInventoryItemCanvasController : BaseCanvasController {

	private readonly Messager _messager;
	private readonly PrefabProvider _prefabProvider;
	private IoCResolver _resolver;
	private UiManager _uiManager;
	private CanvasProvider _canvasProvider;
	private Button _addButton;
		private MessagingToken _onFindInventoryItem;

		public FindInventoryItemCanvasController (IoCResolver resolver, Canvas canvasView) : base(canvasView)
	{
	
		_resolver = resolver;
		_resolver.Resolve (out _messager);
		_resolver.Resolve (out _canvasProvider);
		_uiManager = new UiManager ();


		ResolveElement(out _addButton, "add_Button");
		
		// subscriptions
		//_onFindInventoryItem = _messager.Subscribe<FindInventoryItemMessage>(OnFindInventoryItem);

	}


		public void OnFindInventoryItem(FindInventoryItemMessage message){



		}

		public override void TearDown()
		{}

  }
}
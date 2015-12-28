
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
	public class WinLooseCanvasController : BaseCanvasController
	{
		private readonly Messager _messager;
		private readonly PrefabProvider _prefabProvider;
		private IoCResolver _resolver;
		private UiManager _uiManager;
		private CanvasProvider _canvasProvider;
		private Button _returnButton;

        public WinLooseCanvasController(IoCResolver resolver, Canvas canvasView)
            : base(resolver, canvasView)
		{
			
			_resolver = resolver;
			_resolver.Resolve(out _messager);
			_resolver.Resolve(out _canvasProvider);
			_uiManager = new UiManager();
			
			
			ResolveElement (out _returnButton, "ReplayButton");
			_canvasView.gameObject.SetActive(true);

			_returnButton.onClick.AddListener(onReturnClicked);
		}
		
		void onReturnClicked()
		{
			
			_messager.Publish (new OpenShipBaseMessage{});
			TearDown();
		}
		
		public override void TearDown()
		{
			_returnButton.onClick.RemoveAllListeners();
			base.TearDown();
			
		}
	}
}

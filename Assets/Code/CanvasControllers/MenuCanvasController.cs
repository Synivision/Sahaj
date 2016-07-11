using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;

namespace Assets.Code.Ui.CanvasControllers
{
	public class MenuCanvasController : BaseCanvasController
	{
		private readonly Button _playGameButton;
		private readonly Button _shipBaseGameButton;
		
		private CanvasProvider _canvasProvider;
		private readonly Messager _messager;
		private UiManager _uiManager;
		private IoCResolver _resolver;

        public MenuCanvasController(IoCResolver resolver, Canvas canvasView)
            : base(resolver, canvasView)
		{
			_resolver = resolver;
			ResolveElement (out _shipBaseGameButton, "ship_base_button");

			
			_resolver.Resolve(out _messager);
			_resolver.Resolve(out _canvasProvider);
			_uiManager = new UiManager ();
			
			_shipBaseGameButton.onClick.AddListener (OnShipBaseButtonClicked);

		}
		
		void OnShipBaseButtonClicked(){
			
			//_uiManager.RegisterUi(new LevelSelectCanvasController(_resolver, _canvasProvider.GetCanvas("LevelSelectCanvas")));
			_messager.Publish (new OpenShipBaseMessage{});
			TearDown();
		}
		
		public override void TearDown()
		{	
			
			//Remove Listeners
			_shipBaseGameButton.onClick.RemoveAllListeners();
			base.TearDown();
		}
	}
}

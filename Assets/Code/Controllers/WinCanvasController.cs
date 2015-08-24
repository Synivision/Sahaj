using UnityEngine;
using System.Collections;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.DataPipeline;
using UnityEngine.UI;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;

namespace Assets.Code.Ui.CanvasControllers
{
	public class WinCanvasController: BaseCanvasController {

		private Messager _messager;
		private readonly SpriteProvider _spriteProvider;
		private readonly PrefabProvider _prefabProvider;
		private  Canvas _canvas;
		private GameObject panel;
		private readonly MessagingToken _onWin;
		private readonly Button _replayButton;
		public WinCanvasController (IoCResolver resolver, Canvas canvasView) : base(canvasView)
		{
			
			resolver.Resolve (out _messager);
			resolver.Resolve (out _spriteProvider);
			resolver.Resolve (out _prefabProvider);

			ResolveElement (out _replayButton, "ReplayButton");

			_canvas = _canvasView;
			_canvas.enabled = true;
			//token
			//_onWin = _messager.Subscribe<WinMessage> (OnWin);

			_replayButton.onClick.AddListener (OnReplayClicked);
		}

		private  void OnReplayClicked ()
		{


		}

}
}
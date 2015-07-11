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

		private readonly Messager _messager;

		public MenuCanvasController (IoCResolver resolver, Canvas canvasView) : base(canvasView)
		{
			
			ResolveElement (out _playGameButton, "play_game_button");


			resolver.Resolve(out _messager);
			_playGameButton.onClick.AddListener (startGame);

		}

		void startGame(){
			_messager.Publish(new StartGameMessage{

			});

		}

		public override void TearDown()
		{	

			//Remove Listeners
			_playGameButton.onClick.RemoveAllListeners();
			base.TearDown();
		}
	}
}

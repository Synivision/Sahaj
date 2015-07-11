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
	public class MainCanvasController : BaseCanvasController
	{
		private readonly Messager _messager;
		private readonly Button _makePirateButton;
		private readonly Button _tearDownPirateButton;
		private readonly Button _quitButton;

		public MainCanvasController (IoCResolver resolver, Canvas canvasView) : base(canvasView)
		{

			resolver.Resolve(out _messager);

			ResolveElement (out _makePirateButton, "make_pirate_button");
			ResolveElement (out _tearDownPirateButton, "teardown_pirate_button");
			ResolveElement (out _quitButton,"quit_button");	


			_makePirateButton.onClick.AddListener (generateRandomObjects);
			_tearDownPirateButton.onClick.AddListener (tearDownObjects);
			_quitButton.onClick.AddListener(quitGame);
		}

		void quitGame(){
			_messager.Publish(new QuitGameMessage{

			});

		}

		void generateRandomObjects ()
		{
			//send message to play state to make pirates
			_messager.Publish(new CreatePirateMessage{

			});

		}
				
		void tearDownObjects ()
		{


			//send message to play state to teardown level
			_messager.Publish(new TearDownLevelMessage{});

		}

		public override void TearDown()
		{	

			//Remove Listeners
			_makePirateButton.onClick.RemoveAllListeners();
			_tearDownPirateButton.onClick.RemoveAllListeners();
			_quitButton.onClick.RemoveAllListeners();
			base.TearDown();
		}


	}
	
}

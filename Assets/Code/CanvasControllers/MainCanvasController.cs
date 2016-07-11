using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.UnityBehaviours;

namespace Assets.Code.Ui.CanvasControllers
{
	public class MainCanvasController : BaseCanvasController
	
	{
		private readonly Messager _messager;
		private readonly Button _makePirate1Button;
		private readonly Button _makePirate2Button;
		private readonly Button _makePirate3Button;

		private readonly Button _makeEnemyPirate1Button;
		private readonly Button _makeEnemyPirate2Button;
		private readonly Button _makeEnemyPirate3Button;

		private readonly Text _fpsText;

		private readonly Button _quitButton;

		private UnityReferenceMaster _unityReference;

        public MainCanvasController(IoCResolver resolver, Canvas canvasView)
            : base(resolver, canvasView)
		{
			resolver.Resolve (out _messager);
			resolver.Resolve (out _unityReference);
			ResolveElement (out _makePirate1Button, "makePirate1_button");
			ResolveElement (out _makePirate2Button, "makePirate2_button");
			ResolveElement (out _makePirate3Button, "makePirate3_button");

			ResolveElement (out _makeEnemyPirate1Button, "makeEnemyPirate1_button");
			ResolveElement (out _makeEnemyPirate2Button, "makeEnemyPirate2_button");
			ResolveElement (out _makeEnemyPirate3Button, "makeEnemyPirate3_button");
			ResolveElement (out _fpsText,"fpsText");

			ResolveElement (out _quitButton, "quit_button");	


			_makePirate1Button.onClick.AddListener (OnMakePirate1ButtonClicked);
			_makePirate2Button.onClick.AddListener (OnMakePirate2ButtonClicked);
			_makePirate3Button.onClick.AddListener (OnMakePirate3ButtonClicked);

			_makeEnemyPirate1Button.onClick.AddListener(OnMakeEnemyPirate1ButtonClicked);
			_makeEnemyPirate2Button.onClick.AddListener(OnMakeEnemyPirate2ButtonClicked);
			_makeEnemyPirate3Button.onClick.AddListener(OnMakeEnemyPirate3ButtonClicked);
			_quitButton.onClick.AddListener (OnQuitButtonClicked);
		}

		void OnQuitButtonClicked ()
		{
			_messager.Publish (new QuitGameMessage{

			});

		}

		void OnMakePirate1ButtonClicked ()
		{
			//send message to play state to make pirates
			_messager.Publish (new CreatePirateMessage{
				PirateName = "Pirate1"
			});

		}
				
		void OnMakePirate2ButtonClicked ()
		{
			//send message to play state to teardown level
			_messager.Publish (new CreatePirateMessage{
				PirateName = "Pirate2"
			});

		}

		void OnMakePirate3ButtonClicked(){
			_messager.Publish (new CreatePirateMessage{
				PirateName = "Pirate3"
			});

		}

		void OnMakeEnemyPirate1ButtonClicked(){

			_messager.Publish (new CreatePirateMessage{
				PirateName = "EnemyPirate1"
			});
		}

		void OnMakeEnemyPirate2ButtonClicked(){
			_messager.Publish (new CreatePirateMessage{
				PirateName = "EnemyPirate2"
			});
		}

		void OnMakeEnemyPirate3ButtonClicked(){

			_messager.Publish (new CreatePirateMessage{
				PirateName = "EnemyPirate3"
			});

		}

		public override void TearDown ()
		{	
			//Remove Listeners
			_makePirate1Button.onClick.RemoveAllListeners ();
			_makePirate2Button.onClick.RemoveAllListeners ();
			_makePirate3Button.onClick.RemoveAllListeners ();
			_makeEnemyPirate1Button.onClick.RemoveAllListeners ();
			_makeEnemyPirate2Button.onClick.RemoveAllListeners ();
			_makeEnemyPirate3Button.onClick.RemoveAllListeners ();


			_quitButton.onClick.RemoveAllListeners ();
			base.TearDown ();
		}

		public override void Update ()
		{
			base.Update ();
			   
			var frameRate = (int)(1.0 / Time.deltaTime);
			_fpsText.text = frameRate.ToString()+" fps";

			

		

		
		}


	}
	
}

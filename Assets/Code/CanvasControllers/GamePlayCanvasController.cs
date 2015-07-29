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
	public class GamePlayCanvasController : BaseCanvasController {

		private readonly Messager _messager;
		private readonly Button _makePirate1Button;
		private readonly Button _makePirate2Button;
		private readonly Button _makePirate3Button;
		
		private readonly Button _makeEnemyPirate1Button;
		private readonly Button _makeEnemyPirate2Button;
		private readonly Button _makeEnemyPirate3Button;
		
		private readonly Text _fpsText;
		private Canvas _canvas;
		private readonly Button _quitButton;
		
		private UnityReferenceMaster _unityReference;

		private List<Button> _buttonList;

		public GamePlayCanvasController (IoCResolver resolver, Canvas canvasView) : base(canvasView)
		{
			_canvas = canvasView;
			resolver.Resolve (out _messager);
			resolver.Resolve (out _unityReference);

			ResolveElement (out _fpsText,"FpsText");
			ResolveElement(out _quitButton,"QuitButton");

			var panel = GetElement("SelectPiratePanel");
			var contentpanel = panel.transform.GetChild(0);
			_makePirate1Button = contentpanel.GetChild(0).GetComponent<Button>();
			_makePirate2Button = contentpanel.GetChild(1).GetComponent<Button>();
			_makePirate3Button = contentpanel.GetChild(2).GetComponent<Button>();
			_makeEnemyPirate1Button = contentpanel.GetChild(3).GetComponent<Button>();
			_makeEnemyPirate2Button = contentpanel.GetChild(4).GetComponent<Button>();
			_makeEnemyPirate3Button = contentpanel.GetChild(5).GetComponent<Button>();


			_makePirate1Button.onClick.AddListener (OnMakePirate1ButtonClicked);
			_makePirate2Button.onClick.AddListener (OnMakePirate2ButtonClicked);
			_makePirate3Button.onClick.AddListener (OnMakePirate3ButtonClicked);
			
			_makeEnemyPirate1Button.onClick.AddListener(OnMakeEnemyPirate1ButtonClicked);
			_makeEnemyPirate2Button.onClick.AddListener(OnMakeEnemyPirate2ButtonClicked);
			_makeEnemyPirate3Button.onClick.AddListener(OnMakeEnemyPirate3ButtonClicked);



			_quitButton.onClick.AddListener (OnQuitButtonClicked);

			InitializeButtonList();

		}

		void OnQuitButtonClicked ()
		{
			_messager.Publish (new QuitGameMessage{
				
			});

		}

		void InitializeButtonList(){

			_buttonList = new List<Button>();
			
			_buttonList.Add(_makePirate1Button);
			_buttonList.Add(_makePirate2Button);
			_buttonList.Add(_makePirate3Button);
			_buttonList.Add(_makeEnemyPirate1Button);
			_buttonList.Add(_makeEnemyPirate2Button);
			_buttonList.Add(_makeEnemyPirate3Button);

		}
		void changeState(Button button, bool state){

			InitializeButtonList();

			if(_buttonList.Contains(button)){
				button.interactable = state;
			}

			_buttonList.Remove(button);

			foreach(Button b in _buttonList){
				b.interactable = !state;
			
			}

		}
		
		void OnMakePirate1ButtonClicked ()
		{
			//send message to play state to make pirates
			InputController.PirateName =  "Pirate1";
			//_makePirate1Button.interactable = false;

			changeState(_makePirate1Button, false);

		}
		
		void OnMakePirate2ButtonClicked ()
		{
			//send message to play state to teardown level
			InputController.PirateName =  "Pirate2";
			changeState(_makePirate2Button, false);
		}
		
		void OnMakePirate3ButtonClicked(){
			InputController.PirateName =  "Pirate3";
			changeState(_makePirate3Button, false);
		}
		
		void OnMakeEnemyPirate1ButtonClicked(){
			

			InputController.PirateName =  "EnemyPirate1";
			changeState(_makeEnemyPirate1Button, false);
		}
		
		void OnMakeEnemyPirate2ButtonClicked(){
			InputController.PirateName =  "EnemyPirate2";
			changeState(_makeEnemyPirate2Button, false);
		}
		
		void OnMakeEnemyPirate3ButtonClicked(){
			InputController.PirateName =  "EnemyPirate3";
			changeState(_makeEnemyPirate3Button, false);
			
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

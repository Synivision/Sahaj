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
	public class GamePlayCanvasController : BaseCanvasController {

		private readonly Messager _messager;
		private GameObject _parentButtonObject;
		private Button _previouslyClickedTileButton;

		private readonly Button _makePirate1Button;
		private readonly Button _makePirate2Button;
		private readonly Button _makePirate3Button;
		
		private readonly Button _makeEnemyPirate1Button;
		private readonly Button _makeEnemyPirate2Button;
		private readonly Button _makeEnemyPirate3Button;

		private readonly PrefabProvider _prefabProvider;

		private readonly Text _fpsText;
		private Canvas _canvas;
		private readonly Button _quitButton;
		
		private UnityReferenceMaster _unityReference;

		private List<Button> _buttonList;
		private PlayerManager _playerManager;

		private InputSession _inputSession;

		public GamePlayCanvasController (IoCResolver resolver, Canvas canvasView, PlayerManager playerManager) : base(canvasView)
		{
			_canvas = canvasView;
			resolver.Resolve (out _prefabProvider);

			resolver.Resolve (out _messager);
			resolver.Resolve (out _unityReference);
			resolver.Resolve (out _inputSession);

			ResolveElement (out _fpsText,"FpsText");
			ResolveElement(out _quitButton,"QuitButton");

			var panel = GetElement("Scroll");
			var contentpanel = panel.transform.GetChild(0);

			_parentButtonObject = contentpanel.gameObject;

			_quitButton.onClick.AddListener (OnQuitButtonClicked);
			_playerManager = playerManager;

			List<string> currentPirateName = new List<string>();
			_buttonList = new List<Button>();

			foreach(var item in _playerManager.Model.UnlockedPirates){

				if(item.Value == true){
					_buttonList.Add(CreatePirateButton(item.Key));
				}
			}
		}

		public Button CreatePirateButton(string name)
		{
			var fab = Object.Instantiate(_prefabProvider.GetPrefab("PirateButton")).gameObject.GetComponent<Button>();
			
			fab.gameObject.name = name;
			fab.onClick.AddListener(() => OnPirateButtonClicked(fab,name));
			
			fab.transform.SetParent(_parentButtonObject.transform);
			fab.transform.localScale = Vector3.one;
			return fab;
		}
		
		private void OnPirateButtonClicked(Button button,string name)
		{
			_inputSession.CurrentlySelectedPirateName = name;
			Debug.Log (name);
			if (_previouslyClickedTileButton == button) return;
			
			if (_previouslyClickedTileButton != null)
				_previouslyClickedTileButton.interactable = true;
			
			button.interactable = false;
			_previouslyClickedTileButton = button;
			
		}

		void OnQuitButtonClicked ()
		{
			_messager.Publish (new QuitGameMessage{
				
			});

		}

		void changeState(Button button, bool state){

			if(_buttonList.Contains(button)){
				button.interactable = state;
				_previouslyClickedTileButton = button;
			}

			_buttonList.Remove(button);

			foreach(Button b in _buttonList){
				b.interactable = !state;
			}

		}
		public override void TearDown ()
		{	
					
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

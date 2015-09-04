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


		private readonly PrefabProvider _prefabProvider;

		//player panels
		private Slider _experiencePointBar;
		private Slider _goldCoinBar;
		private Text _goldText;
		private Text _experienceLevelText;
		private Text _availableGoldText;

		//message tokens
		private MessagingToken _onUpdateCanvasPanels;
		private MessagingToken _onUpdatePirateButtonNumberLabel;
		private MessagingToken _PirateDictToken;

		private readonly Text _fpsText;
		private Canvas _canvas;
		private readonly Button _quitButton;
		private readonly Button _shipBaseButton;

		private IoCResolver _resolver;
		private UnityReferenceMaster _unityReference;

		private List<Button> _buttonList;
		private Dictionary<string, Text> _numberLabelDict;
		private PlayerManager _playerManager;

		private InputSession _inputSession;
		private UiManager _uiManager;
		private CanvasProvider _canvasProvider;
		private readonly MessagingToken _onWin;

		public GamePlayCanvasController (IoCResolver resolver, Canvas canvasView, PlayerManager playerManager) : base(canvasView)
		{
			_canvas = canvasView;
			_playerManager = playerManager;
			_resolver = resolver;
			resolver.Resolve (out _prefabProvider);
			resolver.Resolve (out _messager);
			resolver.Resolve (out _unityReference);
			resolver.Resolve (out _inputSession);
			_uiManager = new UiManager ();
			_resolver.Resolve(out _canvasProvider);

			ResolveElement (out _fpsText,"FpsText");
			ResolveElement(out _quitButton,"QuitButton");

			var panel = GetElement("Scroll");
			var contentpanel = panel.transform.GetChild(0);
			_parentButtonObject = contentpanel.gameObject;

			//initialize player ExperiencePanel
			var playerExperinecePanel = GetElement("ExperiencePanel");
			_experiencePointBar = playerExperinecePanel.transform.GetChild(0).GetComponent<Slider>();
			_experienceLevelText = playerExperinecePanel.transform.GetChild(2).GetComponent<Text>();

			//TODO get max value from player manager
			_experiencePointBar.maxValue = 200;
			_experiencePointBar.value = 0;

			//initialize player GoldPanel
			var playerGoldPanel = GetElement("GoldPanel");
			_goldCoinBar = playerGoldPanel.transform.GetChild(0).GetComponent<Slider>();
			_goldText = playerGoldPanel.transform.GetChild(2).GetComponent<Text>();

			//TODO get max value from player manager
			_goldCoinBar.maxValue = _playerManager.Model.MaxGoldCapacity;
			_goldCoinBar.value = 0;

			// initialize availableLootPanel contents
			var availableLootPanel = GetElement("AvailableLootPanel");
			_availableGoldText =  availableLootPanel.transform.GetChild(2).GetComponent<Text>();
			 
			//change state button
			ResolveElement (out _shipBaseButton, "_shipBaseButton");
			_shipBaseButton.onClick.AddListener(ChangeStateToShipBase);

			//subsscribe messages
			_onUpdateCanvasPanels = _messager.Subscribe<UpdateGamePlayUiMessage> (UpdateCanvasPanels);
			InitializeCanvasPanels(_playerManager);
			_onUpdatePirateButtonNumberLabel = _messager.Subscribe<UpdatePirateNumber>(UpdatePirateButtonNumberLabel);
			_quitButton.onClick.AddListener (OnQuitButtonClicked);
			_buttonList = new List<Button>();
			_numberLabelDict = new Dictionary<string, Text>();

			foreach(var item in _playerManager.Model.UnlockedPirates){

				if(item.Value == true){
					_buttonList.Add(CreatePirateButton(item.Key));
				}
			}

			_onWin = _messager.Subscribe<WinMessage> (OnWin);

			InitializePirateButtonNumberLabel();

		}


		public void ChangeStateToShipBase(){

			//send message to playstate to switch state
			_messager.Publish(new PlayStateToShipBaseMessage{});
		}

		public void InitializeCanvasPanels(PlayerManager playerManager){

			_experiencePointBar.value = playerManager.Model.ExperiencePoints;
			_experienceLevelText.text = playerManager.Model.UserLevel.ToString(); 
			_goldCoinBar.value = playerManager.Model.Gold;
			_goldText.text = playerManager.Model.Gold.ToString();

		}

		private  void OnWin (WinMessage message)
		{
			_uiManager.RegisterUi(new WinCanvasController(_resolver, _canvasProvider.GetCanvas("WinCanvas")));
			TearDown();
			
		}

		public void UpdatePirateButtonNumberLabel(UpdatePirateNumber message){


			var text = _numberLabelDict[message.PirateName];
			text.text = message.PirateNumber.ToString();

		}

		public void InitializePirateButtonNumberLabel(){

			foreach(KeyValuePair<string, int> entry in _playerManager.Model.PirateCountDict){

				if(_playerManager.Model.UnlockedPirates.ContainsKey(entry.Key)&&_playerManager.Model.UnlockedPirates[entry.Key] == true){

					var text = _numberLabelDict[entry.Key];
					text.text =  entry.Value.ToString();
				}
			}

		}
		public void UpdateCanvasPanels(UpdateGamePlayUiMessage message){

			//TODO Increase level of player
			_experiencePointBar.value += message.ExperiencePoints;  
			//_experienceLevelText 
			if(message.availableGold > 0){
				_goldCoinBar.value += message.Gold;
				_goldText.text = _goldCoinBar.value.ToString();
			}

			_availableGoldText.text = message.availableGold.ToString();
		}

		public Button CreatePirateButton(string name)
		{
			var fab = Object.Instantiate(_prefabProvider.GetPrefab("pirate_button")).gameObject.GetComponent<Button>();
			
			fab.gameObject.name = name;

			var buttonLabel = fab.transform.GetChild (0).GetComponent<Text>();
			buttonLabel.text = name;
			var buttonNumberLabel = fab.transform.GetChild (1).GetComponent<Text>();

			_numberLabelDict.Add(name,buttonNumberLabel);

			fab.onClick.AddListener(() => OnPirateButtonClicked(fab,name));
			
			fab.transform.SetParent(_parentButtonObject.transform);
			//fab.transform.localScale = Vector3.one;
			return fab;
		}
		
		private void OnPirateButtonClicked(Button button,string name)
		{
			_inputSession.CurrentlySelectedPirateName = name;
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

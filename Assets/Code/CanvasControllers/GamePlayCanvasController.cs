using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;
using Assets.Code.UnityBehaviours;
using Assets.Code.States;

namespace Assets.Code.Ui.CanvasControllers
{
    public class GamePlayCanvasController : BaseCanvasController
    {

        private readonly Messager _messager;
        private GameObject _parentButtonObject;
        private GameObject _parentShipAttackButtonObject;
        private Button _previouslyClickedTileButton;
        private Text _shipAttacksLabel;


        private readonly PrefabProvider _prefabProvider;

        //player panels
        private Slider _experiencePointBar;
        private Slider _goldCoinBar;
        private Text _goldText;
        private Text _experienceLevelText;
        private Text _availableGoldText;

        //message tokens
        private MessagingToken _onUpdateCanvasPanels;
        //private MessagingToken _onUpdatePirateButtonNumberLabel;
        private MessagingToken _onUpdateRowBoatButtonNumberLabel;
        private MessagingToken _PirateDictToken;
        private MessagingToken _onUpdateCurrentShipBulletsMessage;
        private MessagingToken _onRowBoatSentToAttackMessageToken;

        private readonly Text _fpsText;
        private Canvas _canvas;
        private readonly Button _quitButton;
        private readonly Button _shipBaseButton;
        //ship attack buttons


        private IoCResolver _resolver;
        private UnityReferenceMaster _unityReference;

        private List<Button> _buttonList;
        private List<Button> _shipAttackButtonList;
        private Dictionary<string, Text> _numberLabelDict;
        private PlayerManager _playerManager;
        private Text _shipGunPowderLabel;
        private Text _rowBoatsRemainingLabel;
        private InputSession _inputSession;
        private UiManager _uiManager;
        private CanvasProvider _canvasProvider;
       

        private SpriteProvider _spriteProvider;
        private int rowBoatsRemainingCount = 0;
        public GamePlayCanvasController(IoCResolver resolver, Canvas canvasView, PlayerManager playerManager)
            : base(resolver, canvasView)
        {
            _canvas = canvasView;
            _playerManager = playerManager;
            _resolver = resolver;
            _resolver.Resolve(out _prefabProvider);
            _resolver.Resolve(out _messager);
            _resolver.Resolve(out _unityReference);
            _resolver.Resolve(out _inputSession);
            _resolver.Resolve(out _spriteProvider);


            _uiManager = new UiManager();
            _resolver.Resolve(out _canvasProvider);

            ResolveElement(out _fpsText, "FpsText");
            ResolveElement(out _quitButton, "QuitButton");
            ResolveElement(out _shipGunPowderLabel, "NoTouchZone/GunPowderText"); 
            ResolveElement(out _rowBoatsRemainingLabel, "NoTouchZone/RowBoatCountText");

            var shipAttackspanel = GetElement("ShipAttacksPanel");
            var shipAttackContentPanel = shipAttackspanel.transform.GetChild(0).gameObject;
            _shipAttacksLabel = shipAttackspanel.transform.GetChild(1).GetComponent<Text>();
            _parentShipAttackButtonObject = shipAttackContentPanel;

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
            _availableGoldText = availableLootPanel.transform.GetChild(2).GetComponent<Text>();

            //change state button
            ResolveElement(out _shipBaseButton, "_shipBaseButton");
            _shipBaseButton.onClick.AddListener(ChangeStateToShipBase);

            //subsscribe messages
            _onUpdateCanvasPanels = _messager.Subscribe<UpdateGamePlayUiMessage>(UpdateCanvasPanels);
            InitializeCanvasPanels(_playerManager);
            //_onUpdatePirateButtonNumberLabel = _messager.Subscribe<UpdatePirateNumber>(UpdatePirateButtonNumberLabel);
            _onUpdateRowBoatButtonNumberLabel = _messager.Subscribe<UpdateRowBoatPirateNumberMessage>(UpdateRowBoatButtonNumberLabel);
            _onRowBoatSentToAttackMessageToken = _messager.Subscribe<RowBoatSentToAttackMessage>(OnRowBoatSentToAttack);
            _onUpdateCurrentShipBulletsMessage = _messager.Subscribe<UpdateCurrentShipBulletsMessage>(UpdateShipAttackLabel);
            _quitButton.onClick.AddListener(OnQuitButtonClicked);
            _buttonList = new List<Button>();
            _shipAttackButtonList = new List<Button>();

            _numberLabelDict = new Dictionary<string, Text>();


            //add boats to main list not pirates

            foreach (var item in _playerManager.Model.RowBoatCountDict)
            { 
              //_buttonList.Add(CreateRowBoatButton(item.Key));
                rowBoatsRemainingCount++;
            }
           
            _buttonList.Add(CreateRowBoatButton("Boat" + rowBoatsRemainingCount.ToString()));

           
            foreach (var item in _playerManager.Model.UnlockedShipAttacks)
            {

                if (item.Value == true)
                {
                    _shipAttackButtonList.Add(CreateShipAttackButton(item.Key));

                }
            }

            HandleShipButtonVisibility();

            
            _shipGunPowderLabel.text = "Ship GunPowder Remaining : " + _playerManager.Model.ShipBulletsAvailable.ToString();
            _rowBoatsRemainingLabel.text = "RowBoat Remaining : " + rowBoatsRemainingCount.ToString();
        }

        public void OnRowBoatSentToAttack(RowBoatSentToAttackMessage message) {

            rowBoatsRemainingCount--;
            _rowBoatsRemainingLabel.text = "RowBoat Remaining : " + rowBoatsRemainingCount.ToString();
            foreach (var button in _buttonList)
                {
                 button.gameObject.SetActive(false);
                }

            if (rowBoatsRemainingCount>0 && rowBoatsRemainingCount<=4) {
                _buttonList.Add(CreateRowBoatButton("Boat" + rowBoatsRemainingCount.ToString()));
            }
            
        }

        public void UpdateRowBoatButtonNumberLabel(UpdateRowBoatPirateNumberMessage message)
        {
            var text = _numberLabelDict[message.BoatName];
            text.text = message.PirateNumber.ToString();

        }

        private void UpdateShipAttackLabel(UpdateCurrentShipBulletsMessage message){

            _shipGunPowderLabel.text = "Ship GunPowder Remaining : " + _playerManager.Model.ShipBulletsAvailable.ToString();
            //TODO check what buttons can be used for attack by ship

            HandleShipButtonVisibility();

        }

        public Button CreateRowBoatButton(string name)
        {
           
            var fab = Object.Instantiate(_prefabProvider.GetPrefab("pirate_button")).gameObject.GetComponent<Button>();

            fab.gameObject.name = name;

            fab.name = name;

            fab.GetComponent<Image>().sprite = _spriteProvider.GetSprite("row_boat_mini");

            var buttonLabel = fab.transform.GetChild(0).GetComponent<Text>();
            buttonLabel.text = name;
            var buttonNumberLabel = fab.transform.GetChild(1).GetComponent<Text>();
            buttonNumberLabel.text = calculateOccupiedSeatsOfRowBoat(name).ToString();

           _numberLabelDict.Add(name, buttonNumberLabel);

            fab.onClick.AddListener(() => OnRowBoatButtonClicked(fab, name));
            fab.transform.SetParent(_parentButtonObject.transform);
            fab.transform.SetAsFirstSibling();

            return fab;
        }

        private int calculateOccupiedSeatsOfRowBoat(string rowBoatName) {

            Dictionary<int, string> seatsDictionary;
            _playerManager.Model.RowBoatCountDict.TryGetValue(rowBoatName, out seatsDictionary);
            int count = 0;

            foreach (var seat in seatsDictionary) {

                count++;
                if (seat.Value.Equals("")) {
                    count--;
                }

            }
            return count;
        }

        private void OnRowBoatButtonClicked(Button button, string name)
        {
            //_inputSession.CurrentlySelectedRowBoatName = name;
            if (_previouslyClickedTileButton == button)
            {
      
            }

            if (_previouslyClickedTileButton != null)
                _previouslyClickedTileButton.interactable = true;

            _previouslyClickedTileButton = button;
            HandleShipButtonVisibility();

            _messager.Publish(new RowBoatSelectedMessage {
                BoatName = name,
                onCancelled = () => { // Debug.Log("Hello"); 
                }

        });
        }
        public void HandleShipButtonVisibility() {

            foreach (var button in _shipAttackButtonList)
            {

                string name = button.gameObject.name;
                int attackCost;
                _playerManager.Model.ShipAttackCostDict.TryGetValue(name, out attackCost);

                if (attackCost > _playerManager.Model.ShipBulletsAvailable)
                {
                    button.interactable = false;
                }
                else if (button != _previouslyClickedTileButton)
                {
                    button.interactable = true;
                }

            }

        }

        public void ChangeStateToShipBase()
        {

            //send message to playstate to switch state
           
            _messager.Publish(new OpenShipBaseMessage {

            });
             TearDown();
            
        }

        public void InitializeCanvasPanels(PlayerManager playerManager)
        {

            _experiencePointBar.value = playerManager.Model.ExperiencePoints;
            _experienceLevelText.text = playerManager.Model.UserLevel.ToString();
            _goldCoinBar.value = playerManager.Model.Gold;
            _goldText.text = playerManager.Model.Gold.ToString();

        }


        public void UpdateCanvasPanels(UpdateGamePlayUiMessage message)
        {

            //TODO Increase level of player
            _experiencePointBar.value += message.ExperiencePoints;
            //_experienceLevelText 
            if (message.availableGold > 0)
            {
                _goldCoinBar.value += message.Gold;
                _goldText.text = _goldCoinBar.value.ToString();
            }

            _availableGoldText.text = message.availableGold.ToString();
        }

        public Button CreatePirateButton(string name)
        {
            var fab = Object.Instantiate(_prefabProvider.GetPrefab("pirate_button")).gameObject.GetComponent<Button>();

            fab.gameObject.name = name;

            var buttonLabel = fab.transform.GetChild(0).GetComponent<Text>();
            buttonLabel.text = name;
            var buttonNumberLabel = fab.transform.GetChild(1).GetComponent<Text>();

            _numberLabelDict.Add(name, buttonNumberLabel);

            fab.onClick.AddListener(() => OnPirateButtonClicked(fab, name));

            fab.transform.SetParent(_parentButtonObject.transform);

            //fab.transform.localScale = Vector3.one;
            return fab;
        }

        public Button CreateShipAttackButton(string name) {

            var fab = Object.Instantiate(_prefabProvider.GetPrefab("pirate_button").gameObject.GetComponent<Button>());
            fab.gameObject.name = name;
            int value;
            _playerManager.Model.ShipAttackCostDict.TryGetValue(name, out value);
            fab.transform.GetChild(0).GetComponent<Text>().text = value.ToString();
            fab.GetComponent<Image>().sprite = _spriteProvider.GetSprite(name);
            //var buttonLabel = fab.transform.GetChild(0).GetComponent<Text>();
            //buttonLabel.text = name;
            //var buttonNumberLabel = fab.transform.GetChild(1).GetComponent<Text>();
            var buttonNumberLabel = fab.transform.GetChild(1).GetComponent<Text>();
            buttonNumberLabel.text = "";
            fab.onClick.AddListener(() => OnShipAttackButtonClicked(fab, name));

            fab.transform.SetParent(_parentButtonObject.transform);

            return fab;
        }

        private void OnPirateButtonClicked(Button button, string name)
        {
            _inputSession.CurrentlySelectedPirateName = name;
            if (_previouslyClickedTileButton == button) {
               // return;
            }

            if (_previouslyClickedTileButton != null)
                _previouslyClickedTileButton.interactable = true;

            button.interactable = false;
            _previouslyClickedTileButton = button;
            HandleShipButtonVisibility();

        }

        private void OnShipAttackButtonClicked(Button button, string name)
        {
            _inputSession.CurrentlySelectedShipAttackName = name;
            _inputSession.CurrentlySelectedRowBoatName = null;

            int value;
            _playerManager.Model.ShipAttackCostDict.TryGetValue(name, out value);
            _inputSession.CurrentShipAttackCost = value;

            if (_previouslyClickedTileButton == button) return;

            if (_previouslyClickedTileButton != null)
                _previouslyClickedTileButton.interactable = true;

            button.interactable = false;
            _previouslyClickedTileButton = button;
            HandleShipButtonVisibility();

        }

        void OnQuitButtonClicked()
        {
            _messager.Publish(new QuitGameMessage
            {

            });

        }

       
        public override void TearDown()
        {
            foreach (var button in _buttonList)
            {

                button.onClick.RemoveAllListeners();
               
                GameObject.Destroy(button.gameObject);

            }

            foreach (var button in _shipAttackButtonList) {

                button.onClick.RemoveAllListeners();
                
                GameObject.Destroy(button.gameObject);
            }
            _messager.CancelSubscription (_onUpdateCanvasPanels,_onUpdateRowBoatButtonNumberLabel,
                _onUpdateCurrentShipBulletsMessage, _onRowBoatSentToAttackMessageToken);
            _buttonList.Clear();
            _quitButton.onClick.RemoveAllListeners();
            _shipBaseButton.onClick.RemoveAllListeners();
            base.TearDown();
        }

        public override void Update()
        {
            base.Update();

            var frameRate = (int)(1.0 / Time.deltaTime);
            _fpsText.text = frameRate.ToString() + " fps";

        }

    }

}

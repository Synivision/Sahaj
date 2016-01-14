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
namespace Assets.Code.Ui.CanvasControllers{

    public class InventoryCanvasController : BaseCanvasController {
        
        private readonly Messager _messager;
        private readonly PrefabProvider _prefabProvider;
        private IoCResolver _resolver;
        private UiManager _uiManager;
        private CanvasProvider _canvasProvider;
        private Button _closeButton;
        private Button _addPirateButton;
        private MessagingToken _onFindInventoryItem;
        private PlayerManager _playerManager;
        private GameObject _mainPanel;
        private GameObject _pirateImage;
        private GameObject _pirateButtonScrollPanel;
        List<Button> _pirateButtonList;
        private InputSession _inputSession;
        private Button _previouslyClickedTileButton;
        private SpriteProvider _spriteProvider;
        private GameObject _pirateDetailsPanel;
        private GameDataProvider _gameDataProvider;

        private string _rowBoatName;
       

        public InventoryCanvasController(IoCResolver resolver, Canvas canvasView)
            : base(resolver, canvasView)
        {
            
            _resolver = resolver;
            _resolver.Resolve (out _messager);
            _resolver.Resolve (out _canvasProvider);
            _resolver.Resolve(out _inputSession);
            _resolver.Resolve(out _spriteProvider);
            _resolver.Resolve(out _playerManager);
            _resolver.Resolve(out _prefabProvider);
            _resolver.Resolve(out _gameDataProvider);

            _canvasView.gameObject.SetActive (true);
            _mainPanel = GetElement("MainPanel");
            _uiManager = new UiManager();

            // subscriptions
            //_onFindInventoryItem = _messager.Subscribe<FindInventoryItemMessage>(OnFindInventoryItem);
            _closeButton = _mainPanel.transform.GetChild(0).GetComponent<Button>();
            _closeButton.onClick.AddListener(onCloseClicked);

            _pirateImage = _mainPanel.transform.GetChild(1).gameObject;

            _pirateButtonScrollPanel = GetElement("MainPanel/MainChoosePiratePanel/ScrollPiratePanel/Content");
            _pirateDetailsPanel = GetElement("MainPanel/PirateDetailsPanel");

            _addPirateButton = _mainPanel.transform.GetChild(2).GetComponent<Button>();
            _addPirateButton.onClick.AddListener(onAddPirateToRowBoatClicked);

            //add buttons to scrollPanel according to unlocked pirates from playermodel
            _pirateButtonList = new List<Button>();

            if (_playerManager.Model !=null) {

                foreach (var pirate in _playerManager.Model.UnlockedPirates)
                {
                    //if unlocked then add to list
                    if (pirate.Value) {

                        _pirateButtonList.Add(CreatePirateButton(pirate.Key));
                    }
                }
            }
        }

        public InventoryCanvasController(IoCResolver resolver, Canvas canvasView, string rowBoatName)
           : base(resolver, canvasView)
        {

            _resolver = resolver;
            _resolver.Resolve(out _messager);
            _resolver.Resolve(out _canvasProvider);
            _resolver.Resolve(out _inputSession);
            _resolver.Resolve(out _spriteProvider);
            _resolver.Resolve(out _playerManager);
            _resolver.Resolve(out _prefabProvider);
            _resolver.Resolve(out _gameDataProvider);
            
           

            _canvasView.gameObject.SetActive(true);
            _mainPanel = GetElement("MainPanel");
            _rowBoatName = rowBoatName;

            // subscriptions
            //_onFindInventoryItem = _messager.Subscribe<FindInventoryItemMessage>(OnFindInventoryItem);
            _closeButton = _mainPanel.transform.GetChild(0).GetComponent<Button>();
            _closeButton.onClick.AddListener(onCloseClicked);

            _pirateImage = _mainPanel.transform.GetChild(1).gameObject;

            _pirateButtonScrollPanel = GetElement("MainPanel/MainChoosePiratePanel/ScrollPiratePanel/Content");
            _pirateDetailsPanel = GetElement("MainPanel/PirateDetailsPanel");

            _addPirateButton = _mainPanel.transform.GetChild(2).GetComponent<Button>();
            _addPirateButton.onClick.AddListener(onAddPirateToRowBoatClicked);

            //add buttons to scrollPanel according to unlocked pirates from playermodel
            _pirateButtonList = new List<Button>();

            if (_playerManager.Model != null)
            {

                foreach (var pirate in _playerManager.Model.UnlockedPirates)
                {
                    //if unlocked then add to list
                    if (pirate.Value)
                    {

                        _pirateButtonList.Add(CreatePirateButton(pirate.Key));
                    }
                }
            }
        }

        void onAddPirateToRowBoatClicked() {
            //canvas opened from rowboat add pirate option

            if (_rowBoatName != null)
            {

                Dictionary<int, string> seatsDictionary;
                _playerManager.Model.RowBoatCountDict.TryGetValue(_rowBoatName, out seatsDictionary);

                foreach (var seat in seatsDictionary)
                {

                    if (seat.Value.Equals(""))
                    {
                        seatsDictionary[seat.Key] = _inputSession.CurrentlySelectedPirateName;
                        break;
                    }

                }

                _messager.Publish(new UpdateRowBoatPirateNumberMessage {

                    BoatName = _rowBoatName,
                    PirateNumber = calculateOccupiedSeatsOfRowBoat(seatsDictionary)

                });


                TearDown();

            }
            else {

                //TODO this canvas is opened from building action button 
                //so display something so that the user can choose rowboat
                //open select rowboat canvas controller

                _uiManager.RegisterUi(new SelectRowBoatCanvasController(_resolver, _canvasProvider.GetCanvas("SelectRowBoatCanvas")));

            }
            
        }

        private int calculateOccupiedSeatsOfRowBoat(Dictionary<int, string> seatsDictionary)
        {
            int count = 0;

            foreach (var seat in seatsDictionary)
            {

                count++;
                if (seat.Value.Equals(""))
                {
                    count--;
                }

            }
            return count;
        }

        void onCloseClicked(){

            TearDown();
        }
        
        public void OnFindInventoryItem(FindInventoryItemMessage message){
            
            
            
        }

        public Button CreatePirateButton(string name)
        {
           
            var fab = Object.Instantiate(_prefabProvider.GetPrefab("pirate_button")).gameObject.GetComponent<Button>();

            fab.gameObject.name = name;

            fab.GetComponent<Image>().sprite = _spriteProvider.GetSprite(name);

            var buttonLabel = fab.transform.GetChild(0).GetComponent<Text>();
            buttonLabel.text = name;

            fab.onClick.AddListener(() => OnPirateButtonClicked(fab, name));

            fab.transform.SetParent(_pirateButtonScrollPanel.transform);

            //fab.transform.localScale = Vector3.one;
            return fab;
        }

        private void OnPirateButtonClicked(Button button, string name)
        {


            _pirateImage.GetComponent<Image>().sprite = _spriteProvider.GetSprite(name);
            _inputSession.CurrentlySelectedPirateName = name;
            if (_previouslyClickedTileButton == button) return;

            if (_previouslyClickedTileButton != null)
                _previouslyClickedTileButton.interactable = true;

            button.interactable = false;
            _previouslyClickedTileButton = button;

            //TODO update details panel
            var healthSlider = GetElement("MainPanel/PirateDetailsPanel/HealthSlider").GetComponent<Slider>();
            var healthValueText = GetElement("MainPanel/PirateDetailsPanel/HealthValue").GetComponent<Text>();
            var costSlider = GetElement("MainPanel/PirateDetailsPanel/CostSlider").GetComponent<Slider>();
            var costValueText = GetElement("MainPanel/PirateDetailsPanel/CostValue").GetComponent<Text>();
            var powerSlider = GetElement("MainPanel/PirateDetailsPanel/PowerSlider").GetComponent<Slider>();
            var powerValueText = GetElement("MainPanel/PirateDetailsPanel/PowerValue").GetComponent<Text>();

            healthSlider.maxValue = 300;
            costSlider.maxValue = 3000;
            powerSlider.maxValue = 50;
            //get pirate details  
            PirateModel pirateModel =  _gameDataProvider.GetData<PirateModel>(name);
            StatBlock stats = pirateModel.Stats;

            healthSlider.value = stats.MaximumHealth;
            healthValueText.text = stats.MaximumHealth.ToString();
            costSlider.value = pirateModel.TrainingCost;
            costValueText.text = pirateModel.TrainingCost.ToString();
            powerSlider.value = stats.MaximumDamage;
            powerValueText.text = stats.MaximumDamage.ToString();

        }

        public override void TearDown()
        {
            foreach (var button in _pirateButtonList)
            {

                button.onClick.RemoveAllListeners();
                //button.gameObject.SetActive(false);
                GameObject.Destroy(button.gameObject);
            }
            _pirateButtonList = new List<Button>();
            _closeButton.onClick.RemoveAllListeners();
            _addPirateButton.onClick.RemoveAllListeners();
            base.TearDown();

        }

    }
}

using UnityEngine;
using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Code.Messaging;
using Assets.Code.Messaging.Messages;

namespace Assets.Code.Ui.CanvasControllers
{
    public class NewCreatePirateCanvasController : BaseCanvasController
    {
        //private utility object
        private IoCResolver _resolver;
        private GameDataProvider _gameDataProvider;
        private PrefabProvider _prefabProvider;
        private SpriteProvider _spriteProvider;
        private PirateGenerator _pirateGenerator;
        private readonly Messager _messager;

        //Other variables
        private List<PirateModel> _listOfAllPiratesAvailable;
        private List<Button> _listOfPirateButtonsToBeDeleted;
        private List<string> _listOfPiratesBiengGenerated;
        private MessagingToken _newPirateGeneratedMessage;

        public int PlatoonBuildingLevel
        {
            get;set;
        }

        public string BuildingName
        {
            get; set;
        }

        //ui element variables
        private Button _quitButton;
        private Button _leftButton;
        private Button _rightButton;
        private Text _timeRemainingLabel;
        private GameObject _mainPanel;
        private GameObject _buttonPanel;
        private GameObject _pirateDisplayImagePanel;
        

        public NewCreatePirateCanvasController(IoCResolver resolver, Canvas canvasView)
            : base(resolver, canvasView)
		{
            _resolver = resolver;
            _resolver.Resolve(out _gameDataProvider);
            _resolver.Resolve(out _prefabProvider);
            _resolver.Resolve(out _spriteProvider);
            _resolver.Resolve(out _pirateGenerator);
            _resolver.Resolve(out _messager);
        }

        public void Initialize()
        {
            _listOfAllPiratesAvailable = _gameDataProvider.GetAllData<PirateModel>();
            _listOfPiratesBiengGenerated = new List<string>();

            InitializeUIElements();

            AddPiratesAllowedToBeGeneratedByBuildingToButtonsPanel();

            _newPirateGeneratedMessage = _messager.Subscribe<NewPirateGeneratedMessage>(RemovePirate);

            AddPiratesInQueueToPiratesBiengGeneratedPanel();
        }

        private void AddPiratesInQueueToPiratesBiengGeneratedPanel()
        {
            var piratesBiengGeneratedByPirateGenerator = _pirateGenerator.PiratesBiengGeneratedForBuilding(BuildingName);

            foreach (var pirateName in piratesBiengGeneratedByPirateGenerator)
            {
                if (_listOfPiratesBiengGenerated.Contains(pirateName))
                {
                    var pirateBiengGeneratedButtonForCurrentPirate = _pirateDisplayImagePanel.transform.FindChild("Pirate " + pirateName);
                    var numberOfPiratesOfCurrentTypeInQueueLabel = pirateBiengGeneratedButtonForCurrentPirate.GetChild(0).GetComponent<Text>();
                    int numberOfPiratesOfThisType = System.Int32.Parse(numberOfPiratesOfCurrentTypeInQueueLabel.text);
                    numberOfPiratesOfCurrentTypeInQueueLabel.text = (numberOfPiratesOfThisType + 1).ToString() + "";
                }
                else
                {
                    //Add pirate to second scroll panel
                    var fab = Object.Instantiate(_prefabProvider.GetPrefab("pirate_bieng_generated")).gameObject.GetComponent<Button>();
                    fab.transform.SetParent(_pirateDisplayImagePanel.transform);
                    fab.gameObject.name = "Pirate " + pirateName;
                    var pirateImage = _spriteProvider.GetSprite(pirateName + "_Render");
                    fab.image.overrideSprite = pirateImage;
                    _listOfPiratesBiengGenerated.Add(pirateName);
                }
            }


        }

        public override void Update()
        {

            base.Update();
            _timeRemainingLabel.text = _pirateGenerator.GetTimeToCompletionOfPirate(BuildingName).ToString().Substring(0,8);
        }

        private void InitializeUIElements()
        {
            ResolveElement(out _quitButton, "Main Panel/quit");
            ResolveElement(out _leftButton, "Main Panel/Left Button");
            ResolveElement(out _rightButton, "Main Panel/Right Button");
            ResolveElement(out _timeRemainingLabel, "Main Panel/Time Remaining");

            _mainPanel = GetElement("Main Panel") as GameObject;
            _buttonPanel = GetElement("Main Panel/Button Scroll View/Panel") as GameObject;
            _pirateDisplayImagePanel = GetElement("Main Panel/Pirate Images/Panel") as GameObject;

            _quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void AddPiratesAllowedToBeGeneratedByBuildingToButtonsPanel()
        {
            Debug.Log("Platoon Building Level : " + PlatoonBuildingLevel.ToString());

            for (int i = 0; i < PlatoonBuildingLevel; i++)
            {
                var fab = Object.Instantiate(_prefabProvider.GetPrefab("create_pirate_button")).gameObject.GetComponent<Button>() as Button;

                var pirateImage = _spriteProvider.GetSprite(_listOfAllPiratesAvailable[i].Name);

                fab.image.overrideSprite = pirateImage;

                fab.gameObject.name = "pirate choice " + i.ToString();

                fab.name = "pirate choice " + i.ToString();

                var buttonLabel = fab.transform.GetChild(0).GetComponent<Text>();
                buttonLabel.text = _listOfAllPiratesAvailable[i].Name;

                var buttonNumberLabel = fab.transform.GetChild(2).GetComponent<Text>();

                buttonNumberLabel.text = _listOfAllPiratesAvailable[i].TrainingCost.ToString();

                fab.transform.SetParent(_buttonPanel.transform);

                fab.transform.localScale = new Vector3(0.7f,0.7f,0.7f);

                AddListenerToButton(ref fab, _listOfAllPiratesAvailable[i].Name);
            }
        }

        private void AddListenerToButton(ref Button button,string pirateName)
        {
            button.onClick.AddListener(() => OnPirateButtonClicked(pirateName));
        }

        private void OnPirateButtonClicked(string pirateName)
        {
            if (_listOfPiratesBiengGenerated.Contains(pirateName))
            {
                var pirateBiengGeneratedButtonForCurrentPirate = _pirateDisplayImagePanel.transform.FindChild("Pirate " + pirateName);
                var numberOfPiratesOfCurrentTypeInQueueLabel = pirateBiengGeneratedButtonForCurrentPirate.GetChild(0).GetComponent<Text>();
                int numberOfPiratesOfThisType = System.Int32.Parse(numberOfPiratesOfCurrentTypeInQueueLabel.text);
                numberOfPiratesOfCurrentTypeInQueueLabel.text = (numberOfPiratesOfThisType + 1).ToString() + "" ;
            }
            else
            {
                //Add pirate to second scroll panel
                var fab = Object.Instantiate(_prefabProvider.GetPrefab("pirate_bieng_generated")).gameObject.GetComponent<Button>();
                fab.transform.SetParent(_pirateDisplayImagePanel.transform);
                fab.gameObject.name = "Pirate " + pirateName;
                var pirateImage = _spriteProvider.GetSprite(pirateName + "_Render");
                fab.image.overrideSprite = pirateImage;
                _listOfPiratesBiengGenerated.Add(pirateName);
            }

            var pirateModel = _listOfAllPiratesAvailable.Find(temp => temp.Name == pirateName);

            Debug.Log(pirateModel.PirateName + " : " + pirateModel.TrainingTime);

            //TODO: Start Generation of Pirate
            _pirateGenerator.GeneratePirate(pirateModel, BuildingName, System.TimeSpan.FromSeconds((double)Time.time));

        }

        public void RemovePirate(NewPirateGeneratedMessage message)
        {
            var pirateToBeRemoved = _listOfAllPiratesAvailable.Find(temp => temp.Name == message.PirateModel.Name);

            Debug.Log("Pirate " + message.PirateModel.Name + " generated");
            
            foreach (var pirateButton in _pirateDisplayImagePanel.GetComponentsInChildren<Button>(true))
            {
                if (pirateButton.name.Equals("Pirate " + message.PirateModel.Name))
                {
                    var numberOfPiratesOfCurrentTypeInQueueLabel = pirateButton.transform.GetChild(0).GetComponent<Text>();
                    int numberOfPiratesOfThisType = System.Int32.Parse(numberOfPiratesOfCurrentTypeInQueueLabel.text);

                    if (numberOfPiratesOfThisType > 1)
                    {
                        numberOfPiratesOfCurrentTypeInQueueLabel.text = (numberOfPiratesOfThisType - 1).ToString();
                    }
                    else
                    {
                        GameObject.Destroy(pirateButton.gameObject);
                        _listOfPiratesBiengGenerated.Remove(message.PirateModel.Name);
                    }
                    
                }
            }

        }

        public override void TearDown()
        {
            foreach (var pirateButton in _buttonPanel.GetComponentsInChildren<Button>(true))
            {
                pirateButton.onClick.RemoveAllListeners();
                Debug.Log("Removed listener from " + pirateButton.name);
            }

            foreach (var pirateButton in _pirateDisplayImagePanel.GetComponentsInChildren<Button>(true))
            {
                pirateButton.onClick.RemoveAllListeners();
                Debug.Log("Removed listener from " + pirateButton.name);
            }

            _quitButton.onClick.RemoveAllListeners();

            base.TearDown();
        }

        public void enableCanvas()
        {
            _canvasView.enabled = true;
        }

        public void ReInitialize()
        {
            _listOfPirateButtonsToBeDeleted = new List<Button>();

            foreach (var pirateButton in _buttonPanel.GetComponentsInChildren<Button>(true))
            {
                if (pirateButton.name.StartsWith("pirate choice")) {
                    pirateButton.onClick.RemoveAllListeners();
                    Debug.Log("Removed listener from " + pirateButton.name);
                    _listOfPirateButtonsToBeDeleted.Add(pirateButton);
                }
            }

            foreach (var pirateButton in _pirateDisplayImagePanel.GetComponentsInChildren<Button>(true))
            {
                if (pirateButton.name.StartsWith("Pirate"))
                {
                    pirateButton.onClick.RemoveAllListeners();
                    Debug.Log("Removed listener from " + pirateButton.name);
                    _listOfPirateButtonsToBeDeleted.Add(pirateButton);
                }
            }

            _buttonPanel.transform.DetachChildren();
            _pirateDisplayImagePanel.transform.DetachChildren();

            foreach (var button in _listOfPirateButtonsToBeDeleted)
            {
                Debug.Log(button.name + " at location " + button.transform.position.x + " , " + button.transform.position.y);
                GameObject.Destroy(button.gameObject);
            }

            _quitButton.onClick.RemoveAllListeners();

            Initialize();
        }

        private void OnQuitButtonClicked()
        {
            _canvasView.enabled = false;
        }
    }
}
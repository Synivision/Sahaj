using UnityEngine;
using System.Collections;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Code.Ui.CanvasControllers
{
    public class NewCreatePirateCanvasController : BaseCanvasController
    {
        //private utility object
        private IoCResolver _resolver;
        private GameDataProvider _gameDataProvider;
        private PrefabProvider _prefabProvider;

        //GameDataProvider variables
        private List<PirateModel> _listOfAllPiratesAvailable;

        public int PlatoonBuildingLevel
        {
            get;set;
        }

        //ui element variables
        private Button _quitButton;
        private Button _leftButton;
        private Button _rightButton;
        private Text _timeRemainingLabel;
        private GameObject _mainPanel;
        private GameObject _buttonPanel;
        private GameObject _pirateDisplayImage;

        public NewCreatePirateCanvasController(IoCResolver resolver, Canvas canvasView)
            : base(resolver, canvasView)
		{
            _resolver = resolver;
            _resolver.Resolve(out _gameDataProvider);
            _resolver.Resolve(out _prefabProvider);
        }

        public override void TearDown()
        {
            base.TearDown();
        }

        public void Initialize()
        {
            _listOfAllPiratesAvailable = _gameDataProvider.GetAllData<PirateModel>();

            InitializeUIElements();

            AddPiratesAllowedToBeGeneratedByBuildingToButtonsPanel();
        }

        private void InitializeUIElements()
        {
            ResolveElement(out _quitButton, "Main Panel/quit");
            ResolveElement(out _leftButton, "Main Panel/Left Button");
            ResolveElement(out _rightButton, "Main Panel/Right Button");
            ResolveElement(out _timeRemainingLabel, "Main Panel/Time Remaining");

            _mainPanel = GetElement("Main Panel") as GameObject;
            _buttonPanel = GetElement("Main Panel/Button Scroll View") as GameObject;
            _pirateDisplayImage = GetElement("Main Panel/Pirate Images") as GameObject;
        }

        private void AddPiratesAllowedToBeGeneratedByBuildingToButtonsPanel()
        {
            Debug.Log("Platoon Building Level : " + PlatoonBuildingLevel.ToString());

            for (int i = 0; i < PlatoonBuildingLevel; i++)
            {
                var fab = Object.Instantiate(_prefabProvider.GetPrefab("create_pirate_button")).gameObject.GetComponent<Button>();

                fab.gameObject.name = "pirate choice " + i.ToString();

                fab.name = "pirate choice " + i.ToString();

                var buttonLabel = fab.transform.GetChild(0).GetComponent<Text>();
                buttonLabel.text = _listOfAllPiratesAvailable[i].Name;

                var buttonNumberLabel = fab.transform.GetChild(2).GetComponent<Text>();

                buttonNumberLabel.text = _listOfAllPiratesAvailable[i].TrainingCost.ToString();

                fab.transform.SetParent(_buttonPanel.transform);
            }
        }
    }
}

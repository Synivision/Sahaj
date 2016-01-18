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
	public class CreatePirateCanvasController : BaseCanvasController
	{
		//Private utility objects
		private Canvas _canvas;
		private IoCResolver _resolver;
		private PrefabProvider _prefabProvider;
		private readonly Messager _messager;

		//Scroll Views
		private GameObject _parentPirateButtonPanel;
		private GameObject _parentPirateImagesScrollViewPanel;

		//Time left Label
		private Text timeLeftText;

		//Navigation Buttons
		private Button _leftNavigationButton;
		private Button _rightNavigationButton;
		private Button _quitButton;

		BuildingModel _buildingModel;

		List<Button> _pirateButtons;

		public CreatePirateCanvasController(IoCResolver resolver, Canvas canvasView,BuildingModel buildingModel)
		: base(resolver, canvasView){

			_buildingModel = buildingModel;
			_resolver = resolver;
			_canvas = canvasView;
			_resolver.Resolve(out _prefabProvider);
			_resolver.Resolve(out _messager);

			_pirateButtons = new List<Button>();

			ResolveElement (out _leftNavigationButton, "Main Panel/Left Button");
			ResolveElement (out _rightNavigationButton, "Main Panel/Right Button");
			ResolveElement (out _quitButton, "Main Panel/quit");

			var mainPanel = GetElement("Main Panel") as GameObject;
			_parentPirateButtonPanel = mainPanel.transform.GetChild (1).transform.GetChild (0).gameObject;

			_leftNavigationButton.gameObject.SetActive (false);
			_rightNavigationButton.gameObject.SetActive (false);

			//Add Pirate buttons to ui

			for (int i = 0; i < _buildingModel.piratesContained.Count; i++) {
			
				var fab = Object.Instantiate(_prefabProvider.GetPrefab("create_pirate_button")).gameObject.GetComponent<Button>();
				
				fab.gameObject.name = "pirate choice " + i.ToString();
				
				fab.name = "pirate choice " + i.ToString();

				var buttonLabel = fab.transform.GetChild(0).GetComponent<Text>();
				buttonLabel.text = _buildingModel.piratesContained[i].Name;

				var buttonNumberLabel = fab.transform.GetChild(2).GetComponent<Text>();
				
				buttonNumberLabel.text = _buildingModel.piratesContained[i].TrainingCost.ToString();

				fab.transform.SetParent(_parentPirateButtonPanel.transform);

				fab.onClick.AddListener(OnPirateButtonClicked);

				_pirateButtons.Add(fab);
			}

			//_leftNavigationButton.onClick.AddListener (OnQuitClicked);
			//_rightNavigationButton.onClick.AddListener (OnQuitClicked);
			_quitButton.onClick.AddListener (OnQuitClicked);

		}

		public void OnPirateButtonClicked(){

			//Add pirate to second scroll panel


		}

		public void OnQuitClicked(){

			TearDown ();

		}

		public override void TearDown()
		{
			foreach (var button in _pirateButtons) {

				button.onClick.RemoveAllListeners();
			
			}
			_pirateButtons.Clear ();
			_leftNavigationButton.onClick.RemoveAllListeners ();
			_rightNavigationButton.onClick.RemoveAllListeners ();
			_quitButton.onClick.RemoveAllListeners ();

			base.TearDown();
		}

	}
}
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
		//Name of the building that created the canvas
		private string _buildingName;

		//Private utility objects
		private Canvas _canvas;
		private IoCResolver _resolver;
		private PrefabProvider _prefabProvider;
		private readonly Messager _messager;
		private PirateGenerator _pirateGenerator;
		
		//Scroll Views
		private GameObject _parentPirateButtonPanel;
		private GameObject _parentPirateImagesScrollViewPanel;
		
		//Time left Label
		private Text timeLeftText;
		
		//Navigation Buttons
		private Button _leftNavigationButton;
		private Button _rightNavigationButton;
		private Button _quitButton;
		
		public BuildingModel BuildingModel{ get; set;}
		
		List<Button> _pirateButtons;
		List<PirateModel> _pirateVarietyList;
		List<string> _piratesBiengGenerated;
		List<Button> _piratesBiengGeneratedButtons;

		private MessagingToken _newPirateGeneratedMessage;

		public CreatePirateCanvasController(IoCResolver resolver, Canvas canvasView)
		: base(resolver, canvasView){
		
			_resolver = resolver;
			_canvas = canvasView;
			_resolver.Resolve(out _prefabProvider);
			_resolver.Resolve(out _messager);
			_resolver.Resolve (out _pirateGenerator);

		}

		public void Initialize(){

			_buildingName = BuildingModel.Name;

			_pirateButtons = new List<Button>();
			_pirateVarietyList = new List<PirateModel>();
			_piratesBiengGenerated = new List<string> ();
			_piratesBiengGeneratedButtons = new List<Button> ();
			
			
			ResolveElement (out _leftNavigationButton, "Main Panel/Left Button");
			ResolveElement (out _rightNavigationButton, "Main Panel/Right Button");
			ResolveElement (out _quitButton, "Main Panel/quit");
			ResolveElement (out timeLeftText,"Main Panel/Time Remaining");
			
			var mainPanel = GetElement("Main Panel") as GameObject;
			_parentPirateButtonPanel = mainPanel.transform.GetChild (1).transform.GetChild (0).gameObject;
			_parentPirateImagesScrollViewPanel = mainPanel.transform.GetChild (2).transform.GetChild (0).gameObject;
			
			//_leftNavigationButton.gameObject.SetActive (false);
			//_rightNavigationButton.gameObject.SetActive (false);
			
			//Add Pirate buttons to ui
			if(BuildingModel.piratesContained != null)
			for (int i = 0; i < BuildingModel.piratesContained.Count; i++) {
				
				var fab = Object.Instantiate(_prefabProvider.GetPrefab("create_pirate_button")).gameObject.GetComponent<Button>();
				
				fab.gameObject.name = "pirate choice " + i.ToString();
				
				fab.name = "pirate choice " + i.ToString();
				
				var buttonLabel = fab.transform.GetChild(0).GetComponent<Text>();
				buttonLabel.text = BuildingModel.piratesContained[i].Name;
				
				var buttonNumberLabel = fab.transform.GetChild(2).GetComponent<Text>();
				
				buttonNumberLabel.text = BuildingModel.piratesContained[i].TrainingCost.ToString();
				
				fab.transform.SetParent(_parentPirateButtonPanel.transform);
				
				//fab.onClick.AddListener(() => OnPirateButtonClicked(i));
				AddListenerToButton(ref fab,i);
				_pirateVarietyList.Add(BuildingModel.piratesContained[i]);
				_pirateButtons.Add(fab);
			}
			
			//_leftNavigationButton.onClick.AddListener (OnQuitClicked);
			//_rightNavigationButton.onClick.AddListener (OnQuitClicked);
			_quitButton.onClick.AddListener (OnQuitClicked);

			if(_pirateVarietyList.Count > 0)
				_newPirateGeneratedMessage = _messager.Subscribe<NewPirateGeneratedMessage>(RemovePirate);

			
		}
		
		public void AddListenerToButton(ref Button button, int position){
			
			button.onClick.AddListener(() => OnPirateButtonClicked(position));
			
		}
		
		public void OnPirateButtonClicked(int number){
			
			//Debug.Log ("Pirate button number " + number.ToString() + " clicked");
			
			if (!_piratesBiengGenerated.Contains (_pirateVarietyList [number].Name)) {

				//Add pirate to second scroll panel
				var fab = Object.Instantiate (_prefabProvider.GetPrefab ("pirate_bieng_generated")).gameObject.GetComponent<Button> ();
				fab.transform.SetParent (_parentPirateImagesScrollViewPanel.transform);
				fab.gameObject.name = _pirateVarietyList [number].Name;
				_piratesBiengGenerated.Add(_pirateVarietyList [number].Name);
				_piratesBiengGeneratedButtons.Add (fab);

				//Start Generation of Pirate
				_pirateGenerator.GeneratePirate(_pirateVarietyList[number],_buildingName,System.TimeSpan.FromSeconds((double)Time.time));


			} else {

				int buttonPosition;
				//foreach(var button in _piratesBiengGeneratedButtons)
				//	if(button.name == _pirateVarietyList[number].Name)
				//		buttonPosition = _piratesBiengGeneratedButtons.IndexOf(button);

				buttonPosition = _piratesBiengGeneratedButtons.IndexOf(_piratesBiengGeneratedButtons.Find (button => button.name == _pirateVarietyList[number].Name ));

				int numberOfPiratesOfThisType = System.Int32.Parse(_piratesBiengGeneratedButtons[buttonPosition].transform.GetChild(0).GetComponent<Text>().text);
				_piratesBiengGeneratedButtons[buttonPosition].transform.GetChild(0).GetComponent<Text>().text = (numberOfPiratesOfThisType+1).ToString();
				//Start Generation of Pirate
				_pirateGenerator.GeneratePirate(_pirateVarietyList[number],_buildingName,System.TimeSpan.FromSeconds((double)Time.time));
			}
		}
		
		public void OnQuitClicked(){
			
			disableCanvas ();

			foreach (var button in _pirateButtons) {
				
				button.onClick.RemoveAllListeners();
				GameObject.Destroy(button.gameObject);
			}

			foreach (var button in _piratesBiengGeneratedButtons) {
				
				button.onClick.RemoveAllListeners();
				GameObject.Destroy(button.gameObject);
			}

			_pirateButtons = new List<Button>();
			_piratesBiengGenerated = new List<string>();
			_piratesBiengGeneratedButtons = new List<Button>();
		}

		public override void Update(){
		
			base.Update ();
			timeLeftText.text = _pirateGenerator.GetTimeToCompletionOfPirate (_buildingName).ToString ();
		}

		public void RemovePirate(NewPirateGeneratedMessage message){
		
			var pos = _piratesBiengGenerated.IndexOf (message.PirateModel.Name);
			int buttonPosition = 0;
			foreach (var button in _piratesBiengGeneratedButtons) {
			


				if(button.name == message.PirateModel.Name){

				}

			}


			buttonPosition = _piratesBiengGeneratedButtons.IndexOf(_piratesBiengGeneratedButtons.Find (button => button.name == message.PirateModel.Name ));
				
			Debug.Log (message.PirateModel.Name+" button position is "+buttonPosition);
			int numberOfPiratesOfThisType = System.Int32.Parse(_piratesBiengGeneratedButtons[buttonPosition].transform.GetChild(0).GetComponent<Text>().text);
			if (numberOfPiratesOfThisType - 1 > 0) {

				_piratesBiengGeneratedButtons [buttonPosition].transform.GetChild (0).GetComponent<Text> ().text = (numberOfPiratesOfThisType - 1).ToString ();
		
			} else {

				_piratesBiengGeneratedButtons[buttonPosition].onClick.RemoveAllListeners();
				GameObject.Destroy(_piratesBiengGeneratedButtons[buttonPosition].gameObject);
				_piratesBiengGeneratedButtons.RemoveAt(buttonPosition);
				_piratesBiengGenerated.RemoveAt(buttonPosition);
			}
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
	
		public void enableCanvas() {

			//Recreate the buttons to show pirates bieng generated.

			_piratesBiengGenerated = _pirateGenerator.PiratesBiengGeneratedForBuilding (_buildingName);

			foreach (var pirateName in _piratesBiengGenerated) {
			
				var button = _piratesBiengGeneratedButtons.Find(temp => temp.name == pirateName);

				if(button != null ){

					int buttonPosition;
					buttonPosition = _piratesBiengGeneratedButtons.IndexOf(button);
					
					int numberOfPiratesOfThisType = System.Int32.Parse(_piratesBiengGeneratedButtons[buttonPosition].transform.GetChild(0).GetComponent<Text>().text);
					_piratesBiengGeneratedButtons[buttonPosition].transform.GetChild(0).GetComponent<Text>().text = (numberOfPiratesOfThisType+1).ToString();

				}else{

					var fab = Object.Instantiate (_prefabProvider.GetPrefab ("pirate_bieng_generated")).gameObject.GetComponent<Button> ();
					fab.transform.SetParent (_parentPirateImagesScrollViewPanel.transform);
					fab.gameObject.name = pirateName;
					_piratesBiengGeneratedButtons.Add (fab);

				}

			}

			_canvas.enabled = true;

		}

		public void disableCanvas() {
			_canvas.enabled = false;
		}
	}
}
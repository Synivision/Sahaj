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
		private MessagingToken _onFindInventoryItem;
        private PlayerManager _playerManager;
        private GameObject _mainPanel;
        private GameObject _pirateImage;
        private GameObject _pirateButtonScrollPanel;
        List<Button> _pirateButtonList;
        private InputSession _inputSession;
        private Button _previouslyClickedTileButton;
        private SpriteProvider _spriteProvider;

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

            _canvasView.gameObject.SetActive (true);
            _mainPanel = GetElement("MainPanel");


            // subscriptions
            //_onFindInventoryItem = _messager.Subscribe<FindInventoryItemMessage>(OnFindInventoryItem);
            _closeButton = _mainPanel.transform.GetChild(0).GetComponent<Button>();
			_closeButton.onClick.AddListener(onCloseClicked);

            _pirateImage = _mainPanel.transform.GetChild(1).gameObject;
            _pirateButtonScrollPanel = GetElement("MainPanel/MainChoosePiratePanel/ScrollPiratePanel/Content");


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
            //TODO add pirate to selected rowboat
            _pirateImage.GetComponent<Image>().sprite = _spriteProvider.GetSprite(name);
            _inputSession.CurrentlySelectedPirateName = name;
            if (_previouslyClickedTileButton == button) return;

            if (_previouslyClickedTileButton != null)
                _previouslyClickedTileButton.interactable = true;

            button.interactable = false;
            _previouslyClickedTileButton = button;
            

        }


        void onCloseClicked(){

			TearDown();
		}
		
		public void OnFindInventoryItem(FindInventoryItemMessage message){
			
			
			
		}
		
		public override void TearDown()
		{
            foreach (var button in _pirateButtonList)
            {

                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);

            }

            _closeButton.onClick.RemoveAllListeners();
			base.TearDown();

		}
		
	}
}

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
    public class RowBoatCanvasController : BaseCanvasController
    {
        private readonly Messager _messager;
        private readonly PrefabProvider _prefabProvider;
        private IoCResolver _resolver;
        private UiManager _uiManager;
        private CanvasProvider _canvasProvider;
        private Button _addNewPirateButton;
        private Button _closeButton;
        private PlayerManager _playerManager;
        private GameObject _mainPanel;
        private GameObject _seatPanel;
        private SpriteProvider _spriteProvider;

        private Dictionary<int, string> _seatsDictionary;

        public RowBoatCanvasController(IoCResolver resolver, Canvas canvasView, Dictionary<int ,string> seatsDictionary)
            : base(resolver, canvasView)
		{
            _resolver = resolver;
            _seatsDictionary = seatsDictionary;
            _resolver.Resolve(out _messager);
            _resolver.Resolve(out _canvasProvider);
            _resolver.Resolve(out _playerManager);
            _resolver.Resolve(out _prefabProvider);
            _resolver.Resolve(out _spriteProvider);
            _uiManager = new UiManager();

            ResolveElement(out _addNewPirateButton, "MainPanel/AddButton");
            ResolveElement(out _closeButton, "MainPanel/CloseButton");
            _mainPanel = GetElement("MainPanel");
            _seatPanel = GetElement("MainPanel/SeatPanel");
            _canvasView.gameObject.SetActive(true);

            _closeButton.onClick.AddListener(onCloseButtonClicked);
            _addNewPirateButton.onClick.AddListener(onAddPirateButtonClicked);


            //draw pirate images according to rowboat dict
                for (int i =0; i< _seatsDictionary.Count; i++) {

                    var button = _seatPanel.transform.GetChild(i).gameObject.GetComponent<Button>();
                    string buttonName = "";
                     _seatsDictionary.TryGetValue(i,out buttonName);

                    if (buttonName.Equals(""))
                    {
                        button.interactable = false;
                    }
                    else {

                        button.GetComponent<Image>().sprite = _spriteProvider.GetSprite(buttonName);
                        button.transform.GetChild(0).GetComponent<Text>().text = buttonName;
                    }
                }  
            
        }

        void onAddPirateButtonClicked()
        {
            _uiManager.RegisterUi(new InventoryCanvasController(_resolver, _canvasProvider.GetCanvas("InventoryCanvas")));
            TearDown();
        }

        void onCloseButtonClicked()
        {

            TearDown();
        }

        public override void TearDown()
        {
            _addNewPirateButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();

            //reset seats buttons
            for (int i = 0; i<6; i++) {
                var button = _seatPanel.transform.GetChild(i).gameObject.GetComponent<Button>();
                button.GetComponent<Image>().sprite = _spriteProvider.GetSprite("SwatchWhiteAlbedo");
                button.transform.GetChild(0).GetComponent<Text>().text = "Empty";
                button.interactable = true;

            }

            base.TearDown();

        }
    }
}

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
        private InputSession _inputSession;
        private UiManager _uiManager;
        private CanvasProvider _canvasProvider;
        private Button _closeButton;
        private Button _attackButton;
        private PlayerManager _playerManager;
        private GameObject _mainPanel;
        private GameObject _seatPanel;
        private SpriteProvider _spriteProvider;
        private MessagingToken _onUpdateRowBoatCanvasMessage;

        Dictionary<int, string> _seatsDictionary;
        string RowBoatName;

        public RowBoatCanvasController(IoCResolver resolver, Canvas canvasView, string rowBoatName)
            : base(resolver, canvasView)
        {
            _resolver = resolver;
            _resolver.Resolve(out _messager);
            _resolver.Resolve(out _canvasProvider);
            _resolver.Resolve(out _playerManager);
            _resolver.Resolve(out _prefabProvider);
            _resolver.Resolve(out _inputSession);
            _resolver.Resolve(out _spriteProvider);
            _uiManager = new UiManager();
            RowBoatName = rowBoatName;
            ResolveElement(out _closeButton, "MainPanel/CloseButton");
            _mainPanel = GetElement("MainPanel");
            _seatPanel = GetElement("MainPanel/SeatPanel");
            _canvasView.gameObject.SetActive(true);
            ResolveElement(out _attackButton, "MainPanel/SelectRowBoatButton");

            _closeButton.onClick.AddListener(onCloseButtonClicked);
            _attackButton.onClick.AddListener(AttackButtonClicked);

            _onUpdateRowBoatCanvasMessage = _messager.Subscribe<UpdateRowBoatCanvasMessage>(UpdateRowBoatCanvas);

            _playerManager.Model.RowBoatCountDict.TryGetValue(rowBoatName, out _seatsDictionary);


            //draw pirate images according to rowboat dict

            if (_seatsDictionary != null) {

                for (int i = 0; i < _seatsDictionary.Count; i++)
                {

                    var button = _seatPanel.transform.GetChild(i).gameObject.GetComponent<Button>();
                    string buttonName = "";
                    _seatsDictionary.TryGetValue(i, out buttonName);

                    if (buttonName.Equals(""))
                    {
                        // if seat empty then make button add pirate button
                        //button.interactable = false;
                        button.GetComponent<Image>().sprite = _spriteProvider.GetSprite("add_sprite");
                        button.transform.GetChild(0).GetComponent<Text>().text = "Add Pirate ";
                        button.onClick.AddListener(onAddPirateButtonClicked);
                    }
                    else
                    {

                        button.GetComponent<Image>().sprite = _spriteProvider.GetSprite(buttonName);
                        button.transform.GetChild(0).GetComponent<Text>().text = buttonName;
                    }
                }
            }
        }

        void AttackButtonClicked() {
            _inputSession.CurrentlySelectedRowBoatName = RowBoatName;
            _inputSession.CurrentlySelectedShipAttackName = null;
           TearDown();

        }

        void UpdateRowBoatCanvas(UpdateRowBoatCanvasMessage message) {

            _playerManager.Model.RowBoatCountDict.TryGetValue(message.BoatName, out _seatsDictionary);


            //draw pirate images according to rowboat dict

            if (_seatsDictionary != null)
            {

                for (int i = 0; i < _seatsDictionary.Count; i++)
                {

                    var button = _seatPanel.transform.GetChild(i).gameObject.GetComponent<Button>();
                    string buttonName = "";
                    _seatsDictionary.TryGetValue(i, out buttonName);

                    if (buttonName.Equals(""))
                    {
                        // if seat empty then make button add pirate button
                        //button.interactable = false;
                        button.GetComponent<Image>().sprite = _spriteProvider.GetSprite("add_sprite");
                        button.transform.GetChild(0).GetComponent<Text>().text = "Add Pirate ";
                       // button.onClick.AddListener(onAddPirateButtonClicked);
                    }
                    else
                    {

                        button.GetComponent<Image>().sprite = _spriteProvider.GetSprite(buttonName);
                        button.transform.GetChild(0).GetComponent<Text>().text = buttonName;
                    }
                }
            }


        }

        void onAddPirateButtonClicked()
        {
            _messager.Publish(new AddPirateToRowBoatMessage {
                BoatName = RowBoatName,
                onCancelled = () => {  }
            });
        }

        void onCloseButtonClicked()
        {
            _inputSession.CurrentlySelectedRowBoatName = "";
            TearDown();
        }

        public override void TearDown()
        {
            _closeButton.onClick.RemoveAllListeners();

            //reset seats buttons
            for (int i = 0; i<6; i++) {
                var button = _seatPanel.transform.GetChild(i).gameObject.GetComponent<Button>();
                button.GetComponent<Image>().sprite = _spriteProvider.GetSprite("SwatchWhiteAlbedo");
                button.transform.GetChild(0).GetComponent<Text>().text = "Empty";
                button.onClick.RemoveAllListeners();
                button.interactable = true;

            }

            base.TearDown();

        }
    }
}

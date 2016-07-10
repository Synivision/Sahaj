using UnityEngine;
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
    public class StockCanvasController : BaseCanvasController
    {

        private readonly Messager _messager;
        private readonly PrefabProvider _prefabProvider;
        private IoCResolver _resolver;
        private UiManager _uiManager;
        private CanvasProvider _canvasProvider;
        private Button _closeButton;
        private MessagingToken _onFindInventoryItem;
        private GameDataProvider _gameDataProvider;
        private Canvas _canvas;
		PlayerManager _playerManager;
		private List<Button> _shipAttackButtonList;
		private SpriteProvider _spriteProvider;
		private List<Button> _piratesButtonList;

		private GameObject _parentPiratesButtonObject;
		private GameObject _parentShipAttacksButtonObject;

		public StockCanvasController(IoCResolver resolver, Canvas canvasView)
            : base(resolver, canvasView)
		{

            _resolver = resolver;
            _resolver.Resolve(out _messager);
            _resolver.Resolve(out _canvasProvider);
            _resolver.Resolve(out _gameDataProvider);
			_resolver.Resolve(out _spriteProvider);
			_resolver.Resolve (out _prefabProvider);
            _uiManager = new UiManager();
			_resolver.Resolve(out _playerManager);
            _canvas = canvasView;


            //var mainPanel = GetElement("Panel");
            _canvasView.gameObject.SetActive(true);
            ResolveElement(out _closeButton,"Panel/Close");

            _closeButton.onClick.AddListener(onCloseClicked);

			var piratesPanel = GetElement("Panel/PiratesPanel");
			var shipAttackspanel = GetElement("Panel/ShipAttacksPanel");
			_parentShipAttacksButtonObject = shipAttackspanel.transform.GetChild(0).gameObject;

			_parentPiratesButtonObject = piratesPanel.transform.GetChild(0).gameObject;
			initializeView ();

        }

        void initializeView() {
			
			_shipAttackButtonList = new List<Button>();
			_piratesButtonList = new List<Button>();

			foreach (var item in _playerManager.Model.UnlockedShipAttacks)
			{

				if (item.Value == true)
				{
					_shipAttackButtonList.Add(CreateShipAttackButton(item.Key));

				}
			}

			foreach (var pirate in _playerManager.Model.PirateCountDict)
			{
				//if unlocked then add to list
				if (pirate.Value > 0) {

					_piratesButtonList.Add(CreatePirateButton(pirate.Key, pirate.Value));
				}
			}


        }

		public Button CreateShipAttackButton(string name) {

			var fab = Object.Instantiate(_prefabProvider.GetPrefab("pirate_button").gameObject.GetComponent<Button>());
			fab.gameObject.name = name;
			int value;
			_playerManager.Model.ShipAttackCostDict.TryGetValue(name, out value);
			fab.transform.GetChild(0).GetComponent<Text>().text = value.ToString();
			fab.GetComponent<Image>().sprite = _spriteProvider.GetSprite(name);
			var buttonNumberLabel = fab.transform.GetChild(1).GetComponent<Text>();
			buttonNumberLabel.text = "";
//			fab.onClick.AddListener(() => OnShipAttackButtonClicked(fab, name));
			fab.enabled = true;

			fab.transform.SetParent(_parentShipAttacksButtonObject.transform);
			fab.transform.localScale = Vector3.one;
			return fab;
		}

		public Button CreatePirateButton(string name, int pirateCount)
		{

			var fab = Object.Instantiate(_prefabProvider.GetPrefab("pirate_button")).gameObject.GetComponent<Button>();

			fab.gameObject.name = name;

			fab.GetComponent<Image>().sprite = _spriteProvider.GetSprite(name);

			var buttonLabel = fab.transform.GetChild(0).GetComponent<Text>();
			buttonLabel.text = name;

			var countNumberLabel = fab.transform.GetChild(1).GetComponent<Text>();
			countNumberLabel.text = pirateCount.ToString();

//			fab.onClick.AddListener(() => OnPirateButtonClicked(fab, name));

			fab.transform.SetParent(_parentPiratesButtonObject.transform);

			fab.transform.localScale = Vector3.one;

			return fab;
		}

        void onCloseClicked()
        {
            disableCanvas();
        }

        public void enableCanvas() {

            _canvas.enabled = true;
        }

       public void disableCanvas() {

            _canvas.enabled = false;
        }

        public override void TearDown()
        {
            _closeButton.onClick.RemoveAllListeners();
            base.TearDown();

        }
    }
}
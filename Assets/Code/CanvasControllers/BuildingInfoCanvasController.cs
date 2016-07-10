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

namespace Assets.Code.Ui.CanvasControllers
{
	public class BuildingInfoCanvasController : BaseCanvasController
	{
		private readonly Messager _messager;
		private readonly PrefabProvider _prefabProvider;
		private IoCResolver _resolver;
		private UiManager _uiManager;
		private CanvasProvider _canvasProvider;
		private Button _closeButton;
		private MessagingToken _onFindInventoryItem;
        private GameDataProvider _gameDataProvider;

        public BuildingInfoCanvasController(IoCResolver resolver, Canvas canvasView, string buildingName)
            : base(resolver, canvasView)
		{

			_resolver = resolver;
			_resolver.Resolve(out _messager);
			_resolver.Resolve(out _canvasProvider);
            _resolver.Resolve(out _gameDataProvider);
			_uiManager = new UiManager();


			var mainPanel = GetElement("Panel");
			_canvasView.gameObject.SetActive(true);
			// subscriptions
			_closeButton = mainPanel.transform.GetChild(0).GetComponent<Button>();
			_closeButton.onClick.AddListener(onCloseClicked);

            var buildingModel = _gameDataProvider.GetData<BuildingModel>(buildingName);
            var goldValueText = GetElement("Panel/GoldValue").GetComponent<Text>();
            var healthValueText = GetElement("Panel/HealthValue").GetComponent<Text>();
            var descriptionText = GetElement("Panel/DescText").GetComponent<Text>();

            //TODO: Check before the below code if these values are availalble
            healthValueText.text = buildingModel.Stats.MaximumHealth.ToString();
            goldValueText.text = buildingModel.GoldAmount.ToString();
            descriptionText.text = buildingModel.Descipriton;
        }

		void onCloseClicked()
		{

			TearDown();
		}

		public override void TearDown()
		{
			_closeButton.onClick.RemoveAllListeners();
			base.TearDown();

		}
	}
}
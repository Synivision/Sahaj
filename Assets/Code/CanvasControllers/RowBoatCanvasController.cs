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

        public RowBoatCanvasController(IoCResolver resolver, Canvas canvasView)
            : base(resolver, canvasView)
		{
            _resolver = resolver;
            _resolver.Resolve(out _messager);
            _resolver.Resolve(out _canvasProvider);
            _uiManager = new UiManager();

            ResolveElement(out _addNewPirateButton, "MainPanel/AddButton");
            ResolveElement(out _closeButton, "MainPanel/CloseButton");

            _canvasView.gameObject.SetActive(true);

            _closeButton.onClick.AddListener(onCloseButtonClicked);
            _addNewPirateButton.onClick.AddListener(onAddPirateButtonClicked);
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
//            _addNewPirateButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();

            base.TearDown();

        }





    }
}

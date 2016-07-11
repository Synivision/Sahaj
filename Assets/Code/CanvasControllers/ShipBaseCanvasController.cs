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
    public class ShipBaseCanvasController : BaseCanvasController {
        
        private Button _attackButton;
        private Button _shopButton;
        private Button _stockButton;
        private Text _fps;
        
        private Canvas _canvasView;
        private IoCResolver _resolver;
        private readonly Messager _messager;
        private UiManager _uiManager;
        private CanvasProvider _canvasProvider;



        public ShipBaseCanvasController(IoCResolver resolver, Canvas canvasView)
            : base(resolver, canvasView)
        {
            _canvasView = canvasView;
            _resolver = resolver;
            _resolver.Resolve (out _messager);
            _resolver.Resolve (out _canvasProvider);
            _uiManager = new UiManager ();

            ResolveElement (out _attackButton, "AttackButton");
            ResolveElement (out _shopButton, "ShopButton");
            ResolveElement (out _fps,"FpsText");
            ResolveElement(out _stockButton, "StockButton");

            _attackButton.onClick.AddListener (OnAttackClicked);
            _shopButton.onClick.AddListener(OnShopButtonClicked);
            _stockButton.onClick.AddListener(OnStockButtonClicked);
        }


        public void OnStockButtonClicked() {

            _messager.Publish(new OpenStockCanvasMessage { });
        }

        public void OnShopButtonClicked(){

            _messager.Publish(new OpenShopMessage{});
        }

        public void OnAttackClicked(){
    
            //TODO send message
            _uiManager.RegisterUi(new LevelSelectCanvasController(_resolver, _canvasProvider.GetCanvas("LevelSelectCanvas")));
            TearDown();

        }

    }
    
}

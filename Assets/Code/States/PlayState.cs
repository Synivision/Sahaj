using Assets.Code.DataPipeline;
using Assets.Code.Messaging;
using Assets.Code.Ui;
using UnityEngine;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;
using Assets.Code.Ui.CanvasControllers;

namespace Assets.Code.States
{

    public class PlayState : BaseState
    {
		public static  PrefabProvider _prefabProvider;
        /* REFERENCES */
        private readonly Messager _messager;

        /* PROPERTIES */
        private UiManager _uiManager;

		/* UiControllers */
		private MainCanvasController _mainCanvasController;

		private CanvasProvider _canvasProvider;

        public PlayState(IoCResolver resolver) : base(resolver)
        {
            _resolver.Resolve(out _messager);
			_resolver.Resolve(out _prefabProvider);
			_resolver.Resolve(out _canvasProvider);
        }

		public  static PrefabProvider Prefabs {
			get{
				return _prefabProvider;
			}
			set{
				_prefabProvider = value;

			}
		}

        public override void Initialize()
        {
            _uiManager = new UiManager();
            //_uiManager.RegisterUi( ... );

			createMainCanvas();
			createPirateCanvas();

            Debug.Log("Play state initialized.");


        }

		private void createMainCanvas(){

			Canvas mainCanvas = _canvasProvider.GetCanvas("MainCanvas");
			
			_mainCanvasController = new MainCanvasController(_resolver,mainCanvas);
			
			_uiManager.RegisterUi(_mainCanvasController);

		}

		private void createPirateCanvas(){
			
			Canvas pirateCanvas = _canvasProvider.GetCanvas("PirateInfoCanvas");
		
			PirateInfoCanvasController pirateCanvasConroller = new PirateInfoCanvasController(_resolver,pirateCanvas);
			
			_uiManager.RegisterUi(pirateCanvasConroller);
			
		}

        public override void Update()
        {
            _uiManager.Update();
            
            // super general input goes here
        }

        public override void HandleInput() { }

        public override void TearDown()
        {
            _uiManager.TearDown();

            _messager.CancelSubscription(/* any subscriptions you had get unsubscription */);
        }
    }
}

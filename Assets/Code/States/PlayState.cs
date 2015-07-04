using Assets.Code.DataPipeline;
using Assets.Code.Messaging;
using Assets.Code.Ui;
using UnityEngine;
using Assets.Code.DataPipeline.Providers;
using Assets.Code.Logic.Pooling;

namespace Assets.Code.States
{

    public class PlayState : BaseState
    {
		public static  PrefabProvider _prefabProvider;
        /* REFERENCES */
        private readonly Messager _messager;

        /* PROPERTIES */
        private UiManager _uiManager;

        public PlayState(IoCResolver resolver) : base(resolver)
        {
            _resolver.Resolve(out _messager);
			_resolver.Resolve(out _prefabProvider);
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

            Debug.Log("Play state initialized.");


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

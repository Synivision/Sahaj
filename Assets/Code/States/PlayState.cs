using Assets.Code.DataPipeline;
using Assets.Code.Messaging;
using Assets.Code.Ui;
using UnityEngine;

namespace Assets.Code.States
{
    public class PlayState : BaseState
    {
        /* REFERENCES */
        private readonly Messager _messager;

        /* PROPERTIES */
        private UiManager _uiManager;

        public PlayState(IoCResolver resolver) : base(resolver)
        {
            _resolver.Resolve(out _messager);
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

using Assets.Code.DataPipeline;

namespace Assets.Code.States
{
    public abstract class BaseState
    {
        protected IoCResolver _resolver;

        protected BaseState(IoCResolver resolver)
        {
            _resolver = resolver;
        }

        public BaseState TargetSwitchState { get; private set; }

        public bool IsReadyForStateSwitch { get { return TargetSwitchState != null; } }

        protected void SwitchState(BaseState newState)
        {
            TargetSwitchState = newState;
        }

        public abstract void Initialize();
        public abstract void Update();
        public abstract void HandleInput();
        public abstract void TearDown();
    }
}

using UnityEngine;

namespace Assets.Code.UnityBehaviours.Pooling
{
    public delegate void OnDeadEventHandler();

    public class PoolingBehaviour : MonoBehaviour
    {
        private bool _isDead;
        public bool IsDead
        {
            get { return _isDead; }
            protected set { _isDead = value;
                if (_isDead && OnDeadEvent != null) OnDeadEvent();
            }
        }
        public OnDeadEventHandler OnDeadEvent;

        public void Reset()
        {
            gameObject.SetActive(true);
            IsDead = false;
        }

        public virtual void Delete()
        {
            IsDead = true;
        }
    }
}

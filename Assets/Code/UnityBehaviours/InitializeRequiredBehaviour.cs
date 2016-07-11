using UnityEngine;

namespace Assets.Code.UnityBehaviours
{
	public abstract class InitializeRequiredBehaviour : MonoBehaviour
	{
		private bool _isInitialized;
		
		protected void MarkAsInitialized()
		{
			_isInitialized = true;
		}
		
		public void Start()
		{
			if (!_isInitialized)
			{
				Debug.Log("WARNING! " + GetType() + " " + gameObject.name + " has not been initialized!");
			}
		}
	}
}
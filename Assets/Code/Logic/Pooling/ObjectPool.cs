using System.Collections.Generic;
using Assets.Code.UnityBehaviours.Pooling;
using UnityEngine;


namespace Assets.Code.Logic.Pooling
{
    public class ObjectPool
    {
        /* REFERENCES */
        private readonly GameObject _prefab;

        /* PROPERTIES */
        private readonly List<PoolingBehaviour> _objects;
        private readonly List<PoolingBehaviour> _activeObjects;
        private readonly List<PoolingBehaviour> _inactiveObjects; 

        public ObjectPool(GameObject prefab)
        {
            _objects = new List<PoolingBehaviour>();
            _activeObjects = new List<PoolingBehaviour>();
            _inactiveObjects = new List<PoolingBehaviour>();

            // resolve references
            _prefab = prefab;

            if(_prefab.GetComponent<PoolingBehaviour>() == null)
                Debug.Log("WARNING! ObjectPool built for non-pooling behaviour : " + _prefab.name);
        }

        public PoolingBehaviour Instantiate()
        {
            PoolingBehaviour subject = null;

            if (_inactiveObjects.Count == 0)
                subject = CreateNewObject();
            else
            {
                subject = _inactiveObjects[0];
                _inactiveObjects.RemoveAt(0);
                _activeObjects.Add(subject);

                subject.Reset();
            }

            return subject;
        }

        private PoolingBehaviour CreateNewObject()
        {
            var fab = Object.Instantiate(_prefab);
			var fabBehaviour = fab.GetComponent<PoolingBehaviour>();
            fabBehaviour.OnDeadEvent += () => DeactivateObject(fabBehaviour);
			_objects.Add(fabBehaviour);

            return fabBehaviour;
        }

        private void DeactivateObject(PoolingBehaviour subject)
        {
            _activeObjects.Remove(subject);
            _inactiveObjects.Add(subject);

            subject.gameObject.SetActive(false);
        }

        public void TearDown()
        {

            foreach(var item in _objects){
                Object.Destroy(item.gameObject);

			}
			UnityEngine.Debug.Log("Object Length : " + _objects.Count.ToString());
			UnityEngine.Debug.Log("teardown objectpool");
            _objects.Clear();
            _inactiveObjects.Clear();
            _activeObjects.Clear();
        }
    }
}

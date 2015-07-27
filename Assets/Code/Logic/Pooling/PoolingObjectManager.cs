using System.Collections.Generic;
using Assets.Code.DataPipeline;
using Assets.Code.DataPipeline.Providers;
using UnityEngine;
using Assets.Code.UnityBehaviours.Pooling;

namespace Assets.Code.Logic.Pooling
{
    public class PoolingObjectManager : IResolvableItem
    {
        /* REFERENCES */
        private readonly PrefabProvider _prefabProvider;

        /* PROPERTIES */
        private readonly Dictionary<string, ObjectPool> _pools;

        public PoolingObjectManager(PrefabProvider prefabProvider)
        {
            _pools = new Dictionary<string, ObjectPool>();

            _prefabProvider = prefabProvider;
        }

		public  Dictionary<string, ObjectPool> getpools(){

			return _pools;

		}

        public PoolingBehaviour Instantiate(string prefabName)
        {
            if(!_pools.ContainsKey(prefabName))
                _pools.Add(prefabName, new ObjectPool(_prefabProvider.GetPrefab(prefabName)));



            return _pools[prefabName].Instantiate();
        }


        public void TearDown()
        {
            foreach(var pool in _pools){
                pool.Value.TearDown();
				//UnityEngine.Debug.Log(pool.Value.ToString());		
			}

        }

		//TearDownPirate
		//pooling behaviour
		//
    }
}

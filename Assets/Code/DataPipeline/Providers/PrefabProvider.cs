using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.DataPipeline.Providers
{
    public class PrefabProvider : IResolvableItem
    {
        private readonly Dictionary<string, GameObject> _prefabs;

        public PrefabProvider()
        {
            _prefabs = new Dictionary<string, GameObject>();
        }

        public void AddPrefab(GameObject newPrefab)
        {
            _prefabs.Add(newPrefab.name, newPrefab);
        }

        public GameObject GetPrefab(string name)
        {
            if (!_prefabs.ContainsKey(name))
            {
                Debug.Log("WARNING! prefab " + name + " does not exist");
                return null;
            }

            return _prefabs[name];
        }
    }
}

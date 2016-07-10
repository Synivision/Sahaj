using System.Collections.Generic;
using Assets.Code.Logic.Logging;
using UnityEngine;

namespace Assets.Code.DataPipeline.Providers
{
    public class PrefabProvider : IResolvableItem
    {
        private readonly CanonLogger _logger;
        private readonly Dictionary<string, GameObject> _prefabs;

        public PrefabProvider(CanonLogger CanonLogger)
        {
            _logger = CanonLogger;
            _prefabs = new Dictionary<string, GameObject>();
        }

        public void AddPrefab(GameObject newPrefab)
        {
            _prefabs.Add(newPrefab.name, newPrefab);
        }

        public GameObject GetPrefab(string name, bool expectingToFindItem = true)
        {
            if (string.IsNullOrEmpty(name)) return null;

            if (!_prefabs.ContainsKey(name))
            {
                _logger.Log("WARNING! prefab " + name + " does not exist", expectingToFindItem);

                return null;
            }

            return _prefabs[name];
        }
    }
}

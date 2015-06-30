using Assets.Code.DataPipeline.Providers;
using UnityEngine;

namespace Assets.Code.DataPipeline.Loading
{
    public static class PrefabLoader
    {
        public static void LoadPrefabs(PrefabProvider prefabProvider)
        {
            var prefabs = Resources.LoadAll<GameObject>("Prefabs/");
            foreach (var prefab in prefabs)
                prefabProvider.AddPrefab(prefab);
        }
    }
}

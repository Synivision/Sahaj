using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.DataPipeline.Providers
{
    internal class MaterialProvider : IResolvableItem
    {
        private readonly Dictionary<string, Material> _materials;

        public MaterialProvider()
        {
            _materials = new Dictionary<string, Material>();
        }

        public void AddMaterial(Material material)
        {
            if (material == null)
            {
                Debug.Log("WARNING! null material detected");
                return;
            }

            _materials.Add(material.name, material);
        }

        public Material GetMaterial(string name)
        {
            if (!_materials.ContainsKey(name))
            {
                Debug.Log("WARNING! material " + name + " does not exist");
                return null;
            }

            return _materials[name];
        }
    }
}

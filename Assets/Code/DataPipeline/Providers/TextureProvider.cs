using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.DataPipeline.Providers
{
    public class TextureProvider : IResolvableItem
    {
        private readonly Dictionary<string, Texture> _textures;

        public TextureProvider()
        {
            _textures = new Dictionary<string, Texture>();
        }

        public void AddTexture(Texture texture)
        {
            if (texture == null)
            {
                Debug.Log("WARNING! null texture detected");
                return;
            }

            _textures.Add(texture.name, texture);
        }

        public Texture GetTexture(string name)
        {
            if (!_textures.ContainsKey(name))
            {
                Debug.Log("WARNING! texture " + name + " does not exist");
                return null;
            }
            
            return _textures[name];
        }
    }
}

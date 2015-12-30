using System.Collections.Generic;
using Assets.Code.Logic.Logging;
using UnityEngine;

namespace Assets.Code.DataPipeline.Providers
{
    public class TextureProvider : IResolvableItem
    {
        private readonly Logger _logger;
        private readonly Dictionary<string, Texture> _textures;

        public TextureProvider(Logger logger)
        {
            _logger = logger;
            _textures = new Dictionary<string, Texture>();
        }

        public void AddTexture(Texture texture)
        {
            if (texture == null)
            {
                _logger.Log("WARNING! null texture detected", true);
                return;
            }

            _textures.Add(texture.name, texture);
        }

        public Texture GetTexture(string name, bool expectingToFindItem = true)
        {
            if (name == null) return null;

            if (!_textures.ContainsKey(name))
            {
                _logger.Log("WARNING! texture " + name + " does not exist", expectingToFindItem);
                return null;
            }

            return _textures[name];
        }
    }
}

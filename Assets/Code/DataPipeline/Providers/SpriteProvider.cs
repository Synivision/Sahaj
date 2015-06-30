using System.Collections.Generic;
using System.Linq;
using Assets.Code.Extensions;
using UnityEngine;

namespace Assets.Code.DataPipeline.Providers
{
    public class SpriteProvider : IResolvableItem
    {
        private readonly Dictionary<string, Sprite> _sprites;

        public SpriteProvider()
        {
            _sprites = new Dictionary<string, Sprite>();
        }

        public void AddSprite(Sprite sprite)
        {
            if (sprite == null)
            {
                Debug.Log("WARNING! null sprite detected");
                return;
            }

            _sprites.Add(sprite.name, sprite);
        }

        public Sprite GetSprite(string name)
        {
            if (!_sprites.ContainsKey(name))
            {
                Debug.Log("WARNING! sprite " + name + " does not exist");
                return null;
            }

            return _sprites[name];
        }

        public List<Sprite> GetSpritesOfType(string prefix)
        {
            var lowerCasePrefix = prefix.ToFormalString().ToLower();

            return _sprites.Where (sprite => sprite.Key.ToLower().StartsWith(lowerCasePrefix))
                           .Select(sprite => sprite.Value)
                           .ToList();
        }

        public Sprite GetRandomSpriteOfType(string prefix)
        {
            var lowerCasePrefix = prefix.ToFormalString().ToLower();

            return _sprites.Where(sprite => sprite.Key.ToLower().StartsWith(lowerCasePrefix))
                           .Select(sprite => sprite.Value)
                           .ToList()
                           .GetRandomItem();
        }
    }
}

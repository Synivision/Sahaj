using System.Collections.Generic;
using Assets.Code.Logic.Logging;
using UnityEngine;

namespace Assets.Code.DataPipeline.Providers
{
    public class SoundProvider : IResolvableItem
    {
        /* PROPERTIES */
        private readonly CanonLogger _logger;
        private readonly Dictionary<string, AudioClip> _sounds;

        public SoundProvider(CanonLogger CanonLogger)
        {
            _logger = CanonLogger;
            _sounds = new Dictionary<string, AudioClip>();
        }

        public void AddSound(AudioClip sound)
        {
            if (!_sounds.ContainsKey(sound.name))
                _sounds.Add(sound.name, sound);
        }

        public AudioClip GetSound(string query, bool expectingToFindItem = true)
        {
            if (_sounds.ContainsKey(query))
                return _sounds[query];

            if (!string.IsNullOrEmpty(query))
                _logger.Log("WARNING! sound " + query + " does not exist!", expectingToFindItem);

            return null;
        }
    }
}

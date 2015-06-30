using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.DataPipeline.Providers
{
    public class SoundProvider : IResolvableItem
    {
        /* PROPERTIES */
        private readonly Dictionary<string, AudioClip> _sounds;

        public SoundProvider()
        {
            _sounds = new Dictionary<string, AudioClip>();
        }

        public void AddSound(AudioClip sound)
        {
            if(!_sounds.ContainsKey(sound.name))
                _sounds.Add(sound.name, sound);
        }

        public AudioClip GetSound(string query)
        {
            if (_sounds.ContainsKey(query))
                return _sounds[query];

            Debug.Log("WARNING! sound " + query + " does not exist!");
            return null;
        }
    }
}

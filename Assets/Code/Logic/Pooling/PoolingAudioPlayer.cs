using System.Linq;
using Assets.Code.DataPipeline;
using Assets.Code.Logic.Logging;
using Assets.Code.Models.Pooling;
using Assets.Code.UnityBehaviours;
using Assets.Code.UnityBehaviours.Pooling;
using UnityEngine;

namespace Assets.Code.Logic.Pooling
{
    public class PoolingAudioPlayer : IResolvableItem
    {
        /* CONSTANTS */
        private const int NumberOfSources = 32;

        /* REFERENCES */
        private readonly CanonLogger _logger;

        /* PROPERTIES */
        private readonly PooledAudioSource[] _sources;
        private readonly GameObject _sourceParent;

        public PoolingAudioPlayer(CanonLogger CanonLogger, UnityReferenceMaster unityReferenceMaster, GameObject audioSourcePrefab)
        {
            _logger = CanonLogger;

            _sourceParent = Object.Instantiate(new GameObject());
            _sourceParent.name = "audio_source_parent";

            _sources = new PooledAudioSource[NumberOfSources];

            for (var i = 0; i < NumberOfSources; i++)
            {
                _sources[i] = Object.Instantiate(audioSourcePrefab).GetComponent<PooledAudioSource>();
                _sources[i].transform.SetParent(_sourceParent.transform);
                _sources[i].Initialize(unityReferenceMaster);
            }
        }

        public void PingStatus()
        {
            var activeSources = _sources.Where(source => source.IsActive).ToList();
            _logger.Log(string.Format("{0} of {1} sources are active", activeSources.Count, _sources.Length), true);
            foreach (var source in activeSources)
            {
                var audioData = source.GetComponent<AudioSource>();
                _logger.Log(string.Format("{0} sound playing {1}", audioData.clip.name, audioData.loop ? "on loop" : "one shot"), true);
            }
        }

        public AudioToken PlaySound(PooledAudioRequest request)
        {
            for (var i = 0; i < NumberOfSources; i++)
                if (!_sources[i].IsActive)
                {
                    var source = _sources[i];
                    return source.PlaySound(request);
                }

            _logger.Log("NOTE! ran out of audio sources in pool!", true);
            return new AudioToken
            {
                IsCurrentlyActive = false,

                Replace = newRequest => PlaySound(newRequest),
                End = () => { },
                TrailOff = time => { }
            };
        }

        public void KillAllSounds()
        {
            foreach (var source in _sources)
                source.Stop();
        }
    }
}

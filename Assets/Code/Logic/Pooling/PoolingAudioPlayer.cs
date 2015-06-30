using Assets.Code.DataPipeline;
using UnityEngine;

namespace Assets.Code.Logic.Pooling
{
    public class PoolingAudioPlayer : IResolvableItem
    {
        /* CONSTANTS */
        private const int NumberOfSources = 16;

        /* PROPERTIES */
        private readonly AudioSource[] _sources;
        private readonly GameObject _sourceParent;

        public PoolingAudioPlayer(GameObject audioSourcePrefab)
        {
            _sourceParent = Object.Instantiate(new GameObject());
            _sourceParent.name = "audio_source_parent";

            _sources = new AudioSource[NumberOfSources];

            for (var i = 0; i < NumberOfSources; i++)
            {
                _sources[i] = Object.Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
                _sources[i].transform.SetParent(_sourceParent.transform);
            }
        }

        public void PlaySound(Vector3 position, AudioClip sound, float volume = 1f)
        {
            for (var i = 0; i < NumberOfSources; i++)
                if (!_sources[i].isPlaying)
                {
                    _sources[i].transform.position = position;
                    _sources[i].PlayOneShot(sound, volume);
                    return;
                }

            Debug.Log("NOTE! ran out of audio sources in pool!");
        }

        public AudioLoopToken LoopSound(Vector3 position, AudioClip sound, float volume = 1f)
        {
            for (var i = 0; i < NumberOfSources; i++)
                if (!_sources[i].isPlaying)
                {
                    var source = _sources[i];

                    source.transform.position = position;
                    source.clip = sound;
                    source.loop = true;
                    source.volume = volume;
                    source.Play();

                    return new AudioLoopToken
                    {
                        Replace = (replacementPosition, replacement, replacementVolume)
                                => ReplaceLoop(source, replacementPosition, replacement, replacementVolume),
                        End = () => EndLoop(source)
                    };
                }

            Debug.Log("NOTE! ran out of audio sources in pool!");
            return new AudioLoopToken
            {
                Replace = (i, j, k) => ReplaceLoop(null, Vector3.zero, null, 0f),
                End = () => EndLoop(null)
            };
        }

        private void ReplaceLoop(AudioSource source, Vector3 position, AudioClip replacement, float volume = 1f)
        {
            if (source != null)
            {
                source.Stop();

                if (replacement != null)
                {
                    source.transform.position = position;
                    source.clip = replacement;
                    source.loop = true;
                    source.volume = volume;

                    source.Play();
                }
                else
                    source.loop = false;
            }
        }

        private void EndLoop(AudioSource source)
        {
            if (source != null)
            {
                source.Stop();
                source.loop = false;
            }
        }

        public void KillAllSounds()
        {
            foreach(var source in _sources)
                source.Stop();
        }
    }
}

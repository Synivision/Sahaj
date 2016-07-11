using System;
using UnityEngine;

namespace Assets.Code.Models.Pooling
{
    public class PooledAudioRequest
    {
        public AudioClip Sound;
        public float Volume;
        public Vector3 Target;
        public bool IsSpatial;
        public bool IsMusic;
        public bool IsLoop;
        public PooledAudioRequest Next;

        public Action OnFinished;

        public PooledAudioRequest()
        {
            Volume = 1f;
            IsSpatial = true;
            IsMusic = false;
            IsLoop = false;
        }
    }
}

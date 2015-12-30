using System;

namespace Assets.Code.Models.Pooling
{
    public class AudioToken
    {
        public bool IsCurrentlyActive;

        public Func<PooledAudioRequest, AudioToken> Replace;
        public Action End;
        public Action<float> TrailOff;
    }
}

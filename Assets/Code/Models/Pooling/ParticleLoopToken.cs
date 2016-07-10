using System;

namespace Assets.Code.Models.Pooling
{
    public class ParticleLoopToken
    {
        public Func<ActiveParticleLoop, ParticleLoopToken> Replace;
        public Action End;
    }
}

using System;
using UnityEngine;

namespace Assets.Code.Logic.Pooling
{
    public class AudioLoopToken
    {
        public Action<Vector3, AudioClip, float> Replace;
        public Action End;
    }
}

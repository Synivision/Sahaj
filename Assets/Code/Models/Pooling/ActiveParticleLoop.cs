using System;
using UnityEngine;

namespace Assets.Code.Models.Pooling
{
    public class ActiveParticleLoop
    {
        public float EmitModifier;

        public string EffectName;
        public Transform Location;
        public Vector3 Offset;
        public Color Tint;

        public Action<ParticleSystem> Shape;

        public ActiveParticleLoop()
        {
            EmitModifier = 1f;
            Offset = Vector3.zero;
            Tint = Color.white;
        }
    }
}

using System.Collections.Generic;

namespace Assets.Code.Utilities
{
    internal class LayerLabels
    {
        public static readonly string ProjectileLabel = "projectile";
        public static readonly int Projectile = 9;

        public static readonly string AetherLabel = "aether";
        public static readonly int Aether = 10;

        public static readonly string MetaProjectileLabel = "meta_projectile";
        public static readonly int MetaProjectile = 11;

        public static Dictionary<string, int> Lookup = new Dictionary<string, int>{
            { ProjectileLabel, Projectile },
            { AetherLabel, Aether },
            { MetaProjectileLabel, MetaProjectile }
        };
    }
}

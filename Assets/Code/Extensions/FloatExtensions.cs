using UnityEngine;

namespace Assets.Code.Extensions
{
    public static class FloatExtensions
    {
        // http://stackoverflow.com/a/7085653
        public static float Round(float hwat, float to)
        {
            return to * Mathf.Round(hwat / to);
        }
    }
}

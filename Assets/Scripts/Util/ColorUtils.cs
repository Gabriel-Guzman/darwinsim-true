using UnityEngine;
using UnitySharpNEAT.SharpNEAT.Utility;

namespace Util
{
    public class ColorUtils
    {
        private static readonly FastRandom Random = new();

        public static Color RandomColor()
        {
            return new Color(RandomFloat(), RandomFloat(), RandomFloat(), 1);
        }

        private static float RandomFloat()
        {
            return (float)Random.NextDouble();
        }
    }
}
namespace Util
{
    public class Physics
    {
        public static float SoftenForce(float force, float velocity, float threshold)
        {
            return force * (threshold - velocity) / threshold;
        }
    }
}
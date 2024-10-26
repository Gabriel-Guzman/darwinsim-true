using UnityEngine;

namespace Entities
{
    public class TouchedSomethingCollisionHandler : ILivingCollisionHandler
    {
        public void HandleCollisions(Living living, Collider2D[] contacts, int num)
        {
            if (num > 0) living.TouchedThisTick = true;
        }
    }
}
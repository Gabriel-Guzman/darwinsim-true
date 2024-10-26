using UnityEngine;

namespace Entities
{
    public interface ILivingCollisionHandler
    {
        void HandleCollisions(Living living, Collider2D[] contacts, int num);
    }
}
using UnityEngine;

namespace Entities
{
    public abstract class Entity<T> : MonoBehaviour where T : Entity<T>
    {
        public EntityManager<T> Manager;
    }
}
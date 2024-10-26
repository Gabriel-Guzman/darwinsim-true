using System.Collections.Generic;
using UnityEngine;
using UnitySharpNEAT.SharpNEAT.Utility;

namespace Entities
{
    public class EntityManager<T> where T : Entity<T>
    {
        public readonly GameObject Arena;
        private readonly T _entityPrefab;
        public readonly string EntityName;
        private readonly FastRandom _fr = new();

        public Dictionary<string, T> Entities { get; } = new();
        private uint _idx;


        public EntityManager(string entityName, GameObject arena, T entityPrefab)
        {
            Arena = arena;
            _entityPrefab = entityPrefab;
            EntityName = entityName;
        }

        private uint NewIdx()
        {
            return ++_idx;
        }

        public bool HasEntity(string name)
        {
            return Entities.ContainsKey(name);
        }

        public T GetEntity(string name)
        {
            return Entities[name];
        }

        public void ReplaceEntity(string name)
        {
            RemoveEntity(name);
            Spawn(1);
        }

        public void RemoveEntity(string name)
        {
            var entity = Entities[name];
            RemoveEntity(entity);
        }

        public void RemoveEntity(T entity)
        {
            Object.Destroy(entity.gameObject);
            Entities.Remove(entity.name);
        }

        public void Empty()
        {
            foreach (var entity in Entities)
            {
                Object.Destroy(entity.Value.gameObject);
            }

            Entities.Clear();
            _idx = 0;
        }

        public List<T> Spawn(int count)
        {
            var spawned = new List<T>();
            for (var i = 0; i < count; i++)
            {
                spawned.Add(Spawn());
            }

            return spawned;
        }

        protected T Spawn()
        {
            var newEntity = Object.Instantiate(_entityPrefab,
                Arena.transform.TransformVector(NewStartingPosition()),
                NewStartingRotation(),
                Arena.transform
            );

            var name = EntityName + NewIdx();
            newEntity.name = name;
            newEntity.Manager = this;
            Entities.Add(name, newEntity);
            return newEntity;
        }


        private Vector3 NewStartingPosition()
        {
            float RandInRange()
            {
                return ((float)_fr.NextDouble() - 0.5f) * 0.9f;
            }

            return new Vector3(
                RandInRange(),
                RandInRange(),
                0
            );
        }

        private Quaternion NewStartingRotation()
        {
            return Quaternion.Euler(0, 0, (float)(360 * _fr.NextDouble()));
        }
    }
}
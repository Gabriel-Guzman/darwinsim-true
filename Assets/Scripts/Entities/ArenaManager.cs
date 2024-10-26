using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class ArenaManager
    {
        private Dictionary<Type, object> entityManagers = new();

        public ArenaManager()
        {
            // entityManagers = new Dictionary<Type, object>();
            // entityManagers.Add(typeof(Living), new EntityManager<Living>("Living", arena, );
            // entityManagers.Add(typeof(TreeBehavior), treeManager);
        }

        public void AddEntityManager<T>(EntityManager<T> entityManager) where T : Entity<T>
        {
            if (entityManagers.ContainsKey(typeof(T)))
            {
                throw new ArgumentException($"EntityManager for type {typeof(T)} already exists.");
            }

            entityManagers.Add(typeof(T), entityManager);
        }

        public T GetEntity<T>(string name) where T : Entity<T>
        {
            if (!entityManagers.ContainsKey(typeof(T)))
            {
                throw new ArgumentException($"No EntityManager for type {typeof(T)} found.");
            }

            var entityManager = (EntityManager<T>)entityManagers[typeof(T)];
            var entity = entityManager.GetEntity(name);

            if (!entity)
            {
                throw new ArgumentException(
                    $"Entity with name {name} not found in EntityManager for type {typeof(T)}.");
            }

            return entity;
        }

        public EntityManager<T> GetEntityManager<T>() where T : Entity<T>
        {
            if (!entityManagers.ContainsKey(typeof(T)))
            {
                throw new ArgumentException($"No EntityManager for type {typeof(T)} found.");
            }

            return (EntityManager<T>)entityManagers[typeof(T)];
        }
    }
}
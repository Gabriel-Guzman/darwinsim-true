using UnityEngine;

namespace Entities
{
    public class EatTreesCollisionHandler : ILivingCollisionHandler
    {
        private readonly float _eatingHealthPerSecond;

        public EatTreesCollisionHandler(float eatingHealthPerSecond)
        {
            _eatingHealthPerSecond = eatingHealthPerSecond;
        }

        public void HandleCollisions(Living living, Collider2D[] contacts, int num)
        {
            for (var i = 0; i < num; i++)
            {
                var c = contacts[i];
                // if c is a tree, eat it
                var isTree = living.TreeManager.HasEntity(c.name);
                if (isTree)
                {
                    var tree = living.TreeManager.GetEntity(c.name);
                    tree.Bite();
                    living.AteThisTick = true;
                    living.Health += _eatingHealthPerSecond * Time.fixedDeltaTime;
                    if (living.Health > 1) living.Health = 1;
                    living.Score += Time.fixedDeltaTime * 10;
                }
            } 
        }
    }
}
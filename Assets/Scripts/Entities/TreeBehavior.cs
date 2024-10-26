using UnityEngine.UI;

namespace Entities
{
    public class TreeBehavior : Entity<TreeBehavior> 
    {
        private readonly float _damagePerBite = 1 / 50f * 1 / 40;
        public float health = 1;
        // public HealthBarBehavior _healthBarBehavior;
        public Slider healthBar;

        private void SetHealthBar(float health)
        {
            healthBar.value = health;
        }
        public void Bite()
        {
            health -= _damagePerBite;
            // TODO get this to work
            SetHealthBar(health);
            if (health <= 0)
            {
                Manager.ReplaceEntity(name);
            }
        }
    }
}
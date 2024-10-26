using UnityEngine;

namespace Entities
{
    public class HealthBarBehavior : MonoBehaviour
    {
        // public Slider Slider;
        public Canvas canvas;

        private void Start()
        {
            // Slider = transform.GetComponentInChildren<Slider>();
            canvas = GetComponent<Canvas>();
        }

        public void Disable()
        {
            canvas.enabled = false;
        }
    }
}
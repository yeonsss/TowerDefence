using UnityEngine;

namespace Controllers.Tower
{
    public class TowerAttackRangeController : MonoBehaviour
    {
        private CircleCollider2D _collider;

        public void Awake()
        {
            _collider = transform.GetComponent<CircleCollider2D>();
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Enemy"))
            {
                var enemy = col.GetComponent<EnemyController>();
                var tc = transform.parent.GetComponent<TowerController>();
                tc.enemies.Add(enemy);
            }
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Enemy"))
            {
                var enemy = col.GetComponent<EnemyController>();
                var tc = transform.parent.GetComponent<TowerController>();
                if (tc.enemies.Contains(enemy))
                {
                    tc.enemies.Remove(enemy);
                }
            }
        }

        public void SetRadius(float range)
        {
            _collider.radius = range;
        }
    }
}
using System;
using Core;
using Managers;
using UnityEngine;
using static Define;

namespace Controllers.Projectile
{
    public class ProjectileController : MonoBehaviour
    {
        private GameObject _enemyTarget;
        private float moveSpeed { get; set; }
        private int damage { get; set; }
        private SpriteRenderer _image;

        private void Awake()
        {
            _image = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_enemyTarget == null || _enemyTarget.activeSelf == false)
            {
                _enemyTarget = null;
                SpawnManager.instance.ReturnInstance(gameObject, PoolType.ProjectilePool);
                return;
            }

            Rotate();
            Move();
        }

        private void Move()
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                _enemyTarget.transform.position, 
                moveSpeed * Time.deltaTime);

            float distanceToTarget = (_enemyTarget.transform.position - transform.position).magnitude;

            if (distanceToTarget < minDistanceToDealDamage)
            {
                var ec = _enemyTarget.GetComponent<EnemyController>();
                ec.DealDamage(damage);
                SpawnManager.instance.ReturnInstance(gameObject, PoolType.ProjectilePool);
            }
        }

        private void Rotate()
        {
            if (_enemyTarget == null) return;

            var targetPos = _enemyTarget.transform.position - transform.position;
            // 1. 회전의 시작이 되는 백터
            // transform.up 이니까 객체에 세로 선을 그은 느낌으로 생각.
            // 2. 적과 나의 상대 백터
            // 3. 회전의 기준이 되는 백터
            // z 좌표가 기준이 되어야 x와 y 좌표를 이용해서 회전이 가능
            float angle = Vector3.SignedAngle(transform.up, targetPos, transform.forward);
            transform.Rotate(0f, 0f, angle);
        }

        public void SetProjectileBaseImageAndData(string towerName, int level, GameObject enemy)
        {
            _enemyTarget = enemy;
            
            var data = DataManager.instance.towerInfo[towerName];
            var imagePath = data.projectileImagePath;
            var sprite = ResourceManager.instance.Load<Sprite>(imagePath);

            var upgradeDamage = GameManager.instance.upgradeDamageDict[towerName];

            _image.sprite = sprite;

            moveSpeed = data.projectileSpeed;
            damage = data.levels[level].damage + upgradeDamage;
        }
    }
}
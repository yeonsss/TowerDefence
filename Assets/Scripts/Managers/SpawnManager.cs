using System.Collections;
using System.Collections.Generic;
using Controllers;
using Controllers.Projectile;
using Controllers.Tower;
using Core;
using UnityEngine;

namespace Managers
{
    public class RoundData
    {
        public List<string> monsterList = new List<string>()
        {
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
            "UFO-1",
        };
    }
    public class SpawnManager : Singleton<SpawnManager>
    {
        [SerializeField] private GameObject _spawnTestObj;
        private RoundData _data;
        private ObjectPool _spawnPool;
        private float _spawnTime = 2.0f;

        public override void Init()
        {
            _data = new RoundData();
            _spawnPool = new ObjectPool();
        }

        public void SpawnEnemy(Vector3 spawnPos, string enemyName, int lv = 1)
        {
            var newInstance = _spawnPool.GetInstance("BaseEnemy", PoolType.EnemyPool);
            newInstance.transform.position = spawnPos;
            newInstance.SetActive(true);

            var em = newInstance.GetComponent<EnemyController>();
            em.enemyName = enemyName;
            em.level = lv;
            em.SetEnemyBaseImageAndData();
        }

        public void ObjectPoolClear()
        {
            _spawnPool.InstanceAllDelete();
        }

        public void ReturnInstance(GameObject obj, PoolType type)
        {
            _spawnPool.ReturnInstance(obj, type);
        }

        public GameObject SpawnTower(Vector3 pos, string towerName, int lv = 1)
        {   
            var newInstance = _spawnPool.GetInstance("BaseTower", PoolType.TowerPool);
            newInstance.transform.position = pos;
            newInstance.SetActive(true);

            var tc = newInstance.GetComponent<TowerController>();
            tc.level = lv;
            tc.towerName = towerName;
            tc.SetTowerBaseImageAndData(lv);

            return newInstance;
        }

        public void DeleteTower(GameObject tower)
        {
            tower.GetComponent<TowerController>().containerTransform.localPosition = Vector3.zero;
            _spawnPool.ReturnInstance(tower, PoolType.TowerPool);
        }

        public GameObject SpawnProjectile(GameObject origin, GameObject target, Vector3 pos)
        {
            var newInstance = _spawnPool.GetInstance("BaseProjectile", PoolType.ProjectilePool);
            newInstance.transform.position = pos;
            newInstance.SetActive(true);

            var pp = newInstance.GetComponent<ProjectileController>();
            var tower = origin.GetComponent<TowerController>();
            pp.SetProjectileBaseImageAndData(tower.towerName, tower.infoLevel, target);
            
            return newInstance;
        }
    }
}
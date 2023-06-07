using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Controllers.Tower
{
    public class TowerController : MonoBehaviour
    {
        private Animator _anim;
        private Transform _container;
        private Transform _lv;
        private Transform _image;
        private Coroutine _levelUpCor;
        public List<EnemyController> enemies;
        private EnemyController _currentEnemy;
        private TowerAttackRangeController _range;

        private bool _isAttackPossible;

        public int level { get; set; }
        public int infoLevel => level - 1;

        public string towerName { get; set; }

        public Transform containerTransform => _container.transform;

        public void Awake()
        {
            _isAttackPossible = true;
            _container = transform.Find("Container");
            _image = transform.Find("Container/Image");
            _lv = transform.Find("Container/LV/Text");
            _anim = _image.GetComponent<Animator>();
            _range = _container.GetComponent<TowerAttackRangeController>();
            enemies = new List<EnemyController>();
        }

        public void OnDisable()
        {
            _isAttackPossible = true;
            enemies.Clear();
        }

        public void Update()
        {
            GetCurrentEnemyTarget();
            Attack();
        }
        
        private IEnumerator AttackCoolTimeCor()
        {
            var time = DataManager.instance.towerInfo[towerName].attackCool;
            yield return new WaitForSeconds(time);
            _isAttackPossible = true;
        }

        private void LevelUp()
        {
            var randTowerName = GameManager.instance.GetRandomTowerName();
            towerName = randTowerName;
            
            level += 1;
            _lv.GetComponent<TMP_Text>().text = level.ToString();
            SetTowerBaseImage(level);
        }

        private IEnumerator LevelUpCor()
        {
            _anim.Play("Merge_Tower");
            yield return new WaitForSeconds(1);
            _anim.Play("Idle_Tower");
            _levelUpCor = null;
        } 

        public void Merge()
        {
            LevelUp();
            if (_levelUpCor == null)
            {
                _levelUpCor = StartCoroutine(LevelUpCor());
            }
        }

        public void Attack()
        {
            if (_isAttackPossible == false) return;
            if (_currentEnemy == null) return;

            _isAttackPossible = false;
            SpawnManager.instance.SpawnProjectile(gameObject, _currentEnemy.gameObject, transform.position);
            StartCoroutine(AttackCoolTimeCor());
        }

        private void GetCurrentEnemyTarget()
        {
            if (enemies.Count < 1)
            {
                _currentEnemy = null;
                return;
            }
            
            var enemy = enemies.Find(item => item.gameObject.activeSelf == true);
            if (_currentEnemy == null)
            {
                _currentEnemy = enemy;
                return;
            }

            if (_currentEnemy.gameObject.activeSelf == false)
            {
                _currentEnemy = enemy;
            }
        }

        public void SetTowerBaseImageAndData(int updateLevel)
        {
            SetTowerBaseImage(updateLevel);
            var data = DataManager.instance.towerInfo[towerName];
            var distance = data.attackDistance;
            _range.SetRadius(distance);
        }

        public void SetTowerBaseImage(int updateLevel)
        {
            var lv = 0;
            if (1 <= updateLevel && updateLevel < 3)
            {
                lv = 1;
            }
            else if (3 <= updateLevel && updateLevel < 5)
            {
                lv = 3;
            }
            else if (5 <= updateLevel)
            {
                lv = 5;
            }
            
            var data = DataManager.instance.towerInfo[towerName];
            var levelData = data.levels[lv - 1];
            
            if (data.animPath == null) return;
            if (levelData.imagePath == null) return;
            
            var sprite = ResourceManager.instance.Load<Sprite>(levelData.imagePath);
            _image.GetComponent<SpriteRenderer>().sprite = sprite;

            var animator = ResourceManager.instance.Load<RuntimeAnimatorController>(data.animPath);
            _image.GetComponent<Animator>().runtimeAnimatorController = animator;

            Debug.Log(updateLevel);
            _lv.GetComponent<TMP_Text>().text = updateLevel.ToString();
        }

        // private void OnDrawGizmos()
        // {
        //     var data = DataManager.instance.towerInfo[towerName];
        //     var distance = data.attackDistance;
        //     
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawSphere(transform.position, distance);
        // }
    }
}
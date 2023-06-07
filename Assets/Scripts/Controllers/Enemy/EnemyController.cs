using System;
using Core;
using Managers;
using Unity.Mathematics;
using UnityEngine;
using VO;

namespace Controllers
{
    public class EnemyController : MonoBehaviour
    {
        public int currentWayPointIdx { get; set; }

        public int level { get; set; }
        private int infoLevel => level - 1;

        private SpriteRenderer _image;
        private Animator _anim;
        public string enemyName { get; set; }
        private const string _hpPosName = "HpPosition";
        private const string _hpPrefabPath = "UI/HpCanvas";
        private const string _animPath = "Animation";
        private EnemyHp _hpUi;

        private int maxHp { get; set; }
        private int hp { get; set; }
        private int moveSpeed { get; set; }
        private int defense { get; set; }
        
        public Vector3 currentMovePoint => 
            GameManager.instance.wayPoint.GetWayPointPos(currentWayPointIdx);

        private void Awake()
        {
            var animContainer = transform.Find(_animPath);
            _image = animContainer.GetComponent<SpriteRenderer>();
            _anim = animContainer.GetComponent<Animator>();
            OnHpPoint();
        }

        private void Update()
        {
            _hpUi.HpUpdate(maxHp, hp);
            Move();
            if (CurrentPosReached())
            {
                UpdateCurrentPointIdx();
            }
        }

        private void OnHpPoint()
        {
            if (_hpUi != null) return;
            
            var hpPos = Utils.FindChild(gameObject, _hpPosName);
            if (hpPos == null) return;
            var obj = ResourceManager.instance.Instantiate(_hpPrefabPath, pos: hpPos.transform.position , parent: hpPos.transform);
            _hpUi = obj.GetComponent<EnemyHp>();
        }

        private void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position,
                currentMovePoint, moveSpeed * Time.deltaTime);
        }

        private bool CurrentPosReached()
        {
            float distance = (transform.position - currentMovePoint).magnitude;
            if (distance < 0.1f)
            {
                return true;
            }

            return false;
        }

        private void UpdateCurrentPointIdx()
        {
            int lastIdx = GameManager.instance.wayPoint.points.Count - 1;
            if (currentWayPointIdx < lastIdx)
            {
                currentWayPointIdx++;
            }
            else
            {
                Goal();
            }
        }

        private void Goal()
        {
            GameManager.instance.GoalEnemy();
            SpawnManager.instance.ReturnInstance(gameObject, PoolType.EnemyPool);
        }

        public void DealDamage(int damageReceived)
        {
            var offset = Mathf.Clamp(hp - damageReceived, 0, maxHp);
            
            if (offset == 0) Die();
            else hp = offset;
        }

        private void Die()
        {
            hp = maxHp;
            _hpUi.ResetHpUi();
            GameManager.instance.DieEnemy();
            SpawnManager.instance.ReturnInstance(gameObject, PoolType.EnemyPool);
        }

        public void SetEnemyBaseImageAndData()
        {
            var data = DataManager.instance.enemyInfo[enemyName];
            SetEnemyBaseImage(data);

            currentWayPointIdx = 0;
            moveSpeed = data.levels[infoLevel].moveSpeed;
            maxHp = data.levels[infoLevel].hp;
            hp = data.levels[infoLevel].hp;
            defense = data.levels[infoLevel].defense;
        }

        public void SetEnemyBaseImage(EnemyStat data)
        {
            if (data.imagePath == null || data.animPath == null) return;
            
            var sprite = ResourceManager.instance.Load<Sprite>(data.imagePath);
            _image.sprite = sprite;

            var animator = ResourceManager.instance.Load<RuntimeAnimatorController>(data.animPath);
            _anim.runtimeAnimatorController = animator;
        }
        
    }
}
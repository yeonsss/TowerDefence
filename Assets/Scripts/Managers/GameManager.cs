using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Controllers.MapEdit;
using Controllers.Rand;
using Core;
using UnityEngine;
using VO;
using static Utils;
using static Define;
using Random = UnityEngine.Random;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        private int _spawnTime = 1;
        // 그냥 벡터 배열로 할까??
        public WayPoint wayPoint;
        public EnvironmentMapController em;
        public TileMapController tm;
        
        public bool isRoundStart { get; set; } = false;
        public bool isGameEnd { get; set; } = false;
        public bool isWinState { get; set; } = false;

        public int stageNum { get; set; } = 1;
        public int roundNum { get; set; } = 0;
        public int lifePoint { get; set; } = stageLife;

        public int roundTotalEnemyCount { get; set; } = 0;
        public int goalEnemyCount { get; set; } = 0;
        public int killEnemyCount { get; set; } = 0;

        public int towerSpawnCost { get; set; } = 0;
        public int currentGold { get; set; } = 0;

        public Dictionary<string, int> upgradeDamageDict;
        public Dictionary<string, int> upgradeCostDict;
        public Dictionary<string, int> upgradeLevelDict;
        public Dictionary<Vector3, GameObject> isRandInTowerDict;
        
        public void UpgradeTowerDamage(string towerName)
        {
            if (currentGold < upgradeCostDict[towerName]) return;

            currentGold -= upgradeCostDict[towerName];
            upgradeLevelDict[towerName] += 1;
            upgradeDamageDict[towerName] += 1;
            upgradeCostDict[towerName] += 10;
        }

        public override void Init()
        {
            upgradeDamageDict = new Dictionary<string, int>();
            upgradeCostDict = new Dictionary<string, int>();
            upgradeLevelDict = new Dictionary<string, int>();
            isRandInTowerDict = new Dictionary<Vector3, GameObject>();
        }

        private void ResetUpgrade()
        {
            upgradeDamageDict.Clear();
            upgradeCostDict.Clear();
            upgradeLevelDict.Clear();
            
            var towerNames = DataManager.instance.towerInfo;
            
            foreach (var towerStat in towerNames)
            {
                upgradeLevelDict.Add(towerStat.Key, 1);
                upgradeDamageDict.Add(towerStat.Key, 0);
                upgradeCostDict.Add(towerStat.Key, 100);
            }
        }

        private void Update()
        {
            if (isGameEnd == true) return;
            
            if (isRoundStart == true)
            {
                // 라운드 종료 체크
                if (RoundEndCheck() == true)
                {
                    isRoundStart = false;
                    RoundStart();
                }
            }
        }

        private Vector3 GetSpawnPos()
        {
            var spawnPossiblePos = new List<Vector3>();
            
            foreach (var rand in isRandInTowerDict)
            {
                var randController = rand.Value.GetComponent<TowerRandController>();
                
                if (randController.isTowerIn == false)
                {
                    spawnPossiblePos.Add(rand.Key);
                }
            }

            if (spawnPossiblePos.Count < 1) return default;
            
            var randIdx = Random.Range(0, spawnPossiblePos.Count);
            return spawnPossiblePos[randIdx];
        }

        public void StageStart()
        {
            roundNum = 0;
            lifePoint = stageLife;
            towerSpawnCost = firstTowerSpawnCost;
            currentGold = firstStageGold;
            isGameEnd = false;
            isWinState = false;
            ResetUpgrade();
            StageLoad();
            RoundStart();
        }

        public void StageLoad()
        {
            var stageMapInfo = DataManager.instance.stageMapInfo[stageNum];
            
            // 웨이포인트 로드
            {
                var wp = ResourceManager.instance.Instantiate("Map/WayPoint");  
                wayPoint = wp.GetComponent<WayPoint>();
                wayPoint.SetRoundWayPoint(stageMapInfo.wayPoint.ToArray());
            }

            // 타일 로드
            {
                var tp = ResourceManager.instance.Instantiate("Map/TileMap", MAP_BASE_POS);
                tm = tp.GetComponent<TileMapController>();
                tm.TileGen();

                var tiles = tm.GetComponentsInChildren<SpriteRenderer>();

                Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

                for (int i = 0; i < stageMapInfo.tileSpriteNameList.Count; i++)
                {
                    var path = stageMapInfo.tileSpriteNameList[i];
                    if (path == "") continue;
                    
                    var resourcePath = ConvertResourcePath(path);
                    if (!sprites.ContainsKey(resourcePath))
                    {
                        var sprite = Resources.Load<Sprite>(resourcePath);
                        sprites.Add(resourcePath, sprite);
                    }

                    tiles[i].sprite = sprites[resourcePath];
                }
            }

            // 환경 로드
            {
                var ep = ResourceManager.instance.Instantiate("Map/EnvironmentMap", MAP_BASE_POS);
                em = ep.GetComponent<EnvironmentMapController>();
                em.TileGen();
                
                var tiles = em.GetComponentsInChildren<SpriteRenderer>();

                Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

                for (int i = 0; i < stageMapInfo.envSpriteNameList.Count; i++)
                {
                    var path = stageMapInfo.envSpriteNameList[i];
                    if (path == "") continue;
                    
                    var resourcePath = ConvertResourcePath(path);
                    if (!sprites.ContainsKey(resourcePath))
                    {
                        var sprite = Resources.Load<Sprite>(resourcePath);
                        sprites.Add(resourcePath, sprite);
                    }

                    tiles[i].sprite = sprites[resourcePath];
                }
            }

            // 스폰 위치 저장
            {
                isRandInTowerDict.Clear();
                var spawns = stageMapInfo.spawnPosList;
                
                foreach (var pos in spawns)
                {
                    var sPos = ResourceManager.instance.Instantiate("Map/SpawnPos");
                    sPos.transform.localPosition = pos;
                    isRandInTowerDict.Add(pos, sPos);
                }
            }
        }

        private void StageClear()
        {
            roundNum = 1;
            
            isGameEnd = true;
            isWinState = true;
            
            var prevData = DataManager.instance.stageCurrentInfo[stageNum];
            prevData.clear = true;
            
            var star = CalcClearStar();
            if (prevData.clearStarCount < star)
            {
                prevData.clearStarCount = star;
            }

            DataManager.instance.stageCurrentInfo[stageNum] = prevData;

            var nextStageNum = stageNum + 1;

            if (nextStageNum <= DataManager.instance.stageCurrentInfo.Count)
            {
                var nextStageData = DataManager.instance.stageCurrentInfo[nextStageNum];
                if (nextStageData.open == false)
                {
                    nextStageData.open = true;
                    nextStageData.clear = false;
                    nextStageData.clearStarCount = 0;
                }
                
                DataManager.instance.stageCurrentInfo[nextStageNum] = nextStageData;
            }
            
            DataManager.instance.UploadStageCurrent();
        }

        private int CalcClearStar()
        {
            if (stageLife == lifePoint)
            {
                return 3;
            }

            if (stageLife > lifePoint && lifePoint > stageLife * 0.7)
            {
                return 2;
            }
            
            return 1;
        }

        private void RoundStart()
        {
            Debug.Log($"RoundStart {roundNum}");
            var maxRoundCount = DataManager.instance.stageInfo[stageNum].round.Count;
            if (roundNum >= maxRoundCount)
            {
                // stage clear
                StageClear();
                return;
            }
            
            roundNum++;
            roundTotalEnemyCount = 0;
            
            var enemyInfos = DataManager.instance.stageInfo[stageNum].round[roundNum - 1].spawnEnemyList;
            foreach (var ei in enemyInfos)
            {
                roundTotalEnemyCount += ei.count;
            }
            
            goalEnemyCount = 0;
            killEnemyCount = 0;
            isRoundStart = true;
            // 코루틴으로 스폰 시작
            StartCoroutine(SpawnEnemyCor());
        }

        public void SetSpawnPos(Vector3 pos, bool inPos)
        {
            if (isRandInTowerDict.ContainsKey(pos))
            {
                var tr = isRandInTowerDict[pos].GetComponent<TowerRandController>();
                tr.isTowerIn = inPos;    
            }
        }

        private bool RoundEndCheck()
        {
            if (goalEnemyCount + killEnemyCount >= roundTotalEnemyCount)
            {
                return true;
            } 
            
            return false;
        }

        public string GetRandomTowerName()
        {
            // Random Tower Name
            var towerNames = DataManager.instance.towerInfo;
            if (towerNames.Count < 1)
                throw new Exception("no towerData");

            List<string> keys = new List<string>(towerNames.Keys);
            var randomIdx = Random.Range(0, towerNames.Count);

            return keys[randomIdx];
        }

        public void SpawnRandomTower()
        {
            if (currentGold < towerSpawnCost) return;

            var pos = GetSpawnPos();
            if (pos == default) return;

            var towerName = GetRandomTowerName();
            
            currentGold -= towerSpawnCost;
            towerSpawnCost += 10;
            SpawnManager.instance.SpawnTower(pos, towerName);
            SetSpawnPos(pos, true);
        }
        
        public void GoalEnemy()
        {
            lifePoint--;
            goalEnemyCount++;
            if (lifePoint <= 0)
            {
                isGameEnd = true;
                isWinState = false;
                Debug.Log("you lose");
            }
        }

        public void DieEnemy()
        {
            killEnemyCount++;
            currentGold += DataManager.instance.stageInfo[stageNum].killPoint;
        }

        public IEnumerator SpawnEnemyCor()
        {
            var spawnList = DataManager.instance.stageInfo[stageNum].round[roundNum-1].spawnEnemyList;

            foreach (var enemyInfo in spawnList)
            {
                for (int i = 0; i < enemyInfo.count; i++)
                {
                    var startPos = wayPoint.GetWayPointPos(0);
                    SpawnManager.instance.SpawnEnemy(startPos, enemyInfo.enemyName);
            
                    yield return new WaitForSeconds(_spawnTime);
                }
            }
        }
    }
}
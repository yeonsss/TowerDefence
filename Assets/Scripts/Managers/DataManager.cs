using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using VO;
using static Define;

namespace Managers
{
    public class DataManager : Core.Singleton<DataManager>
    {
        public Dictionary<string, TowerStat> towerInfo;
        public Dictionary<int, StageStat> stageInfo;
        public Dictionary<string, EnemyStat> enemyInfo;
        public Dictionary<int, StageMap> stageMapInfo;
        public Dictionary<int, ChapterStat> chapterInfo;
        public Dictionary<int, StageCurrentState> stageCurrentInfo;

        private bool _isInit = false;

        public T DataInitialize<T>(string path)
        {
            TextAsset tAsset = Resources.Load<TextAsset>(path);
            T dataMapJson = JsonConvert.DeserializeObject<T>(tAsset.text);
            return dataMapJson;
        }

        public override void Init()
        {
            if (_isInit == false)
            {
                towerInfo = new Dictionary<string, TowerStat>();
                stageInfo = new Dictionary<int, StageStat>();
                enemyInfo = new Dictionary<string, EnemyStat>();
                stageMapInfo = new Dictionary<int, StageMap>();
                chapterInfo = new Dictionary<int, ChapterStat>();
                stageCurrentInfo = new Dictionary<int, StageCurrentState>();
            
                TowerDataInit();
                EnemyDataInit();
                StageDataInit();
                StageMapDataInit();
                ChapterDataInit();
                StageCurrentDataInit();

                _isInit = true;
            }
        }

        public void UploadStageCurrent()
        {
            // var updateData = new StageStatus();
            // updateData.status = stageCurrentInfo.Values.ToList();
            //
            // string dataString = JsonConvert.SerializeObject(updateData);
            //
            // using (StreamWriter writer = new StreamWriter($"{stageCurrentDataPath}.json", false))
            // {
            //     writer.Write(dataString);
            // }
        }

        private void StageCurrentDataInit()
        {
            var data = DataInitialize<StageStatus>(STAGE_CURRENT_DATA_PATH);
            for (int i = 0; i < data.status.Count; i++)
            {
                stageCurrentInfo.Add(i + 1, data.status[i]);
            }
        }

        private void TowerDataInit()
        {
            var data = DataInitialize<Towers>(TOWER_DATA_PATH);
            foreach (var stat in data.towers)
            {
                towerInfo.Add(stat.towerName, stat);
            }
        }
        
        private void EnemyDataInit()
        {
            var data = DataInitialize<Enemies>(ENEMY_DATA_PATH);
            foreach (var stat in data.enemies)
            {
                enemyInfo.Add(stat.enemyName, stat);
            }
        }
        
        private void StageDataInit()
        {
            var data = DataInitialize<Stages>(STAGE_DATA_PATH);

            for (int i = 0; i < data.stages.Count; i++)
            {
                stageInfo.Add(i + 1, data.stages[i]);
            }
        }
        
        private void ChapterDataInit()
        {
            var data = DataInitialize<Chapters>(CHAPTER_DATA_PATH);

            for (int i = 0; i < data.chapters.Count; i++)
            {
                chapterInfo.Add(i + 1, data.chapters[i]);
            }
        }

        private void StageMapDataInit()
        {
            Object[] resources = Resources.LoadAll(STAGE_MAP_DATA_PATH, typeof(TextAsset));

            foreach (var textAsset in resources)
            {
                var asset = textAsset as TextAsset;
                if (asset != null)
                {
                    StageMap dataMapJson = JsonConvert.DeserializeObject<StageMap>(asset.text);
                    var elements = asset.name.Split("-");
                    if(elements.Length < 2) continue;
                    stageMapInfo.Add(int.Parse(elements[1]), dataMapJson);
                }
            }
        }
    }
}
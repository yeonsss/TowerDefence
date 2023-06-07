using System.Collections.Generic;

namespace VO
{
    public class Stages
    {
        public List<StageStat> stages { get; set; }
        
    }
    
    public class StageStat
    {
        public string stagePath { get; set; }
        public string stageName { get; set; }
        public int killPoint { get; set; }
        public List<RoundInfo> round { get; set; }
        
    }

    public class RoundInfo
    {
        public string roundName { get; set; }
        public List<StageEnemyInfo> spawnEnemyList { get; set; }
    }

    public class StageEnemyInfo
    {
        public string enemyName { get; set; }
        public int count { get; set; }
    }
}
using System.Collections.Generic;

namespace VO
{
    public class Towers
    {
        public List<TowerStat> towers { get; set; }
    }
    
    public class TowerStat
    {
        public string towerName { get; set; }
        public string projectileImagePath { get; set; }
        public float attackDistance { get; set; }
        public float projectileSpeed { get; set; }
        public string animPath { get; set; }
        public float attackCool { get; set; }
        public List<TowerLevelInfo> levels { get; set; }
    }

    public class TowerLevelInfo
    {
        public int level { get; set; }
        public int damage { get; set; }
        public string imagePath { get; set; }
    }
}
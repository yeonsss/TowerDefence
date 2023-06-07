using System.Collections.Generic;

namespace VO
{
    public class Enemies
    {
        public List<EnemyStat> enemies { get; set; }
    }
    
    public class EnemyStat
    {
        public string enemyName { get; set; }
        public string animPath { get; set; }
        public string imagePath { get; set; }
        public List<EnemyLevelInfo> levels { get; set; }
    }

    public class EnemyLevelInfo
    {
        public int level { get; set; }
        public int hp { get; set; }
        public int defense { get; set; }
        public int moveSpeed { get; set; }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace VO
{
    public class StageMap
    {
        public List<string> tileSpriteNameList { get; set; }
        public List<string> envSpriteNameList { get; set; }
        public List<Vector3> spawnPosList { get; set; }
        public List<Vector3> wayPoint { get; set; }
    }
}
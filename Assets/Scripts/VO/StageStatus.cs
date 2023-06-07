using System.Collections.Generic;

namespace VO
{
    public class StageStatus
    {
        public List<StageCurrentState> status { get; set; }
    }

    public class StageCurrentState
    {
        public bool open { get; set; }
        public bool clear { get; set; }
        public int clearStarCount { get; set; }
    }
}
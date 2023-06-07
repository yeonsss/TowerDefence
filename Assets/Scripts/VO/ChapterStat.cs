using System.Collections.Generic;

namespace VO
{
    public class Chapters
    {
        public List<ChapterStat> chapters { get; set; }
    }
    
    public class ChapterStat
    {
        public string chapterName { get; set; }
        public string chapterImagePath { get; set; }
        public string backColor { get; set; }
        public string backImageColor { get; set; }
        public int stageCount { get; set; }
        public List<int> includeStage { get; set; }
    }
}
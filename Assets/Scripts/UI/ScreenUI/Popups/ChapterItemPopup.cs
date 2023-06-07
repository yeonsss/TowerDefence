using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ScreenUI.Popups
{
    public class ChapterItemPopup : BaseUI
    {
        enum Texts
        {
            PointText,
            ChapterName
        }

        enum Images
        {
            ChapterImage
        }

        enum Sliders
        {
            ClearSlider
        }

        public int _chapterNum = 1;

        public override void Init()
        {
            Bind<TMP_Text>(typeof(Texts));
            Bind<Image>(typeof(Images));
            Bind<Slider>(typeof(Sliders));
        }

        public void SetData(int chapNum)
        {
            _chapterNum = chapNum;
            var stageCount = DataManager.instance.chapterInfo[chapNum].includeStage.Count;
            var chapInfo = DataManager.instance.chapterInfo[_chapterNum];

            Get<TMP_Text>((int)Texts.ChapterName).text = chapInfo.chapterName;
            var image = ResourceManager.instance.Load<Sprite>(chapInfo.chapterImagePath);
            if (image == null) return;
            Get<Image>((int)Images.ChapterImage).sprite = image;

            var clearStarCount = 0;
            foreach (var stageNum in chapInfo.includeStage)
            {
                clearStarCount += DataManager.instance.stageCurrentInfo[stageNum].clearStarCount;
            }

            Get<TMP_Text>((int)Texts.PointText).text = $"{clearStarCount} / {(stageCount * 3).ToString()}";
            Get<Slider>((int)Sliders.ClearSlider).value = Mathf.Clamp(clearStarCount / (stageCount * 3),0, 1);
        }
    }
}
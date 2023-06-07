using System;
using Managers;
using UI.ScreenUI.Popups;
using UnityEngine;
using UnityEngine.UI;
using static Utils;

namespace UI.ScreenUI
{
    public class MainUI : BaseUI
    {
        enum Images
        {
            BackColor,
            BackImage
        }

        enum Chapters
        {
            Chapters
        }

        enum Stages
        {
            Stages
        }
        
        private float _colorMoveSpeed = 0.5f;

        public override void Init()
        {
            Bind<Image>(typeof(Images));
            Bind<ChapterPopup>(typeof(Chapters));
            Bind<StagePopup>(typeof(Stages));

            GoChapterView();
        }

        public void Start()
        {
            if (DataManager.instance.chapterInfo.TryGetValue(1, out var chapData))
            {
                SetBackImage(chapData.backImageColor, chapData.backColor);
            }
        }

        public void GoChapterView()
        {
            Get<StagePopup>((int)Stages.Stages).gameObject.SetActive(false);
            Get<ChapterPopup>((int)Chapters.Chapters).gameObject.SetActive(true);
        }

        public void GoStageView(int chapNum)
        {
            Get<ChapterPopup>((int)Chapters.Chapters).gameObject.SetActive(false);
            Get<StagePopup>((int)Stages.Stages).gameObject.SetActive(true);
            Get<StagePopup>((int)Stages.Stages).GenStage(chapNum);
        }

        private void SetBackImage(string backImageColor, string backColor)
        {
            var backImageObj = Get<Image>((int)Images.BackImage);
            if (backImageColor == null) return;
            var backColorObj = Get<Image>((int)Images.BackColor);
            if (backColorObj == null) return;

            var biColor = GetCodeToColor(backImageColor);
            var bcColor = GetCodeToColor(backColor);

            backImageObj.color = biColor;
            backColorObj.color = bcColor;
        }

        public void SetBackImageSlerp(string backImageColor, string backColor)
        {
            // 천천히 바뀌도록
            var backImageObj = Get<Image>((int)Images.BackImage);
            var backColorObj = Get<Image>((int)Images.BackColor);

            var biColor = GetCodeToColor(backImageColor);
            var bcColor = GetCodeToColor(backColor);
            
            LeanTween.value(backImageObj.gameObject, UpdateBackImageColor, backImageObj.color, biColor, _colorMoveSpeed)
                .setEase(LeanTweenType.easeOutQuad);
            
            LeanTween.value(backColorObj.gameObject, UpdateBackColor, backColorObj.color, bcColor, _colorMoveSpeed)
                .setEase(LeanTweenType.easeOutQuad);
        }
        
        private void UpdateBackImageColor(Color color)
        {
            var backImageObj = Get<Image>((int)Images.BackImage);
            backImageObj.color = color;
        }
        
        private void UpdateBackColor(Color color)
        {
            var backColorObj = Get<Image>((int)Images.BackColor);
            backColorObj.color = color;
        }
        
    }
}
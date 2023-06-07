using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ScreenUI.Popups
{
    public class StagePopup : BaseUI
    {
        private int _stageMaxCount = 15;

        enum Buttons
        {
            BackButton
        }

        enum Texts
        {
            ChapterName
        }

        enum Grids
        {
            StageContent
        }

        public override void Init()
        {
            Bind<TMP_Text>(typeof(Texts));
            Bind<RectTransform>(typeof(Buttons));
            Bind<GridLayoutGroup>(typeof(Grids));
            
            Get<RectTransform>((int)Buttons.BackButton).gameObject.BindEvent(GoChapter);
        }

        public void GenStage(int chapNum)
        {
            Get<TMP_Text>((int)Texts.ChapterName).text = $"Chapter {chapNum.ToString()}";

            var content = Get<GridLayoutGroup>((int)Grids.StageContent);

            for (int i = 0; i < content.transform.childCount; i++)
            {
                var pastStage = content.transform.GetChild(0);
                ResourceManager.instance.Destroy(pastStage.gameObject);
            }
            
            var stages = DataManager.instance.chapterInfo[chapNum].includeStage;

            foreach (var stageNum in stages)
            {
                var stage = ResourceManager.instance.Instantiate("UI/Stage", parent: content.transform);
                stage.GetComponent<StageItemPopup>().SetState(stageNum);
            }
        }

        private void GoChapter()
        {
            transform.parent.GetComponent<MainUI>().GoChapterView();
        }
    }
}
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Define;
using Image = UnityEngine.UI.Image;

namespace UI.ScreenUI.Popups
{
    public class StageItemPopup : BaseUI
    {
        enum Texts
        {
            ClearStageNumber,
            StageNumber
        }

        enum Images
        {
            StageImage,
            LockStageImage
        }

        enum Grids
        {
            ClearStar
        }

        private StageState _state;
        private int _number;

        public override void Init()
        {
            Bind<Image>(typeof(Images));
            Bind<TMP_Text>(typeof(Texts));
            Bind<GridLayoutGroup>(typeof(Grids));
            
            gameObject.BindEvent(ClickEvt);
        }
        
        private void SetOpenState()
        {
            Get<Image>((int)Images.LockStageImage).gameObject.SetActive(false);
            Get<Image>((int)Images.StageImage).color = new Color32() {
                r = 91, g = 255, b = 66, a = 255
            };
            
            Get<GridLayoutGroup>((int)Grids.ClearStar).gameObject.SetActive(false);

            var num = Get<TMP_Text>((int)Texts.StageNumber);
            num.text = _number.ToString();
            num.gameObject.SetActive(true);
            
            Get<TMP_Text>((int)Texts.ClearStageNumber).gameObject.SetActive(false);
        }
        
        private void SetClearState()
        {
            Get<Image>((int)Images.LockStageImage).gameObject.SetActive(false);
            Get<Image>((int)Images.StageImage).color = new Color32() {
                r = 255, g = 255, b = 255, a = 255
            };

            var clearStar = Get<GridLayoutGroup>((int)Grids.ClearStar);
            clearStar.gameObject.SetActive(true);
            var stageState = DataManager.instance.stageCurrentInfo[_number];
            var star = clearStar.transform.GetChild(0);
            
            for (int i = 0; i < stageState.clearStarCount; i++)
            {
                ResourceManager.instance.Instantiate(star, parent: clearStar.transform);
            }
            
            ResourceManager.instance.Destroy(star.gameObject);

            Get<TMP_Text>((int)Texts.StageNumber).gameObject.SetActive(false);

            var num = Get<TMP_Text>((int)Texts.ClearStageNumber);
            num.text = _number.ToString();
            num.gameObject.SetActive(true);
        }
        
        private void SetLockState()
        {
            Get<Image>((int)Images.LockStageImage).gameObject.SetActive(true);
            Get<Image>((int)Images.StageImage).color = new Color32() {
                r = 0, g = 0, b = 0, a = 100
            };
            
            Get<GridLayoutGroup>((int)Grids.ClearStar).gameObject.SetActive(false);
            
            Get<TMP_Text>((int)Texts.StageNumber).gameObject.SetActive(false);
            Get<TMP_Text>((int)Texts.ClearStageNumber).gameObject.SetActive(false);
        }

        public void SetState(int stageNum)
        {
            _number = stageNum;
            var state = DataManager.instance.stageCurrentInfo[stageNum];
            
            if (state.open == false)
            {
                _state = StageState.Lock;
                SetLockState();
            }
            else if (state.open == true && state.clear == false)
            {
                _state = StageState.Open;
                SetOpenState();
            }
            else if (state.open == true && state.clear == true)
            {
                _state = StageState.Clear;
                SetClearState();
            }
        }

        private void ClickEvt()
        {
            switch (_state)
            {
                case StageState.Open :
                case StageState.Clear :
                    GameManager.instance.stageNum = _number;
                    C_SceneManager.instance.SwitchStageScene();
                    break;
                case StageState.Lock : 
                    break;
            }
        }

        
    }
}
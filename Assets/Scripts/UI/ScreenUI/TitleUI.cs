using System;
using Managers;
using UnityEngine.UI;

namespace UI.ScreenUI
{
    public class TitleUI : BaseUI
    {
        private enum Buttons
        {
            GameStartBtn
        }
        
        private enum Sliders
        {
            LoadingBar
        }

        public override void Init()
        {
            Bind<Button>(typeof(Buttons));
            Bind<Slider>(typeof(Sliders));
            
            Get<Button>((int)Buttons.GameStartBtn).gameObject.BindEvent(SwitchScene);
            Get<Slider>((int)Sliders.LoadingBar).gameObject.SetActive(false);
        }

        // public void SetLodingBarValue(float offset)
        // {
        //     Get<Slider>((int)Sliders.LoadingBar).value = offset;
        // }

        public void SwitchScene()
        {
            Get<Button>((int)Buttons.GameStartBtn).gameObject.SetActive(false);
            Get<Slider>((int)Sliders.LoadingBar).gameObject.SetActive(true);
            C_SceneManager.instance.SwitchMainScene();
        }
    }
}
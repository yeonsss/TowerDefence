using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ScreenUI
{
    public class AlertUI : BaseUI
    {

        enum Alerts
        {
            LoseAlert,
            WinAlert,
        }

        enum Buttons
        {
            LoseBtn,
            WinBtn
        }

        public override void Init()
        {
            Bind<Animator>(typeof(Alerts));
            Bind<Button>(typeof(Buttons));

            Get<Button>((int)Buttons.LoseBtn).gameObject.BindEvent(MoveTitleScene);
            Get<Button>((int)Buttons.WinBtn).gameObject.BindEvent(MoveTitleScene);

            SetAllAlertState(false);
        }

        private void Update()
        {
            var end = GameManager.instance.isGameEnd;
            var winState = GameManager.instance.isWinState;
            
            if (end == false) return;

            if (winState == true)
            {
                AlertWin();
            }
            else
            {
                AlertLose();
            }
        }

        private void SetAllAlertState(bool state)
        {
            Get<Animator>((int)Alerts.LoseAlert).gameObject.SetActive(state);
            Get<Animator>((int)Alerts.WinAlert).gameObject.SetActive(state);
        }

        private void MoveTitleScene()
        {
            SetAllAlertState(false);
            C_SceneManager.instance.SwitchMainScene();
        }

        private void AlertWin()
        {
            var winAlert = Get<Animator>((int)Alerts.WinAlert);
            winAlert.gameObject.SetActive(true);
            winAlert.Play("WinAlert");
        }
        
        private void AlertLose()
        {
            var loseAlert =  Get<Animator>((int)Alerts.LoseAlert);
            loseAlert.gameObject.SetActive(true);
            loseAlert.Play("LoseAlert");
        }
    }
}
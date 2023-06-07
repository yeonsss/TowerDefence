using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ScreenUI
{
    public class GameUI : BaseUI
    {
        enum Texts
        {
            GoldCount,
            SpawnCount,
            RoundName,
        }
        enum Buttons
        {
            SpawnBtn,
        }

        enum Panels
        {
            LifePoint,
            UpgradePanel
        }

        private int _roundNum = 0;
        private int _currentCost = 0;
        private int _spawnCost = 0;
        private int _lifePoint = 0;

        public override void Init()
        {
            Bind<Button>(typeof(Buttons));
            Bind<TMP_Text>(typeof(Texts));
            Bind<RectTransform>(typeof(Panels));
            
            _lifePoint = GameManager.instance.lifePoint;
            var panel = Get<RectTransform>((int)Panels.LifePoint);
            var childCount = panel.childCount;
            for (int i = 0; i < childCount; i++)
            {
                ResourceManager.instance.Destroy(panel.GetChild(panel.childCount - 1).gameObject);
            }

            for (int i = 0; i < _lifePoint; i++)
            {
                ResourceManager.instance.Instantiate("UI/Life", parent:panel);
            }

            var upgradePanel = Get<RectTransform>((int)Panels.UpgradePanel).transform;
            foreach (var towerInfo in DataManager.instance.towerInfo)
            {
                var btn = ResourceManager.instance.Instantiate("UI/UpgradeBtn", parent: upgradePanel);
                btn.GetComponent<UpgradeBtn>().SetData(towerInfo.Key);
                btn.gameObject.BindEvent(() =>
                {
                    GameManager.instance.UpgradeTowerDamage(towerInfo.Key);
                });
            }

            Get<Button>((int)Buttons.SpawnBtn).gameObject.BindEvent(SpawnBtnEvent);
            
            _roundNum = GameManager.instance.roundNum;
            Get<TMP_Text>((int)Texts.RoundName).text = $"Round {_roundNum.ToString()}";

            _spawnCost = GameManager.instance.towerSpawnCost;
            Get<TMP_Text>((int)Texts.SpawnCount).text = _spawnCost.ToString();

            _currentCost = GameManager.instance.currentGold;
            Get<TMP_Text>((int)Texts.GoldCount).text = _currentCost.ToString();
        }
        
        public void Update()
        {
            ChangeSpawnBtnText();
            ChangeGoldText();
            ChangeRoundName();
            ChangeLifePoint();
        }
        
        private void ChangeLifePoint()
        {
            var life = GameManager.instance.lifePoint;
            var deleteCount = _lifePoint - life;
            if (deleteCount < 1) return;
            
            if (life != _lifePoint)
            {
                var panel = Get<RectTransform>((int)Panels.LifePoint);
                if (panel.childCount < 1) return; 

                for (int i = 0; i < deleteCount; i++)
                {
                    ResourceManager.instance.Destroy(panel.GetChild(panel.childCount - 1).gameObject);
                }
                _lifePoint = life;
            }
        }

        private void ChangeSpawnBtnText()
        {
            var cost = GameManager.instance.towerSpawnCost;
            if (_spawnCost != cost)
            {
                _spawnCost = cost;
                Get<TMP_Text>((int)Texts.SpawnCount).text = _spawnCost.ToString();
            }
            
        }
        
        private void ChangeGoldText()
        {
            var cost = GameManager.instance.currentGold;
            if (_currentCost != cost)
            {
                _currentCost = cost;
                Get<TMP_Text>((int)Texts.GoldCount).text = _currentCost.ToString();    
            }
        }

        private void ChangeRoundName()
        {
            var num = GameManager.instance.roundNum;
            if (_roundNum != num)
            {
                _roundNum = num;
                Get<TMP_Text>((int)Texts.RoundName).text = $"Round {_roundNum.ToString()}";
            }
        }

        private void SpawnBtnEvent()
        {
            GameManager.instance.SpawnRandomTower();
        }
    }
}
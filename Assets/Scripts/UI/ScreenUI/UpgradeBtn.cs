using System;
using Managers;
using Mono.Cecil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ScreenUI
{
    public class UpgradeBtn : BaseUI
    {
        private string _towerName;
        private int _level;
        private int _cost;

        enum Images
        {
            Image,
        }

        enum Texts
        {
            LevelText,
            UpgradeCost
        }

        public override void Init()
        {
            Bind<Image>(typeof(Images));
            Bind<TMP_Text>(typeof(Texts));
        }

        private void Update()
        {
            UpdateCostText();
            UpdateLevelText();
        }

        private void UpdateCostText()
        {
            var currentCost = GameManager.instance.upgradeCostDict[_towerName];
            if (currentCost != _cost)
            {
                _cost = currentCost;
                Get<TMP_Text>((int)Texts.UpgradeCost).text = _cost.ToString();
            }
        }
        
        private void UpdateLevelText()
        {
            var currentLevel = GameManager.instance.upgradeLevelDict[_towerName];
            if (currentLevel != _level)
            {
                _level = currentLevel;
                Get<TMP_Text>((int)Texts.LevelText).text = $"LV {_level}";
            }
        }
        

        public void SetData(string towerName)
        {
            _towerName = towerName;
            var imagePath = DataManager.instance.towerInfo[towerName].levels[0].imagePath;
            Get<Image>((int)Images.Image).sprite = Resources.Load<Sprite>(imagePath);
        }
        
    }
}
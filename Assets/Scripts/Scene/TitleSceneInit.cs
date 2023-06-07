using System;
using Managers;
using UnityEngine;

namespace Scene
{
    public class TitleSceneInit : MonoBehaviour
    {
        private void Awake()
        {
            DataManager.instance.Init();
            Debug.Log(DataManager.instance.chapterInfo[1]);
        }
    }
}
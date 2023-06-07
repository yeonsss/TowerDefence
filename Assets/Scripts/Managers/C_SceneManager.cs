using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class C_SceneManager : Singleton<C_SceneManager>
    {
        public string m_currentScene;
        private Slider LoadingGuage;

        private List<string> SceneList = new List<string>();

        public override void Init()
        {
            GetSceneList();
        }

        private void GetSceneList()
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                SceneList.Add(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));
            }
        }

        private GameObject SwitchLoadingUI()
        {
            GameObject currentUI = GameObject.FindGameObjectWithTag("UI");
            currentUI.SetActive(false);

            var loadingUI = ResourceManager.instance.Instantiate("UI/LoadingUI");
            return loadingUI;
        }
        
        public void SwitchMainScene()
        {
            m_currentScene = "MainScene";
            GameObject loading = SwitchLoadingUI();

            LoadingGuage = loading.transform.Find("LoadingBar").GetComponent<Slider>();
            StartCoroutine(LoadSceneProcrss());
        }

        public void SwitchTitleScene()
        {
            m_currentScene = "TitleScene";
            SceneManager.LoadSceneAsync(m_currentScene);
        }
        
        public void SwitchStageScene()
        {
            m_currentScene = "StageScene";
            GameObject loading = SwitchLoadingUI();

            LoadingGuage = loading.transform.Find("LoadingBar").GetComponent<Slider>();
            StartCoroutine(LoadSceneProcrss());
        }

        IEnumerator LoadSceneProcrss()
        {
            yield return null;
            AsyncOperation op = SceneManager.LoadSceneAsync(m_currentScene);
            // 씬이 다 불러와졌을 떄 자동으로 이동할 것 인지
            // false면 90까지만 하고, true로 바꿔줄 때 나머지 10퍼센트 하면서 씬 전환
            // 게임이 커지면 에셋 번들로 리소스를 나눠서 로드해야한다.
            op.allowSceneActivation = false;

            float timer = 0f;

            while (!op.isDone)
            {
                if (op.progress < 0.9f)
                {
                    // 첫 90퍼센트는 progress에 맞춰서 바꿔주고
                    LoadingGuage.value = op.progress;
                }
                else
                {
                    // 나머지 10퍼센트는 페이크 로딩을 사용해 바꿔준다.
                    // timer += Time.unscaledDeltaTime;
                    timer += Time.deltaTime;
                    LoadingGuage.value = Mathf.Lerp(0.9f, 1f, timer);
                    if (LoadingGuage.value >= 1f)
                    {
                        op.allowSceneActivation = true;
                        yield break;
                        // Debug.Log("loading done");
                    }
                }
                yield return null;
            }
        }
    }
}
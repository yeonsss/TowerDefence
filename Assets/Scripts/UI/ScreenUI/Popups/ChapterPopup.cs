using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using static Utils;

namespace UI.ScreenUI.Popups
{
    public class ChapterPopup : BaseUI
    {
        enum Elements
        {
            ChapterContent,
            Chapters,
            ChapterPanel,
        }

        private float _screenWidth;
        private float _contentWidth;  // 콘텐츠 전체 길이
        private Vector2 _touchStartPosition;  // 터치 시작 위치
        public int _currentChapterViewNum = 0;
        private List<float> _chapterPos;
        private List<Transform> _chapterIdx;
        private float _minMoveOffset = 300f;
        private float _moveChapterSpeed = 0.5f;

        public override void Init()
        {
            _chapterPos = new ();
            _chapterIdx = new();
            
            Bind<RectTransform>(typeof(Elements));
            Get<RectTransform>((int)Elements.ChapterContent).anchoredPosition = Vector2.zero;

            SetContentWidth();
            SetChapterIdx();
            SetChapterItem();

            SetHighlightChapterIdx(_currentChapterViewNum, true);
        }
        
        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
            
                if (touch.phase == TouchPhase.Began)
                {
                    _touchStartPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    // 터치 이동 거리 계산
                    float touchDelta = touch.position.x - _touchStartPosition.x;
                    var content = Get<RectTransform>((int)Elements.ChapterContent);

                    if (Mathf.Abs(touchDelta) < _minMoveOffset)
                    {
                        MoveCurrentChapter(content);
                        return;
                    };

                    if (touchDelta > 0)
                    {
                        if (_currentChapterViewNum == 0) return;
                        MoveChapter(content, true);
                    }
                    else if (touchDelta < 0)
                    {
                        if (_currentChapterViewNum + 1 == _chapterPos.Count) return;
                        MoveChapter(content, false);
                    }
                }
            }
        }

        private void MoveChapter(RectTransform content, bool isLeft)
        {
            SetHighlightChapterIdx(_currentChapterViewNum, false);
            _currentChapterViewNum += isLeft == true ? -1 : 1 ;
            SetHighlightChapterIdx(_currentChapterViewNum, true);

            MoveCurrentChapter(content);
        }

        private void MoveCurrentChapter(RectTransform content)
        {
            if (DataManager.instance.chapterInfo.TryGetValue(_currentChapterViewNum + 1, out var chapData))
            {
                transform.parent.GetComponent<MainUI>().SetBackImageSlerp(chapData.backImageColor, chapData.backColor);

                // LeanTween https://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
                LeanTween.moveX(content, _chapterPos[_currentChapterViewNum], _moveChapterSpeed)
                    .setEase(LeanTweenType.easeOutSine);
            }
        }

        private void SetChapterItem()
        {
            var chapterCount = DataManager.instance.chapterInfo.Count;
            var content = Get<RectTransform>((int)Elements.ChapterContent);
            
            for (int i = 0; i < chapterCount; i++)
            {
                var chapter = ResourceManager.instance.Instantiate("UI/Chapter", parent:content);
                var cc = chapter.GetComponent<ChapterItemPopup>();
                cc.SetData(i + 1);
                chapter.BindEvent(() =>
                {
                    transform.parent.GetComponent<MainUI>().GoStageView(cc._chapterNum);
                });
            }
        }

        private void SetHighlightChapterIdx(int idxNum, bool onOffState)
        {
            if (_chapterIdx.Count < 1) return;
            var idx = _chapterIdx[idxNum];
            
            if (onOffState == true)
            {
                idx.GetComponent<Image>().color = new Color32() { r = 255, g = 255, b = 255, a = 255};
                idx.localScale = new Vector3() { x = 1.2f, y = 1.2f };
            }
            else
            {
                idx.GetComponent<Image>().color = new Color32() { r = 255, g = 255, b = 255, a = 100};
                idx.localScale = new Vector3() { x = 1.0f, y = 1.0f };
            }
        }

        private void SetChapterIdx()
        {
            var chapterCount = DataManager.instance.chapterInfo.Count;
            var chapterPanel = Get<RectTransform>((int)Elements.ChapterPanel);
            var idxObj = chapterPanel.transform.GetChild(0);

            for (int i = 0; i < chapterCount; i++)
            {
                var idx = ResourceManager.instance.Instantiate(idxObj, parent: chapterPanel);
                _chapterIdx.Add(idx);
            }

            ResourceManager.instance.Destroy(idxObj.gameObject);
        }
        
        private void SetContentWidth()
        {
            // 현재 화면 넓이
            _screenWidth = Screen.width;

            var chapterCount = DataManager.instance.chapterInfo.Count;
            
            // 챕터 개수만큼 곱하기
            _contentWidth = _screenWidth * chapterCount;
            var content = Get<RectTransform>((int)Elements.ChapterContent);
            Vector2 newSize = content.sizeDelta;
            newSize.x = _contentWidth;
            content.sizeDelta = newSize;

            for (int i = 0; i < chapterCount; i++)
            {
                _chapterPos.Add(i * _screenWidth * -1);
            }
            
            var grid = content.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2()
            {
                x = _screenWidth,
                y = _screenWidth
            };
        }
    }
}

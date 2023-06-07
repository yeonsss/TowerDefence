using System;
using Controllers.Rand;
using Controllers.Tower;
using UnityEngine;
using UnityEngine.EventSystems;
using static Utils;
using static Define;

namespace Managers
{
    public class InputManager : Core.Singleton<InputManager>
    {
        private bool _isDragging;
        private Vector3 _prevPos;
        private GameObject touchObj;
        private TowerController touchObjController;
        private Ray _ray;
        private RaycastHit _hit;
        private RaycastHit[] _hits;
        private int _towerLayer;
        private int _spawnPosLayer;

        enum InputState
        {
            TowerMove,
            Merge
        }

        public override void Init()
        {
            _isDragging = false;
            touchObj = null;
            _hits = new RaycastHit[2];
            _towerLayer = GetLayerNumber((int)LayerNames.Tower);
            _spawnPosLayer = GetLayerNumber((int)LayerNames.SpawnPos);
        }
        
        private void Update()
        {
            if (Input.touchCount >= 1)
            {
                if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {  
                    return;
                }    
            }
            
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _ray = Camera.main.ScreenPointToRay(touch.position);
                        if (Physics.Raycast(_ray, out _hit, 1000f, _towerLayer))
                        {
                            _isDragging = true;
                            touchObj = _hit.transform.gameObject;
                            touchObjController = touchObj.GetComponent<TowerController>();
                            _prevPos = touchObjController.containerTransform.position;
                            // Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
                        }
                        // else
                        // {
                        //     Vector3 endPosition = ray.origin + ray.direction * 1000;
                        //     Debug.DrawLine(ray.origin, endPosition, Color.red, 1f);
                        // }
                        break;
                    
                    case TouchPhase.Stationary:
                    case TouchPhase.Moved:
                        if (_isDragging != true) break;
                        Vector3 newPosition = Camera.main.ScreenToWorldPoint(touch.position);
                        newPosition.z = touchObj.transform.position.z;
                        touchObjController.containerTransform.position = newPosition;
                        // touchObj.transform.position = newPosition;
                        break;

                    case TouchPhase.Ended:
                        // 타워 옮기는 거는 일단 레이케스팅이 타워 그림만 되는 거라
                        // 특히나 게임 메니저에서만 위치를 갖고 있는거라
                        // 어떻게 해야 할까
                        // 여기 진입했을 때 아무것도 없었다면 
                        // 아니다 에초에 옮기는 걸 할려면 메모리에서만 갖고 있는 위치를
                        // 따로 게임 오브젝트로 관리해야 한다. 그래야 레이케스팅이 되니까
                        _isDragging = false;
                        _ray = Camera.main.ScreenPointToRay(touch.position);

                        Array.Clear(_hits, 0, _hits.Length);
                        var size = Physics.RaycastNonAlloc(_ray.origin, _ray.direction, _hits, 1000f, _towerLayer);
                        try
                        {
                            if (size > 0)
                            {
                                foreach (var hit in _hits)
                                {
                                    // RaycastHit이 null인지??
                                    if (hit.collider.gameObject == touchObj)
                                    {
                                        if (touchObjController == null) return;
                                        touchObjController.containerTransform.position = _prevPos;
                                        continue;
                                    }
                                    
                                    if (hit.collider == null && hit.distance == 0 && hit.normal == Vector3.zero) continue;
                                
                                    if (hit.transform.CompareTag("Tower") && 
                                        hit.transform.gameObject != touchObj)
                                    {
                                        if (touchObj == null) break;
                                        var origin = touchObj.GetComponent<TowerController>();
                                        var target = hit.transform.GetComponent<TowerController>();
                                    
                                        if (origin.level == target.level && origin.towerName == target.towerName)
                                        {
                                            GameManager.instance.SetSpawnPos(touchObj.transform.position, false);
                                            // 여기서 타워 풀에 넣는 것도 체크
                                            SpawnManager.instance.DeleteTower(touchObj);
                                            touchObjController = null;
                                            touchObj = null;
                                            _prevPos = default;
                                            target.Merge();
                                            break;                                            
                                        }
                                        throw new Exception("not match level");
                                    }
                                }
                                break;
                            }

                            if (Physics.Raycast(_ray, out var hitInfo, 1000f, _spawnPosLayer))
                            {
                                Debug.Log(hitInfo.transform.name);
                                var tr = hitInfo.transform.GetComponent<TowerRandController>();

                                if (tr.isTowerIn == false)
                                {
                                    touchObjController.containerTransform.position = _prevPos;
                                    GameManager.instance.SetSpawnPos(touchObj.transform.position, false);
                                    GameManager.instance.SetSpawnPos(hitInfo.transform.position, true);
                                    
                                    touchObj.transform.position = hitInfo.transform.position;
                                    
                                    touchObjController = null;
                                    touchObj = null;
                                }
                            }
                            else
                            {
                                throw new Exception("is not spawnPos");
                            }
                        }
                        catch (Exception e)
                        {
                            if (touchObjController != null)
                            {
                                touchObjController.containerTransform.position = _prevPos;
                                touchObjController = null;
                                touchObj = null;
                            }
                        }
                        break;

                    case TouchPhase.Canceled:
                        _isDragging = false;
                        touchObjController.containerTransform.position = _prevPos;
                        break;
                }
            }
        }
    }
}
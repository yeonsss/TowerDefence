using System;
using System.Collections.Generic;
using System.IO;
using Controllers;
using Controllers.MapEdit;
using Core;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VO;
using static Define;

namespace Editor
{
    public class MapEditTool : EditorWindow
    {
        private TileMapController _map;
        private Vector2 _scrollPos = Vector2.zero;
        private string _saveMapFileName = "stage-1";
        private string _tileName = "grass";
        private string spritePathForEmptyTile = "Assets/Resources/Sprites/Tiles/Normal/";
        
        private WindowStyle _style;

        [MenuItem("Tools/Tile Edit")]
        public static void ShowWindow()
        {
            GetWindow<MapEditTool>("Map Editor");
        }

        private void OnEnable()
        {
            _style = new WindowStyle();
            minSize = new Vector2(400, 600);

            SetTileMap();
        }

        private void CreateBtn(string btnTitle, Action func)
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(_style.GetStyle(Styles.Block));
            if (GUILayout.Button(btnTitle, GUILayout.Height(30)))
            {
                func.Invoke();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            // GUILayout.BeginHorizontal();

            CreateBtn("타일맵 객체 설정", SetTileMap);
            
            GUILayout.Space(10);
            GUILayout.Label("채울 타일 이름 지정", EditorStyles.boldLabel);
            GUILayout.Space(5);
            _tileName = EditorGUILayout.TextField("Tile name", _tileName);
            CreateBtn("빈 타일 채우기", FillEmptyTile);
            
            CreateBtn("타일맵 만들기", CreateTileMapAndTiles);
            
            CreateBtn("환경 타일맵 만들기", CreateEnvironmentTileMap);
            
            CreateBtn("타워 스폰지역 타일맵 만들기", CreateTowerSpawnTileMap);
            
            CreateBtn("웨이포인트 만들기", CreateWayPoint);

            CreateBtn("모든 타일 오브젝트 삭제", DeleteTileAll);

            CreateBtn("모든 타일 스프라이트 제거", ClearTileAll);

            GUILayout.Space(10);
            GUILayout.Label("맵 정보를 저장할 파일 이름 설정", EditorStyles.boldLabel);
            GUILayout.Space(5);
            _saveMapFileName = EditorGUILayout.TextField("file name", _saveMapFileName);
            CreateBtn("맵 검증 및 저장 (씬 구성 요소 확인 및 검증)", MapCheck);

            EditorGUILayout.EndScrollView();
        }

        private void FillEmptyTile()
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{spritePathForEmptyTile}{_tileName}.png");
            Debug.Log($"{spritePathForEmptyTile}{_tileName}");
            
            foreach (var tile in _map.tiles)
            {
                var currentTileSprite = tile.gameObject.GetComponent<SpriteRenderer>();
                if (currentTileSprite.sprite == null)
                {
                    currentTileSprite.sprite = sprite;    
                }
            }
        }

        private void MapCheck()
        {
            if (string.IsNullOrEmpty(_saveMapFileName)) return;
            
            // 카메라 체크
            var camera = GameObject.FindWithTag("MainCamera");
            if (camera == null) Debug.LogError("메인 카메라가 필요합니다.");
            
            //웨이포인트 체크
            var wayPoint = GameObject.Find(wayPointObjName);
            if (wayPoint == null) Debug.LogError("WayPoint가 필요합니다.");
            
            // 타일맵 체크
            var tileMap = GameObject.Find(tileMapObjName);
            if (tileMap == null) Debug.LogError("타일맵이 필요합니다.");
            var tileSpriteList = new List<string>();
            
            var tiles = tileMap.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sp in tiles)
            {
                string assetPath = AssetDatabase.GetAssetPath(sp.sprite);
                tileSpriteList.Add(assetPath);
            }
            
            // 환경 타일맵 체크
            // 이때 스프라이트 확인해서 none이 아닌 녀석은 active로 두고
            // none인 녀석은 active를 false로 한다.
            var envMap = GameObject.Find(enviroMapObjName);
            if (envMap == null) Debug.LogError("환경타일맵이 필요합니다.");
            var envSpriteList = new List<string>();
            var envs = envMap.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sp in envs)
            {
                string assetPath = AssetDatabase.GetAssetPath(sp.sprite);
                envSpriteList.Add(assetPath);
            }

            // 타워 스폰구역 체크
            // 이때 스프라이트 확인해서 none이 아닌 녀석은 active로 두고
            // none인 녀석은 active를 false로 한다.
            var spawnMap = GameObject.Find(towerSpawnMapObjName);
            if (spawnMap == null) Debug.LogError("스폰맵이 필요합니다.");
            var spawnPosList = new List<Vector3>();
            var spawns = spawnMap.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sp in spawns)
            {
                if (sp.sprite != null)
                {
                    spawnPosList.Add(sp.transform.position);
                }
            }
            
            Debug.Log(wayPoint.GetComponent<WayPoint>().points[1]);

            var mapInfo = new StageMap()
            {
                wayPoint = wayPoint.GetComponent<WayPoint>().points,
                tileSpriteNameList = tileSpriteList,
                envSpriteNameList = envSpriteList,
                spawnPosList = spawnPosList,
            };
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            
            string jsonString = JsonConvert.SerializeObject(mapInfo, settings);
            Debug.Log(jsonString);

            using (StreamWriter writer = new StreamWriter($"{stageMapDataPath}/{_saveMapFileName}.json", false))
            {
                writer.Write(jsonString);
            }
        }

        private void ClearTileAll()
        {
            Debug.Log("Clear tile");
            foreach (var tile in _map.tiles)
            {
                tile.gameObject.GetComponent<SpriteRenderer>().sprite = null;
            }
        }

        private void DeleteTileAll()
        {
            _map.DeleteAllTile();
        }

        private void SetTileMap()
        {
            var obj = GameObject.Find(tileMapObjName);
            if (obj == null)
            {
                Debug.Log("is not correct name");
                return;
            }

            var tileMapController = obj.GetComponent<TileMapController>();
            if (tileMapController == null)
            {
                Debug.Log("Component does not exist.");
                return;
            }

            _map = tileMapController;
        }

        private void CreateTileMapAndTiles()
        {
            var obj = GameObject.Find(tileMapObjName);
            if (obj == null)
            {
                obj = new GameObject()
                {
                    name = tileMapObjName
                };
                obj.transform.position = MAP_BASE_POS;
                obj.layer = (int)LayerNames.Tile;
                var tileMapController = Utils.GetOrAddComponent<TileMapController>(obj);
                tileMapController.TileGen();
            }
            else
            {
                Debug.Log("Please Delete TileMap Object");
            }
        }
        
        private void CreateEnvironmentTileMap()
        {
            var obj = GameObject.Find(enviroMapObjName);
            if (obj == null)
            {
                obj = new GameObject()
                {
                    name = enviroMapObjName
                    
                };
                obj.transform.position = MAP_BASE_POS;
                obj.layer = (int)LayerNames.Environment;
                var tileMapController = Utils.GetOrAddComponent<EnvironmentMapController>(obj);
                tileMapController.TileGen();
            }
            else
            {
                // 환경타일맵이 이미 있다면 active가 false인 자식들을 true로 바꿈.
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    Transform child = obj.transform.GetChild(i);
                    if (child.gameObject.activeSelf == false)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void CreateTowerSpawnTileMap()
        {
            var obj = GameObject.Find(towerSpawnMapObjName);
            if (obj == null)
            {
                obj = new GameObject()
                {
                    name = towerSpawnMapObjName
                    
                };
                obj.transform.position = MAP_BASE_POS;
                obj.layer = (int)LayerNames.TowerSpawn;
                var tileMapController = Utils.GetOrAddComponent<TowerSpawnMapController>(obj);
                tileMapController.TileGen();
            }
            else
            {
                // 스폰타일맵이 이미 있다면 active가 false인 자식들을 true로 바꿈.
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    Transform child = obj.transform.GetChild(i);
                    if (child.gameObject.activeSelf == false)
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void CreateWayPoint()
        {
            var obj = GameObject.Find(wayPointObjName);
            if (obj == null)
            {
                obj = new GameObject()
                {
                    name = wayPointObjName
                };

                Utils.GetOrAddComponent<WayPoint>(obj);
                
            }
            else
            {
                Debug.Log("Please Delete WayPoint Object");
            }
        }
    }
}
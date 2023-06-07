using System;
using Controllers;
using Controllers.MapEdit;
using UnityEditor;
using UnityEngine;
using static Define;
using static Utils;

namespace Editor
{
    [CustomEditor(typeof(TileMapController))]
    public class TileMapEditor : UnityEditor.Editor
    {
        private TileMapController _map => target as TileMapController;
        private RaycastHit[] _hits;
        private void OnEnable()
        {
            _map.TileGen();
        }

        public void OnSceneGUI()
        {
            if (Selection.activeObject.name != _map.name)
            {
                return;
            }

            EventType eventType = Event.current.type;

            switch (eventType)
            {
                case EventType.DragPerform :
                    DragAndDrop.AcceptDrag();
                    var mousePosition = Event.current.mousePosition;
                    var textureName = "";

                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is not Texture2D texture) continue;
                        textureName = draggedObject.name;

                        if (!Enum.TryParse(typeof(TileNames), textureName, out var result))
                        {
                            Debug.Log("Tiles 폴더 안에 있는 타일 스프라이트만 가능합니다.");
                            continue;
                        }

                        string assetPath = AssetDatabase.GetAssetPath(draggedObject);
                        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                        
                        var ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                        var layerNum = GetLayerNumber((int)LayerNames.Tile);
                        if (Physics.Raycast(ray, out var hit, 1000.0f, layerNum))
                        {
                            Debug.Log(hit.transform.parent.name);
                            hit.transform.GetComponent<SpriteRenderer>().sprite = sprite;
                        }
                    }
                    
                    var obj = GameObject.Find(textureName);
                    Debug.Log(obj);
                    if (obj != null)
                    {
                        DestroyImmediate(obj);
                        obj = null;
                    }
                    Event.current.Use();
                    break;
            }
        }
    }
}
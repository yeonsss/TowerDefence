using System;
using Controllers;
using Controllers.MapEdit;
using UnityEditor;
using UnityEngine;
using static Define;
using static Utils;

namespace Editor
{
    [CustomEditor(typeof(EnvironmentMapController))]
    public class EnvironmentMapEditor : UnityEditor.Editor
    {
        private EnvironmentMapController _map => target as EnvironmentMapController;
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

                        if (!Enum.TryParse(typeof(EnvironmentTileNames), textureName, out var result))
                        {
                            Debug.Log("Environment 폴더 안에 있는 타일 스프라이트만 가능합니다.");
                            continue;
                        }
                        
                        string assetPath = AssetDatabase.GetAssetPath(draggedObject);
                        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                        
                        var ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                        var layerNum = GetLayerNumber((int)LayerNames.Environment);
                        if (Physics.Raycast(ray, out var hit, 1000, layerNum))
                        {
                            
                            hit.transform.GetComponent<SpriteRenderer>().sprite = sprite;
                            Debug.Log(hit.transform.GetComponent<SpriteRenderer>().sprite);
                        }
                        else
                        {
                            Debug.Log("what the...");
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
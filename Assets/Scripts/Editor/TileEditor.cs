using Controllers;
using Controllers.MapEdit;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TileController))]
    public class TileEditor : UnityEditor.Editor
    {
        private TileController tile => target as TileController;
        public void OnSceneGUI()
        {
            EventType eventType = Event.current.type;

            switch (eventType)
            {
                case EventType.MouseDown :
                    // Right Click
                    if (Event.current.button == 1)
                    {
                        tile.GetComponent<SpriteRenderer>().sprite = null;
                    }
                    break;
            }
        }
    }
}
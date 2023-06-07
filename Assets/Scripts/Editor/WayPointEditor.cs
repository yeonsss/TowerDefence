using System;
using Core;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(WayPoint))]
    public class WayPointEditor : UnityEditor.Editor
    {
        private WayPoint _wayPoint => target as WayPoint;

        private void OnSceneGUI()
        {
            Handles.color = Color.red;
            if (_wayPoint.points.Count < 1) return;
            for (int i = 0; i < _wayPoint.points.Count; i++)
            {
                // Create Handles
                var pos = _wayPoint.currentPos + _wayPoint.points[i];
                var newWayPoint = Handles.FreeMoveHandle(
                    pos, Quaternion.identity, 0.5f, 
                    new Vector3(0.3f, 0.3f, 0.3f), Handles.SphereHandleCap);
                
                // Create text;
                GUIStyle textStyle = new GUIStyle()
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 14,
                    normal =
                    {
                        textColor = Color.cyan
                    },
                };

                var textAlligment = Vector3.down * 0.35f + Vector3.right * 0.35f;
                Handles.Label(pos + textAlligment, $"{i + 1}", textStyle);

                EditorGUI.EndChangeCheck();

                // 매 프레임마다 에디터 수정이 끝났다고 판단되면
                // 수정된 위치를 WayPoint에 기록
                if (EditorGUI.EndChangeCheck())
                {
                    // 웨이포인트를 잘못 수정했을 때 되돌리기위해 
                    // WayPoint를 Undo스택에 기록
                    Undo.RecordObject(target, "Free Move Handle");
                    _wayPoint.points[i] = newWayPoint - _wayPoint.currentPos;
                }
            }
        }
    }
}
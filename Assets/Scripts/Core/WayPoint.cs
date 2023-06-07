using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Core
{
    public class WayPoint : MonoBehaviour
    {
        [SerializeField]
        private List<Vector3> _points;

        public List<Vector3> points => _points;
        public Vector3 currentPos => _currentPosition;
        
        private Vector3 _currentPosition;
        private bool _gameStarted;

        private void Start()
        {
            _gameStarted = true;
            _currentPosition = transform.position;
        }

        public void ClearWayPoint()
        {
            _points.Clear();
        }

        public void SetRoundWayPoint(Vector3[] roundWayPos)
        {
            _points = new List<Vector3>(roundWayPos);
        }

        public Vector3 GetWayPointPos(int idx)
        {
            if (_points.Count > idx)
                return _currentPosition + _points[idx];
            return Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            if (!_gameStarted && transform.hasChanged)
            {
                _currentPosition = transform.position;
            }
            
            for (int i = 0; i < _points.Count; i++)
            {
                var currentPos = _points[i] + _currentPosition;

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(currentPos, 0.5f);

                if (i < _points.Count-1)
                {
                    var nextPos = _points[i+1] + _currentPosition;
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(currentPos, nextPos);
                    DrawArrow(currentPos, nextPos);
                }
            }
        }

        private void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            var midPos = new Vector3(
                pos.x + ((direction.x - pos.x) / 2),
                pos.y + ((direction.y - pos.y) / 2),
                0);
            
            Gizmos.color = Color.green;

            var offset = midPos - direction;
            var degree = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

            var leftArrowPos = new Vector3(
                midPos.x + Mathf.Cos((degree + arrowHeadAngle) * Mathf.Deg2Rad) * arrowHeadLength, 
                midPos.y + Mathf.Sin((degree + arrowHeadAngle) * Mathf.Deg2Rad) * arrowHeadLength, 0);
            
            var rightArrowPos = new Vector3(
                midPos.x + Mathf.Cos((degree - arrowHeadAngle) * Mathf.Deg2Rad) * arrowHeadLength, 
                midPos.y + Mathf.Sin((degree - arrowHeadAngle) * Mathf.Deg2Rad) * arrowHeadLength, 0);
            
            Gizmos.DrawLine(midPos, leftArrowPos);
            Gizmos.DrawLine(midPos, rightArrowPos);
        }
    }
}
using System;
using UnityEngine;

namespace Controllers.Rand
{
    public class TowerRandController : MonoBehaviour
    {
        public bool isTowerIn;

        private void Awake()
        {
            isTowerIn = false;
        }
    }
}
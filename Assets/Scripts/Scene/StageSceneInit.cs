using Managers;
using UnityEngine;

namespace Scene
{
    public class StageSceneInit : MonoBehaviour
    {
        private void Start()
        {
            SpawnManager.instance.ObjectPoolClear();
            InputManager.instance.Init();
            GameManager.instance.StageStart();
        }
    }
}
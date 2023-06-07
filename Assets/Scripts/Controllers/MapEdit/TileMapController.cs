using UnityEngine;
using static Define;
using static Utils;


namespace Controllers.MapEdit
{
    public class TileMapController : MonoBehaviour
    {
        [SerializeField] private bool _isTileGen = false;

        public bool isTileGen => _isTileGen;
        public TileController[] tiles => GetComponentsInChildren<TileController>();

        public virtual void MakeTile(Vector3 tilePos)
        {
            GameObject tile = new GameObject();
            tile.layer = (int)LayerNames.Tile;
            tile.transform.parent = transform;
            tile.transform.localPosition = new Vector3(tilePos.x, tilePos.y, tilePos.z);
                    
            GetOrAddComponent<TileController>(tile);
            var sprite = GetOrAddComponent<SpriteRenderer>(tile);
            sprite.sortingOrder = (int)SortingLayerNum.Tile;
            var col = GetOrAddComponent<BoxCollider>(tile);
            col.size = new Vector3(1, 1, 0.1f);
        }
        
        public void TileGen()
        {
            if (_isTileGen == true) return;

            _isTileGen = true;
            float spriteSizeX = BASE_SPRITE_SIZE_X;
            float spriteSizeY = BASE_SPRITE_SIZE_Y;
            float spawnPosY = 0;
            float spawnPosX = 0;
            float spawnPosZ = -1 * (float)BaseMapSize.Y / 100;

            for (int i = 0; i < (int)BaseMapSize.Y; i++)
            {
                if (i % 2 == 0)
                {
                    spawnPosX = 0;
                }
                else
                {
                    spawnPosX = (spriteSizeX / 2);
                }
                
                for (int j = 0; j < (int)BaseMapSize.X; j++)
                {
                    var spawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
                    MakeTile(spawnPos);
                    spawnPosX += spriteSizeX;
                }

                spawnPosY += 0.7f;
                spawnPosZ += 0.01f;
            }
        }

        public void DeleteAllTile()
        {
            foreach (var tile in tiles)
            {
                if (tile.gameObject == null) continue;
                DestroyImmediate(tile.gameObject);
            }

            _isTileGen = false;
        }

        protected virtual void DrawGizmos(Vector3 pos , Vector3 size)
        {
            Gizmos.DrawWireCube(pos, size);
        }

        // protected void OnDrawGizmosSelected()
        // {
        //     float spriteSizeX = baseSpriteSizeX;
        //     float spriteSizeY = baseSpriteSizeY;
        //     float spawnPosY = 0;
        //     float spawnPosX = 0;
        //     float spawnPosZ = -1 * (float)BaseMapSize.Y / 100;
        //
        //     var pos = transform.position;
        //
        //     for (int i = 0; i < (int)BaseMapSize.Y; i++)
        //     {
        //         if (i % 2 == 0)
        //         {
        //             Gizmos.color = Color.green;
        //             spawnPosX = 0;
        //         }
        //         else
        //         {
        //             Gizmos.color = Color.red;
        //             spawnPosX = (spriteSizeX / 2);
        //         }
        //         
        //         for (int j = 0; j < (int)BaseMapSize.X; j++)
        //         {
        //             var gizmoPos = new Vector3(pos.x + spawnPosX, pos.y + spawnPosY, pos.z + spawnPosZ - 1);
        //             var gizmoSize = new Vector3(1, 1, 0.1f);
        //             
        //             DrawGizmos(gizmoPos, gizmoSize);
        //             spawnPosX += spriteSizeX;
        //         }
        //
        //         spawnPosY += 0.7f;
        //         spawnPosZ += 0.01f;
        //     }
        // }
    }
}
using UnityEngine;
using static Utils;
using static Define;

namespace Controllers.MapEdit
{
    public class EnvironmentMapController : TileMapController
    {
        public override void MakeTile(Vector3 tilePos)
        {
            GameObject tile = new GameObject();
            tile.layer = (int)LayerNames.Environment;
            tile.transform.parent = transform;
            tile.transform.localPosition = new Vector3(tilePos.x, tilePos.y + 0.7f, tilePos.z);
            
            GetOrAddComponent<TileController>(tile);
            var sprite = GetOrAddComponent<SpriteRenderer>(tile);
            sprite.sortingOrder = (int)SortingLayerNum.Environment;
            var col = GetOrAddComponent<BoxCollider>(tile);
            col.size = new Vector3(1, 1, 0.1f);
        }

        protected override void DrawGizmos(Vector3 pos , Vector3 size)
        {
            pos.y += 0.5f;
            base.DrawGizmos(pos, size);
        }
    }
}
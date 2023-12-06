using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zephyr.RollGame.Tile
{
    /// <summary>
    /// RollGame의 TileMap을 나타내는 클래스
    /// Tile은 좌하단부터 (0, 0), (1, 0), ((2, 0), ... (0, 1), (1, 1), (2, 1), ... 순서로 생성됩니다.
    /// </summary>
    public class TileMap
    {
        public TileMap(GameObject parent)
        {
            if(parent == null)
            {
                Debug.LogError("TileMap의 부모 오브젝트가 비어있습니다.");
                return;
            }
            _parent = parent.transform;
        }

        private Transform _parent;
        private List<GameObject> _tiles = new List<GameObject>();

        public void Clear()
        {
            foreach(var tile in _tiles)
            {
                GameObject.Destroy(tile);
            }
            _tiles.Clear();
        }

        public void GenerateTile(RollStage stage, GameObject tilePrefab)
        {
            if(stage == null || !stage.IsValid)
            {
                Debug.LogError("Stage가 유효하지 않습니다.");
                return;
            }
            if(tilePrefab == null)
            {
                Debug.LogError("TilePrefab이 비어있습니다.");
                return;
            }

            int startX = -(stage.Width / 2);
            int startY = -(stage.Height / 2);

            for(int y = 0; y < stage.Height; ++y)
            {
                for(int x = 0; x < stage.Width; ++x)
                {
                    var tile = GameObject.Instantiate(tilePrefab, _parent);
                    tile.name = $"Tile_{x}_{y}";
                    tile.transform.localPosition = new Vector3(x + startX, y + startY, 0);
                    _tiles.Add(tile);
                }
            }
        }
    }
}

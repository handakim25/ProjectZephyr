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
        /// <summary>
        /// TileMap 생성자
        /// </summary>
        /// <param name="parent">Tile이 생성될 부모 오브젝트</param>
        /// <param name="tilePalette">Tile 종류를 지정</param>
        /// <param name="tilePrefab">Tile의 Prefab</param>
        public TileMap(GameObject parent, GameObject tilePrefab = null, TilePalette tilePalette = null)
        {
            if(parent == null)
            {
                Debug.LogError("TileMap의 부모 오브젝트가 비어있습니다.");
                return;
            }
            _parent = parent.transform;
            _tilePalette = tilePalette;
            _tilePrefab = tilePrefab;
        }

        /// <summary>
        /// TileMap이 생성될 부모 오브젝트의 Transfrom
        /// </summary>
        private readonly Transform _parent;
        /// <summary>
        /// TileMap에 생성된 Tile들
        /// </summary>
        private List<GameObject> _tiles = new List<GameObject>();

        private TilePalette _tilePalette;
        private const string _defaultTilePalettePath = "RollGame/DefaultTilePalette";
        public TilePalette TilePalette
        {
            get
            {
                if(_tilePalette == null)
                {
                    _tilePalette = Resources.Load<TilePalette>(_defaultTilePalettePath);
                    if(_tilePalette == null)
                    {
                        Debug.LogError("Default Tile Pallette을 찾을 수 없습니다.");
                    }
                }
                return _tilePalette;
            }
            set
            {
                _tilePalette = value;
            }
        }

        private GameObject _tilePrefab;
        private const string _defaultTilePrefabPath = "RollGame/TilePrefab";
        public GameObject TilePrefab
        {
            get
            {
                if(_tilePrefab == null)
                {
                    _tilePrefab = Resources.Load<GameObject>(_defaultTilePrefabPath);
                    if(_tilePrefab == null)
                    {
                        Debug.LogError("Default Tile Prefab을 찾을 수 없습니다.");
                    }
                }
                return _tilePrefab;
            }
            set
            {
                _tilePrefab = value;
            }
        }

        /// <summary>
        /// TileMap이 유효한지 체크. TilePalette와 TilePrefab이 유효한지 체크
        /// </summary>
        public bool IsValid => _parent != null && TilePalette != null && TilePrefab != null;

        public void Clear()
        {
            foreach(var tile in _tiles)
            {
                GameObject.Destroy(tile);
            }
            _tiles.Clear();
        }

        public void GenerateTileMap(RollStage stage)
        {
            if(stage == null || !stage.IsValid)
            {
                Debug.LogError("Stage가 유효하지 않습니다.");
                return;
            }
            if(!IsValid)
            {
                Debug.LogError("TileMap이 유효하지 않습니다.");
                return;
            }

            int startX = -(stage.Width / 2);
            int startY = -(stage.Height / 2);

            for(int y = 0; y < stage.Height; ++y)
            {
                for(int x = 0; x < stage.Width; ++x)
                {
                    TileData tileData = GetTileData(stage, x, y);
                    GameObject tile = CreateTile(startX + x, startY + y, tileData);
                    tile.name = $"Tile_{x}_{y}";
                    _tiles.Add(tile);
                }
            }
        }

        private GameObject CreateTile(int x, int y, TileData tileData)
        {
            var tile = GameObject.Instantiate(TilePrefab, _parent);
            tile.transform.localPosition = new Vector3(x, y, 0);

            tile.GetComponent<SpriteRenderer>().sprite = tileData.Sprite;
            return tile;
        }

        /// <summary>
        /// Stage의 TileMap에서 해당 좌표의 TileData를 가져온다.
        /// </summary>
        /// <param name="stage">Stage Data</param>
        /// <param name="x">Stage X 좌표</param>
        /// <param name="y">Stage Y 좌표</param>
        /// <returns>잘못된 값일 경우 Default Tile을 반환</returns>
        private TileData GetTileData(RollStage stage, int x, int y)
        {
            int tileIndex = stage.GetTile(x, y);
            if(tileIndex < 0 || tileIndex >= TilePalette.TileData.Count)
            {
                Debug.LogError($"TileMap의 TileIndex가 유효하지 않습니다. x: {x}, y: {y}, tileIndex: {tileIndex}");
                return TilePalette.DefaultTileData;
            }
            return TilePalette.TileData[tileIndex];
        }
    }
}

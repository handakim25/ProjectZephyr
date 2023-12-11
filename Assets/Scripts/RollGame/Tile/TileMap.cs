using System.Linq;
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
        #region Variables and Properties
        
        /// <summary>
        /// TileMap이 생성될 부모 오브젝트의 Transfrom
        /// </summary>
        private readonly Transform _parent;
        /// <summary>
        /// TileMap에 생성된 Tile들
        /// </summary>
        private GameObject[,] _tiles;
        private Dictionary<GameObject, TileData> _tileDataMap = new();

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
        
        #endregion Variables and Properties

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
        /// TileMap을 초기화한다.
        /// </summary>
        public void Clear()
        {
            for(int i = 0; i < _parent.childCount; ++i)
            {
                GameObject.Destroy(_parent.GetChild(i).gameObject);
            }
            _tiles = null;
            _tileDataMap.Clear();
        }

        /// <summary>
        /// Stage Data를 이용해 TileMap을 생성한다.
        /// </summary>
        /// <param name="stage">생성할 Stage Data</param>
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
            _tiles = new GameObject[stage.Width, stage.Height];

            int startX = -(stage.Width / 2);
            int startY = -(stage.Height / 2);

            for(int y = 0; y < stage.Height; ++y)
            {
                for(int x = 0; x < stage.Width; ++x)
                {
                    TileData tileData = GetStageTileData(stage, x, y);
                    GameObject tile = CreateTile(startX + x, startY + y, tileData, $"Tile_{x}_{y}");
                    if(tile == null)
                    {
                        return; 
                    }
                    _tiles[x, y] = tile;
                    _tileDataMap.Add(tile, tileData);
                    _tileDataMap.TryAdd(tile, tileData);
                }
            }
        }

        /// <summary>
        /// TileData를 이용해 Tile을 생성한다.
        /// </summary>
        /// <param name="x">x 좌표</param>
        /// <param name="y">y 좌표</param>
        /// <param name="tileData">생성할 Tile</param>
        /// <param name="goName">생성할 Tile의 이름</param>
        /// <returns></returns>
        private GameObject CreateTile(int x, int y, TileData tileData, string goName = null)
        {
            if(tileData.IsEmpty)
            {
                return null;
            }
            var tile = GameObject.Instantiate(TilePrefab, _parent);
            tile.transform.localPosition = new Vector3(x, y, 0);
            tile.name = goName;

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
        private TileData GetStageTileData(RollStage stage, int x, int y)
        {
            int tileIndex = stage.GetTile(x, y);
            if(tileIndex < 0 || tileIndex >= TilePalette.TileData.Count)
            {
                Debug.LogError($"TileMap의 TileIndex가 유효하지 않습니다. x: {x}, y: {y}, tileIndex: {tileIndex}");
                return TilePalette.DefaultTileData;
            }
            return TilePalette.TileData[tileIndex];
        }

        /// <summary>
        /// TileMap에서 해당 좌표의 Tile 데이터를 가져온다. 좌하단이 (0, 0), (x, y) 좌표계
        /// </summary>
        /// <param name="x">x 좌표</param>
        /// <param name="y">y 좌표</param>
        /// <returns>out of boundary</returns>
        public TileData GetTileData(int x, int y)
        {
            int lengthX = _tiles.GetLength(0);
            int lengthY = _tiles.GetLength(1);
            if(x < 0 || x >= lengthX || y < 0 || y >= lengthY)
            {
                return null;
            }
            if(_tiles[x, y] == null)
            {
                return new TileData() { IsEmpty = true };
            }
            return _tileDataMap[_tiles[x, y]];
        }
    }
}

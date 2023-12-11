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
        /// TileMap에 생성된 Tile들, 좌하단 (0, 0), ~ (x, y) 좌표계.
        /// 비어있는 Tile은 null로 표시한다.
        /// </summary>
        private GameObject[,] _tiles;
        private Dictionary<GameObject, TileData> _tileDataMap = new();
        public int Width => _tiles?.GetLength(0) ?? 0;
        public int Height => _tiles?.GetLength(1) ?? 0;
        public bool IsInTileMap(int x, int y) =>
             x >= 0 && x < Width && y >= 0 && y < Height;

        private int _startX = 0;
        private int _startY = 0;

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

            _startX = -(stage.Width / 2);
            _startY = -(stage.Height / 2);

            for(int y = 0; y < stage.Height; ++y)
            {
                for(int x = 0; x < stage.Width; ++x)
                {
                    TileData tileData = GetStageTileData(stage, x, y);
                    GameObject tile = CreateTile(_startX + x, _startY + y, tileData, $"Tile_{x}_{y}");
                    AddTile(x, y, tile, tileData);
                }
            }
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
        /// TileMap에 Tile을 추가한다. 비어 있는 타일을 추가할 경우 아무런 동작을 하지 않는다.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tileGO">추가할 Tile Gameobject</param>
        /// <param name="tileData">Tile의 데이터, 현재는 TileData를 Tile GameObject가 공유해서 쓰고 있다.</param>
        private void AddTile(int x, int y, GameObject tileGO, TileData tileData)
        {
            if(tileGO == null)
            {
                return;
            }
            // tiles는 이미 null로 되어 있으므로 비어 있는 타일을 처리할 필요가 없다.
            _tiles[x, y] = tileGO;
            _tileDataMap.Add(tileGO, tileData);
        }
        
        /// <summary>
        /// TileMap에서 해당 좌표의 Tile 데이터를 가져온다. 좌하단이 (0, 0), (x, y) 좌표계
        /// </summary>
        /// <param name="x">x 좌표</param>
        /// <param name="y">y 좌표</param>
        /// <returns>Tile이 없을 경우 empty를 반환, 범위를 벗어났을 경우 null을 반환</returns>
        public TileData GetTileData(int x, int y)
        {
            if(!IsInTileMap(x, y))
            {
                return null;
            }
            var tile = GetTile(x, y);
            if(tile == null)
            {
                return new TileData() { IsEmpty = true };
            }
            return _tileDataMap[tile];
        }

        /// <summary>
        /// TileMap에서 해당 좌표의 Tile을 가져온다. 좌하단이 (0, 0), (x, y) 좌표계
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>(x,y)의 Tile 객체를 반환한다. 만약에 범위를 벗어났을 경우 null을 반환한다.</returns>
        public GameObject GetTile(int x, int y)
        {
            if(!IsInTileMap(x, y))
            {
                return null;
            }
            return _tiles[x, y];
        }

        private (int, int) FindTile(GameObject tile)
        {
            for(int y = 0; y < Height; ++y)
            {
                for(int x = 0; x < Width; ++x)
                {
                    if(_tiles[x, y] == tile)
                    {
                        return (x, y);
                    }
                }
            }
            return (-1, -1);
        }

        /// <summary>
        /// TileMap에서 해당 좌표의 Tile을 설정한다. 좌하단이 (0, 0), (x, y) 좌표계, 비어 있는 타일을 설정할려면 null로 설정한다.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tile"></param>
        /// <returns>Tile을 설정했을 경우 true를 반환한다. 만약에 범위를 벗어났을 경우 false를 반환한다.</returns>
        private bool SetTile(int x, int y, GameObject tile)
        {
            if(!IsInTileMap(x, y))
            {
                return false;
            }
            // 이미 해당 좌표에 Tile이 있을 경우 false를 반환한다.
            if(GetTile(x,y) != null)
            {
                return false;
            }
            _tiles[x, y] = tile;
            AdjustTilePos(x, y, tile);
            return true;
        }

        private void AdjustTilePos(int x, int y, GameObject tile)
        {
            if(!IsInTileMap(x, y))
            {
                return;
            }
            if(tile == null)
            {
                return;
            }
            tile.transform.localPosition = new Vector3(_startX + x, _startY + y, tile.transform.localPosition.z);
        }

        public bool MoveToTile(int x, int y, GameObject tile)
        {
            if(!IsInTileMap(x, y))
            {
                return false;
            }
            // 이미 해당 좌표에 Tile이 있을 경우 false를 반환한다.
            if(GetTile(x,y) != null)
            {
                return false;
            }
            var (prevX, prevY) = FindTile(tile);
            SetTile(prevX, prevY, null);
            SetTile(x, y, tile);
            return true;
        }
    }
}

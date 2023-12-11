using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zephyr.RollGame.Tile;

namespace Zephyr.RollGame
{
    /// <summary>
    /// RollGame의 게임 매니저
    /// </summary>
    public sealed class RollGameManager : MonoSingleton<RollGameManager>
    {
        [Header("Tile Setting")]
        [Tooltip("Tile을 생성할 부모 오브젝트")]
        [SerializeField] private GameObject _tiles;
        [Tooltip("Tile 생성 위치 오프셋")]
        [SerializeField] private Vector2 _tileOffset = new Vector2(0, 0);
        [SerializeField] private string _tilePalettePath;
        [SerializeField] private TilePalette _tilePalette;
        [SerializeField] private GameObject _testTilePrefab;
        private TileMap _tileMap;

        public GameObject Tiles
        {
            get
            {
                if(_tiles == null)
                {
                    _tiles = GameObject.Find("Tiles");
                    if(_tiles == null)
                    {
                        _tiles = new GameObject("Tiles");
                    }
                }
                return _tiles;
            }
            set
            {
                _tiles = value;
            }
        }

        [SerializeField] private RollGameSetting _rollGameSetting;

        // 나중에 Stage Data로 분리될 영역
        [Header("Stage")]
        [SerializeField] private RollStage _curStage;

        /// <summary>
        /// 현재 Stage가 유효한지 체크
        /// </summary>
        public bool CheckValid => _curStage != null && _curStage.IsValid;

        private void Awake()
        {
           Tiles.transform.position = _tileOffset;
            _tileMap = new TileMap(Tiles, _testTilePrefab, _tilePalette);

            if(_tilePalette == null)
            {
                _tilePalette = Resources.Load<TilePalette>(_tilePalettePath);
                if(_tilePalette == null)
                {
                    Debug.LogError($"TilePalette를 찾을 수 없습니다 : {_tilePalettePath}");
                }
            }

            Tile.Tile.Init(Camera.main, _tileMap, _rollGameSetting);
        }

        private void Start()
        {
            InitGame();
        }

        /// <summary>
        /// 현재 설정된 Stage Data를 기반으로 게임을 초기화한다.
        /// </summary>
        public void InitGame()
        {
            if(!CheckValid)
            {
                Debug.LogError("게임 초기화에 필요한 데이터가 유효하지 않습니다.");
                return;
            }
            _tileMap.Clear();
            _tileMap.GenerateTileMap(_curStage);
        }

        public void StartGame()
        {
            // Blcok Input

            // Show Start UI
        }

        public void PlayGame()
        {
            // UnBlock Input

            // Start Timer
        }

        /// <summary>
        /// Goal Condition을 체크.
        /// </summary>
        /// <returns></returns>
        public bool CheckGoalCondition()
        {
            // Check Game Clear Condition
            return false;
        }

        #region Test Methods
        
        [ContextMenu("Show TileMap")]
        public void ShowTileMap()
        {
            if(_tileMap == null)
            {
                Debug.Log($"생성된 TileMap이 없습니다.");
                return;
            }

            Debug.Log($"Width: {_tileMap.Width}, Height: {_tileMap.Height}");
            Debug.Log($"TileMap:\n{_tileMap}");
        }
        
        #endregion Test Methods
    }
}

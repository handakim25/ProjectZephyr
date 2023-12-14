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
        [SerializeField] private RollGameSetting _rollGameSetting;

        [Header("Tile Setting")]
        [Tooltip("Tile을 생성할 부모 오브젝트")]
        [SerializeField] private GameObject _tiles;
        [Tooltip("Tile 생성 위치 오프셋")]
        [SerializeField] private Vector2 _tileOffset = new Vector2(0, 0);
        [SerializeField] private string _tilePalettePath;
        [SerializeField] private TilePalette _tilePalette;
        [SerializeField] private GameObject _testTilePrefab;
        private TileMap _tileMap;

        public GameObject TileMapGo
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

        [Header("Background")]
        [SerializeField] private GameObject _background;
        public GameObject Background
        {
            get
            {
                if(_background == null)
                {
                    _background = GameObject.Find("Background");
                    if(_background == null)
                    {
                        _background = new GameObject("Background", typeof(SpriteRenderer));
                    }
                }
                return _background;
            }
            set
            {
                _background = value;
            }
        }

        // 나중에 Stage Data로 분리될 영역
        [Header("Stage")]
        [SerializeField] private RollStage _curStage;

        /// <summary>
        /// 현재 Stage가 유효한지 체크
        /// </summary>
        public bool CheckValid => _curStage != null && _curStage.IsValid;

        private void Awake()
        {
           TileMapGo.transform.position = _tileOffset;
            _tileMap = new TileMap(TileMapGo, _testTilePrefab, _tilePalette);

            if(_tilePalette == null)
            {
                _tilePalette = Resources.Load<TilePalette>(_tilePalettePath);
                if(_tilePalette == null)
                {
                    Debug.LogError($"TilePalette를 찾을 수 없습니다 : {_tilePalettePath}");
                }
            }

            Tile.Tile.InitTileSetting(Camera.main, _tileMap, _rollGameSetting);
        }

        private void Start()
        {
            InitGame(_curStage);
        }

        /// <summary>
        /// 현재 설정된 Stage Data를 기반으로 게임을 초기화한다.
        /// </summary>
        public void InitGame(RollStage stage)
        {
            if(!CheckValid)
            {
                Debug.LogError("게임 초기화에 필요한 데이터가 유효하지 않습니다.");
                return;
            }
            _tileMap.Clear();
            _tileMap.GenerateTileMap(stage);
            GenerateBackground(_tileOffset, _tileMap.Width, _tileMap.Height);
        }

        // @Memo
        // 관련 로직이 길어질 경우 별도의 객체로 분리
        /// <summary>
        /// TileMap의 배경을 생성한다. 9-Slice 이미지를 이용한다.
        /// </summary>
        private void GenerateBackground(Vector2 tilemapOrigin, int width, int height)
        {
            // Tile 들의 경우 시작점이 - (width / 2)이다.
            // 현재 Tile의 Width가 4이므로 -2가 시작점이다.
            // 여기에 배경의 패딩을 계산해서 크기를 정한다. 
            float bottomLeftX = - (width / 2f) - _rollGameSetting.BackgroundLeftPadding - _rollGameSetting.TileSize / 2f;
            float bottomLeftY = - (height / 2f) - _rollGameSetting.BackgroundDownPadding - _rollGameSetting.TileSize / 2f;
            
            if(Background.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                // Sprite Size
                // x : width + (left padding + right padding)
                // y : height + (up padding + down padding)
                spriteRenderer.size = new Vector2(width + _rollGameSetting.BackgroundLeftPadding + _rollGameSetting.BackgroundRightPadding,
                    height + _rollGameSetting.BackgroundUpPadding + _rollGameSetting.BackgroundDownPadding);
                // Sprite Position

                spriteRenderer.transform.position = new Vector3(bottomLeftX + (width / 2f), bottomLeftY + (height / 2f), 0);
            }
            else
            {
                Debug.LogError($"SpriteRenderer를 찾을 수 없습니다.");
            }
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

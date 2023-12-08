using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zephyr.RollGame
{
    [CreateAssetMenu(fileName = "RollStage", menuName = "Stages/RollStage")]
    public class RollStage : Stage
    {
        [Tooltip("스테이지의 가로 길이")]
        [Range(1,16)]
        public int Width = 4;
        [Tooltip("스테이지의 세로 길이")]
        [Range(1, 16)]
        public int Height = 4;
        [Tooltip("Stage Tile Data. 좌하단부터 행 우선으로 저장")]
        public int[] TileMap = new int[16];
        public int GetTile(int x, int y)
        {
            if(x < 0 || x >= Width || y < 0 || y >= Height)
            {
                Debug.LogError($"TileMap의 범위를 벗어난 인덱스입니다. x: {x}, y: {y}");
                return -1;
            }
            return TileMap[y * Width + x];
        }

        public bool TryGetTile(int x, int y, out int tile)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                tile = -1;
                return false;
            }
            tile = TileMap[y * Width + x];
            return true;
        }

        public bool IsValid => Width > 0 && Height > 0;
    }
}

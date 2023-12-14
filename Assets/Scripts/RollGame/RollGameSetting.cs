using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zephyr
{
    [CreateAssetMenu(fileName = "RollGameSetting", menuName = "RollGame/RollGameSetting")]
    public class RollGameSetting : ScriptableObject
    {
        [Header("Tile Setting")]
        public float TileSize = 1f;
        [Range(0f, 1f)]
        public float TileMoveThreshold = 0.1f;
        [Range(0f, 1f)]
        public float TileSnapThreshold = 0.4f;

        [Header("Background Setting")]
        [Tooltip("배경의 왼쪽 패딩 비율")]
        public float BackgroundLeftPadding = 0.2f;
        [Tooltip("배경의 위쪽 패딩 비율")]
        public float BackgroundUpPadding = 0.2f;
        [Tooltip("배경의 오른쪽 패딩 비율")]
        public float BackgroundRightPadding = 0.2f;
        [Tooltip("배경의 아래쪽 패딩 비율")]
        public float BackgroundDownPadding = 0.2f;
    }
}

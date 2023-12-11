using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zephyr
{
    [CreateAssetMenu(fileName = "RollGameSetting", menuName = "RollGame/RollGameSetting")]
    public class RollGameSetting : ScriptableObject
    {
        public float TileSize = 1f;
        [Range(0f, 1f)]
        public float TileMoveThreshold = 0.1f;
        [Range(0f, 1f)]
        public float TileSnapThreshold = 0.4f;
    }
}

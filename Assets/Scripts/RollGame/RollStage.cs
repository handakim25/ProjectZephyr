using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zephyr.RollGame
{
    [CreateAssetMenu(fileName = "RollStage", menuName = "Stages/RollStage")]
    public class RollStage : Stage
    {
        public int Width;
        public int Height;
        public int[,] TileMap;

        public bool IsValid => Width > 0 && Height > 0;
    }
}

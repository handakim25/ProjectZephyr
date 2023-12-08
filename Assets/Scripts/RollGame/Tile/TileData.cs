using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zephyr.RollGame.Tile
{
    [System.Serializable]
    public class TileData
    {
        public bool IsEmpty = false;
        public bool CanMove = true;

        public Sprite Sprite;
        public string TileName;
        public string SpriteTilePath;
    }
}

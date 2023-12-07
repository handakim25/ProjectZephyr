using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zephyr.RollGame.Tile
{
    /// <summary>
    /// Tile 종류를 나타내는 SO
    /// </summary>
    [CreateAssetMenu(fileName ="TilePalette", menuName ="RollGame/TilePalette")]
    public class TilePalette : ScriptableObject
    {
        public List<TileData> TileData = new List<TileData>();
        public  List<string> TileNames => TileData.Select(tileData => tileData.TileName).ToList();
    }
}

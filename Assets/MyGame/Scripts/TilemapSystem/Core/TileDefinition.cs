using System;

namespace MyGame.TilemapSystem.Core
{
    public enum TileType
    {
        Empty,      // 空間
        Ground,     // 地面
        Wall,       // 壁
        Treasure,   // お宝
        Enemy       // エネミー配置位置
    }

    public struct MapData
    {
        public readonly int Width;           // 横幅
        public readonly int Height;          // 縦幅
        public readonly TileType[,] Tiles;   // タイル配置
        public readonly int Seed;            // 生成シード
        public readonly int Level;           // レベル番号

        public MapData(int width, int height, TileType[,] tiles, int seed, int level)
        {
            Width = width;
            Height = height;
            Tiles = tiles;
            Seed = seed;
            Level = level;
        }
    }
}
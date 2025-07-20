namespace MyGame.TilemapSystem.Core
{
    public struct MapData
    {
        public readonly int Width;           // 横幅
        public readonly int Height;          // 縦幅
        public readonly BlockType[,] Tiles;   // タイル配置
        public readonly int Seed;            // 生成シード
        public readonly int Level;           // レベル番号

        public MapData(int width, int height, BlockType[,] tiles, int seed, int level)
        {
            Width = width;
            Height = height;
            Tiles = tiles;
            Seed = seed;
            Level = level;
        }
    }
}
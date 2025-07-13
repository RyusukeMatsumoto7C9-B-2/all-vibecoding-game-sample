using System;
using MyGame.TilemapSystem.Generation;

namespace MyGame.TilemapSystem.Core
{
    public class TilemapGenerator
    {
        public const int MAP_WIDTH = 20;
        public const int MAP_HEIGHT = 30;
        public const int GROUND_AREA_HEIGHT = 5;

        private readonly SeedManager _seedManager;

        public TilemapGenerator(SeedManager seedManager)
        {
            _seedManager = seedManager ?? throw new ArgumentNullException(nameof(seedManager));
        }

        public MapData GenerateMap(int level, int seed)
        {
            _seedManager.SetSeed(seed);
            var random = _seedManager.CreateRandomForLevel(level);
            
            var tiles = new TileType[MAP_WIDTH, MAP_HEIGHT];
            
            // 上5マスを地上エリア（空間）として設定
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                for (int y = MAP_HEIGHT - GROUND_AREA_HEIGHT; y < MAP_HEIGHT; y++)
                {
                    tiles[x, y] = TileType.Empty;
                }
            }
            
            // 地下エリアの基本パターン生成
            GenerateUndergroundArea(tiles, random);
            
            return new MapData(MAP_WIDTH, MAP_HEIGHT, tiles, seed, level);
        }

        private void GenerateUndergroundArea(TileType[,] tiles, Random random)
        {
            var undergroundHeight = MAP_HEIGHT - GROUND_AREA_HEIGHT;
            
            // 基本的な地形パターン：通路と壁のランダム配置
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                for (int y = 0; y < undergroundHeight; y++)
                {
                    // 境界は必ず壁にする
                    if (x == 0 || x == MAP_WIDTH - 1 || y == 0)
                    {
                        tiles[x, y] = TileType.Wall;
                    }
                    else
                    {
                        // 60%の確率で空間、40%の確率で壁
                        tiles[x, y] = random.NextDouble() < 0.6 ? TileType.Empty : TileType.Wall;
                    }
                }
            }
            
            // 最低限の通路を確保（中央縦ライン）
            var centerX = MAP_WIDTH / 2;
            for (int y = 1; y < undergroundHeight - 1; y++)
            {
                tiles[centerX, y] = TileType.Empty;
                // 左右にも通路を作る
                if (centerX > 1) tiles[centerX - 1, y] = TileType.Empty;
                if (centerX < MAP_WIDTH - 2) tiles[centerX + 1, y] = TileType.Empty;
            }
        }
    }
}
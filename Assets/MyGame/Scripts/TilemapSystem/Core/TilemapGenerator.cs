using System;
using MyGame.TilemapSystem.Generation;

namespace MyGame.TilemapSystem.Core
{
    public class TilemapGenerator
    {
        // public定数
        public static readonly int MAP_WIDTH = 15;
        public static readonly int MAP_HEIGHT = 20;
        public static readonly int GROUND_AREA_HEIGHT = 5;

        // privateフィールド
        private readonly SeedManager _seedManager;
        private readonly ProceduralGenerator _proceduralGenerator;


        // publicメソッド
        public TilemapGenerator(SeedManager seedManager)
        {
            _seedManager = seedManager ?? throw new ArgumentNullException(nameof(seedManager));
            _proceduralGenerator = new ProceduralGenerator(MAP_WIDTH, MAP_HEIGHT, GROUND_AREA_HEIGHT);
        }


        public MapData GenerateMap(int level, int seed)
        {
            _seedManager.SetSeed(seed);
            var random = _seedManager.CreateRandomForLevel(level);
            
            // ProceduralGeneratorを使用して高度な地形生成（レベル情報を渡す）
            var tiles = _proceduralGenerator.GenerateTerrain(random, level);
            
            return new MapData(MAP_WIDTH, MAP_HEIGHT, tiles, seed, level);
        }


        public int GetSeedForLevel(int level)
        {
            return _seedManager.GetSeedForLevel(level);
        }
    }
}
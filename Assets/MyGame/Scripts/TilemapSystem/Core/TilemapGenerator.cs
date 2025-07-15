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
        private readonly ProceduralGenerator _proceduralGenerator;

        public TilemapGenerator(SeedManager seedManager)
        {
            _seedManager = seedManager ?? throw new ArgumentNullException(nameof(seedManager));
            _proceduralGenerator = new ProceduralGenerator(MAP_WIDTH, MAP_HEIGHT, GROUND_AREA_HEIGHT);
        }

        public MapData GenerateMap(int level, int seed)
        {
            _seedManager.SetSeed(seed);
            var random = _seedManager.CreateRandomForLevel(level);
            
            // ProceduralGeneratorを使用して高度な地形生成
            var tiles = _proceduralGenerator.GenerateTerrain(random);
            
            return new MapData(MAP_WIDTH, MAP_HEIGHT, tiles, seed, level);
        }

        public int GetSeedForLevel(int level)
        {
            return _seedManager.GetSeedForLevel(level);
        }
    }
}
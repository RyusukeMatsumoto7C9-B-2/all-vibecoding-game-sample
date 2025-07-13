using System;

namespace MyGame.TilemapSystem.Generation
{
    public class SeedManager
    {
        private int _baseSeed;
        private Random _random;

        public SeedManager(int baseSeed = 0)
        {
            SetSeed(baseSeed);
        }

        public void SetSeed(int baseSeed)
        {
            _baseSeed = baseSeed;
            _random = new Random(_baseSeed);
        }

        public int GetSeedForLevel(int level)
        {
            var combinedSeed = _baseSeed + level * 1000;
            return combinedSeed;
        }

        public Random CreateRandomForLevel(int level)
        {
            var levelSeed = GetSeedForLevel(level);
            return new Random(levelSeed);
        }
    }
}
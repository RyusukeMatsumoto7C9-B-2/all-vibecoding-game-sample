using System;
using MyGame.TilemapSystem.Core;

namespace MyGame.TilemapSystem.Generation
{
    public class ProceduralGenerator
    {
        private readonly int _mapWidth;
        private readonly int _mapHeight;
        private readonly int _groundAreaHeight;
        
        public ProceduralGenerator(int mapWidth = 20, int mapHeight = 30, int groundAreaHeight = 5)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _groundAreaHeight = groundAreaHeight;
        }
        
        public TileType[,] GenerateTerrain(Random random)
        {
            var tiles = new TileType[_mapWidth, _mapHeight];
            
            // GroundベースでランダムにWallを配置
            FillAllTilesWithoutGaps(tiles, random);
            
            return tiles;
        }
        
        private void FillAllTilesWithoutGaps(TileType[,] tiles, Random random)
        {
            // 横20 x 縦30のグリッドを隙間なく配置
            // Groundをベースとして、30%の確率でWallを配置
            const double wallProbability = 0.3;
            
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapHeight; y++)
                {
                    // 基本はGround、30%の確率でWallに変更
                    if (random.NextDouble() < wallProbability)
                    {
                        tiles[x, y] = TileType.Wall;
                    }
                    else
                    {
                        tiles[x, y] = TileType.Ground;
                    }
                }
            }
        }
    }
}
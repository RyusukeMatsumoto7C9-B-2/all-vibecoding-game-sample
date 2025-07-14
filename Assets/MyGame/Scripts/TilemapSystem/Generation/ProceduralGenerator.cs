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
            
            // 全てのマスを隙間なく配置（ランダム要素なし）
            FillAllTilesWithoutGaps(tiles);
            
            return tiles;
        }
        
        private void FillAllTilesWithoutGaps(TileType[,] tiles)
        {
            // 横20 x 縦30のグリッドを隙間なく、ランダム要素なしで配置
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapHeight; y++)
                {
                    // シンプルなパターンで全てのマスにタイルを配置
                    tiles[x, y] = TileType.Wall;
                }
            }
        }
    }
}
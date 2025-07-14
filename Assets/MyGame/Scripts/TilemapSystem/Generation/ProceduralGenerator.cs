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
            // まず全てをGroundで埋める
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapHeight; y++)
                {
                    tiles[x, y] = TileType.Ground;
                }
            }
            
            // 1レベル当たり3~5個のWallをランダム配置
            PlaceLimitedWalls(tiles, random);
        }
        
        private void PlaceLimitedWalls(TileType[,] tiles, Random random)
        {
            // 3~5個のランダムな数のWallを配置
            int wallCount = random.Next(3, 6); // 3以上6未満（つまり3~5個）
            
            for (int i = 0; i < wallCount; i++)
            {
                // ランダムな位置を選択（既にWallが配置されている場合は再選択）
                int x, y;
                int attempts = 0;
                const int maxAttempts = 100; // 無限ループ回避
                
                do
                {
                    x = random.Next(0, _mapWidth);
                    y = random.Next(0, _mapHeight);
                    attempts++;
                } while (tiles[x, y] == TileType.Wall && attempts < maxAttempts);
                
                // Wallを配置
                tiles[x, y] = TileType.Wall;
            }
        }
    }
}
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
        
        public BlockType[,] GenerateTerrain(Random random, int level = 1)
        {
            var tiles = new BlockType[_mapWidth, _mapHeight];
            
            // レベル1の場合は上5マス分をSkyブロックに設定
            if (level == 1)
            {
                FillWithSkyAndGround(tiles, random);
            }
            else
            {
                // レベル2以降はGroundベースでランダムにRockを配置
                FillAllTilesWithoutGaps(tiles, random);
            }
            
            return tiles;
        }
        
        private void FillWithSkyAndGround(BlockType[,] tiles, Random random)
        {
            // レベル1専用：上5マス分をSkyブロック、残りをGroundで埋める
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapHeight; y++)
                {
                    // 上5マス分（y座標が25以上）をSkyブロックに設定
                    if (y >= _mapHeight - _groundAreaHeight)
                    {
                        tiles[x, y] = BlockType.Sky;
                    }
                    else
                    {
                        tiles[x, y] = BlockType.Ground;
                    }
                }
            }
            
            // 1レベル当たり3~5個のRockをランダム配置（Sky部分を除く）
            PlaceLimitedRocks(tiles, random);
        }
        
        private void FillAllTilesWithoutGaps(BlockType[,] tiles, Random random)
        {
            // 横20 x 縦30のグリッドを隙間なく配置
            // まず全てをGroundで埋める
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapHeight; y++)
                {
                    tiles[x, y] = BlockType.Ground;
                }
            }
            
            // 1レベル当たり3~5個のRockをランダム配置
            PlaceLimitedRocks(tiles, random);
        }
        
        private void PlaceLimitedRocks(BlockType[,] tiles, Random random)
        {
            // 3~5個のランダムな数のRockを配置
            int rockCount = random.Next(3, 6); // 3以上6未満（つまり3~5個）
            
            for (int i = 0; i < rockCount; i++)
            {
                // ランダムな位置を選択（既にRockが配置されている場合やSkyブロックは再選択）
                int x, y;
                int attempts = 0;
                const int maxAttempts = 100; // 無限ループ回避
                
                do
                {
                    x = random.Next(0, _mapWidth);
                    y = random.Next(0, _mapHeight);
                    attempts++;
                } while ((tiles[x, y] == BlockType.Rock || tiles[x, y] == BlockType.Sky) && attempts < maxAttempts);
                
                // Rockを配置
                tiles[x, y] = BlockType.Rock;
            }
        }
    }
}
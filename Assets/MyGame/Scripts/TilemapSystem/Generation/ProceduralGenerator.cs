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
            
            // 地上エリアを空間に設定
            SetGroundArea(tiles);
            
            // 地下エリアの基本地形生成
            GenerateBaseTerrain(tiles, random);
            
            // 通路確保処理
            EnsurePassageways(tiles, random);
            
            // 地形の滑らか化（ノイズ除去）
            SmoothTerrain(tiles, random);
            
            return tiles;
        }
        
        private void SetGroundArea(TileType[,] tiles)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = _mapHeight - _groundAreaHeight; y < _mapHeight; y++)
                {
                    tiles[x, y] = TileType.Empty;
                }
            }
        }
        
        private void GenerateBaseTerrain(TileType[,] tiles, Random random)
        {
            var undergroundHeight = _mapHeight - _groundAreaHeight;
            
            // セルラーオートマトンによる地形生成
            GenerateCellularAutomata(tiles, random, undergroundHeight);
        }
        
        private void GenerateCellularAutomata(TileType[,] tiles, Random random, int undergroundHeight)
        {
            // 初期ランダム配置（45%の確率で壁）
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < undergroundHeight; y++)
                {
                    // 境界は必ず壁
                    if (x == 0 || x == _mapWidth - 1 || y == 0)
                    {
                        tiles[x, y] = TileType.Wall;
                    }
                    else
                    {
                        tiles[x, y] = random.NextDouble() < 0.45 ? TileType.Wall : TileType.Empty;
                    }
                }
            }
            
            // セルラーオートマトンのイテレーション（3回）
            for (int iteration = 0; iteration < 3; iteration++)
            {
                ApplyCellularAutomataRules(tiles, undergroundHeight);
            }
        }
        
        private void ApplyCellularAutomataRules(TileType[,] tiles, int undergroundHeight)
        {
            var newTiles = new TileType[_mapWidth, _mapHeight];
            
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < undergroundHeight; y++)
                {
                    // 境界は変更しない
                    if (x == 0 || x == _mapWidth - 1 || y == 0)
                    {
                        newTiles[x, y] = TileType.Wall;
                        continue;
                    }
                    
                    int wallCount = CountNeighboringWalls(tiles, x, y, undergroundHeight);
                    
                    // ルール：周囲に4個以上の壁があれば壁、そうでなければ空間
                    newTiles[x, y] = wallCount >= 4 ? TileType.Wall : TileType.Empty;
                }
            }
            
            // 結果をコピー
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < undergroundHeight; y++)
                {
                    tiles[x, y] = newTiles[x, y];
                }
            }
        }
        
        private int CountNeighboringWalls(TileType[,] tiles, int centerX, int centerY, int undergroundHeight)
        {
            int wallCount = 0;
            
            for (int x = centerX - 1; x <= centerX + 1; x++)
            {
                for (int y = centerY - 1; y <= centerY + 1; y++)
                {
                    // 中心座標はカウントしない
                    if (x == centerX && y == centerY) continue;
                    
                    // 範囲外は壁として扱う
                    if (x < 0 || x >= _mapWidth || y < 0 || y >= undergroundHeight)
                    {
                        wallCount++;
                    }
                    else if (tiles[x, y] == TileType.Wall)
                    {
                        wallCount++;
                    }
                }
            }
            
            return wallCount;
        }
        
        private void EnsurePassageways(TileType[,] tiles, Random random)
        {
            var undergroundHeight = _mapHeight - _groundAreaHeight;
            
            // メイン通路を確保（中央縦方向）
            CreateMainVerticalPassage(tiles, undergroundHeight);
            
            // 横方向の通路を作成
            CreateHorizontalPassages(tiles, random, undergroundHeight);
            
            // 接続性チェックと修正
            EnsureConnectivity(tiles, random, undergroundHeight);
        }
        
        private void CreateMainVerticalPassage(TileType[,] tiles, int undergroundHeight)
        {
            int centerX = _mapWidth / 2;
            
            for (int y = 1; y < undergroundHeight - 1; y++)
            {
                // 中央とその左右に通路を作成
                tiles[centerX, y] = TileType.Empty;
                if (centerX > 1) tiles[centerX - 1, y] = TileType.Empty;
                if (centerX < _mapWidth - 2) tiles[centerX + 1, y] = TileType.Empty;
            }
        }
        
        private void CreateHorizontalPassages(TileType[,] tiles, Random random, int undergroundHeight)
        {
            // 3〜5本の横通路を作成
            int passageCount = random.Next(3, 6);
            
            for (int i = 0; i < passageCount; i++)
            {
                int y = random.Next(2, undergroundHeight - 2);
                
                // 左端から右端へ通路を作成
                for (int x = 1; x < _mapWidth - 1; x++)
                {
                    tiles[x, y] = TileType.Empty;
                    
                    // 50%の確率で上下にも空間を作る
                    if (random.NextDouble() < 0.5 && y > 1)
                    {
                        tiles[x, y - 1] = TileType.Empty;
                    }
                    if (random.NextDouble() < 0.5 && y < undergroundHeight - 2)
                    {
                        tiles[x, y + 1] = TileType.Empty;
                    }
                }
            }
        }
        
        private void EnsureConnectivity(TileType[,] tiles, Random random, int undergroundHeight)
        {
            // 孤立した空間エリアを検出して接続
            var visited = new bool[_mapWidth, _mapHeight];
            var regions = FindEmptyRegions(tiles, visited, undergroundHeight);
            
            // 最大の領域以外を主要領域に接続
            if (regions.Count > 1)
            {
                ConnectRegions(tiles, regions, random, undergroundHeight);
            }
        }
        
        private System.Collections.Generic.List<System.Collections.Generic.List<(int x, int y)>> FindEmptyRegions(
            TileType[,] tiles, bool[,] visited, int undergroundHeight)
        {
            var regions = new System.Collections.Generic.List<System.Collections.Generic.List<(int x, int y)>>();
            
            for (int x = 1; x < _mapWidth - 1; x++)
            {
                for (int y = 1; y < undergroundHeight - 1; y++)
                {
                    if (tiles[x, y] == TileType.Empty && !visited[x, y])
                    {
                        var region = new System.Collections.Generic.List<(int x, int y)>();
                        FloodFillRegion(tiles, visited, x, y, undergroundHeight, region);
                        
                        if (region.Count > 0)
                        {
                            regions.Add(region);
                        }
                    }
                }
            }
            
            return regions;
        }
        
        private void FloodFillRegion(TileType[,] tiles, bool[,] visited, int startX, int startY, 
            int undergroundHeight, System.Collections.Generic.List<(int x, int y)> region)
        {
            var stack = new System.Collections.Generic.Stack<(int x, int y)>();
            stack.Push((startX, startY));
            
            while (stack.Count > 0)
            {
                var (x, y) = stack.Pop();
                
                if (x < 1 || x >= _mapWidth - 1 || y < 1 || y >= undergroundHeight - 1) continue;
                if (visited[x, y] || tiles[x, y] != TileType.Empty) continue;
                
                visited[x, y] = true;
                region.Add((x, y));
                
                // 4方向に拡張
                stack.Push((x + 1, y));
                stack.Push((x - 1, y));
                stack.Push((x, y + 1));
                stack.Push((x, y - 1));
            }
        }
        
        private void ConnectRegions(TileType[,] tiles, 
            System.Collections.Generic.List<System.Collections.Generic.List<(int x, int y)>> regions, 
            Random random, int undergroundHeight)
        {
            // 最大の領域を見つける
            var mainRegion = regions[0];
            foreach (var region in regions)
            {
                if (region.Count > mainRegion.Count)
                {
                    mainRegion = region;
                }
            }
            
            // 他の領域を主要領域に接続
            foreach (var region in regions)
            {
                if (region == mainRegion) continue;
                
                ConnectTwoRegions(tiles, mainRegion, region, random, undergroundHeight);
            }
        }
        
        private void ConnectTwoRegions(TileType[,] tiles, 
            System.Collections.Generic.List<(int x, int y)> region1,
            System.Collections.Generic.List<(int x, int y)> region2,
            Random random, int undergroundHeight)
        {
            // 最も近い2点を見つけて接続
            var point1 = region1[random.Next(region1.Count)];
            var point2 = region2[random.Next(region2.Count)];
            
            // L字型の通路で接続
            CreateLShapedTunnel(tiles, point1.x, point1.y, point2.x, point2.y, undergroundHeight);
        }
        
        private void CreateLShapedTunnel(TileType[,] tiles, int x1, int y1, int x2, int y2, int undergroundHeight)
        {
            // 水平方向に移動
            int currentX = x1;
            int targetX = x2;
            int direction = targetX > currentX ? 1 : -1;
            
            while (currentX != targetX)
            {
                if (currentX > 0 && currentX < _mapWidth - 1 && y1 > 0 && y1 < undergroundHeight - 1)
                {
                    tiles[currentX, y1] = TileType.Empty;
                }
                currentX += direction;
            }
            
            // 垂直方向に移動
            int currentY = y1;
            int targetY = y2;
            direction = targetY > currentY ? 1 : -1;
            
            while (currentY != targetY)
            {
                if (x2 > 0 && x2 < _mapWidth - 1 && currentY > 0 && currentY < undergroundHeight - 1)
                {
                    tiles[x2, currentY] = TileType.Empty;
                }
                currentY += direction;
            }
        }
        
        private void SmoothTerrain(TileType[,] tiles, Random random)
        {
            var undergroundHeight = _mapHeight - _groundAreaHeight;
            
            // 小さな孤立した壁や空間を除去
            for (int x = 1; x < _mapWidth - 1; x++)
            {
                for (int y = 1; y < undergroundHeight - 1; y++)
                {
                    int sameTypeCount = CountSameTypeNeighbors(tiles, x, y, undergroundHeight);
                    
                    // 周囲の大部分が異なるタイプの場合、そちらに合わせる
                    if (sameTypeCount <= 2)
                    {
                        tiles[x, y] = tiles[x, y] == TileType.Wall ? TileType.Empty : TileType.Wall;
                    }
                }
            }
        }
        
        private int CountSameTypeNeighbors(TileType[,] tiles, int centerX, int centerY, int undergroundHeight)
        {
            var centerType = tiles[centerX, centerY];
            int count = 0;
            
            for (int x = centerX - 1; x <= centerX + 1; x++)
            {
                for (int y = centerY - 1; y <= centerY + 1; y++)
                {
                    if (x == centerX && y == centerY) continue;
                    
                    if (x >= 0 && x < _mapWidth && y >= 0 && y < undergroundHeight)
                    {
                        if (tiles[x, y] == centerType)
                        {
                            count++;
                        }
                    }
                }
            }
            
            return count;
        }
    }
}
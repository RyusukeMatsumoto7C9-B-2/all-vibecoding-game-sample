using System;
using NUnit.Framework;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem.Generation;

namespace MyGame.TilemapSystem.Tests.EditMode
{
    [Description("プロシージャル地形生成システムのテスト")]
    public class ProceduralGeneratorTests
    {
        private ProceduralGenerator _generator;
        private const int TEST_WIDTH = 20;
        private const int TEST_HEIGHT = 30;
        private const int TEST_GROUND_AREA_HEIGHT = 5;

        [SetUp]
        public void Setup()
        {
            _generator = new ProceduralGenerator(TEST_WIDTH, TEST_HEIGHT, TEST_GROUND_AREA_HEIGHT);
        }

        [Test]
        [Description("地形生成で正しいサイズのマップが作成されることを検証")]
        public void GenerateTerrain_WithValidParameters_CreatesCorrectSizeMap()
        {
            var random = new Random(12345);
            
            var tiles = _generator.GenerateTerrain(random, 1);
            
            Assert.AreEqual(TEST_WIDTH, tiles.GetLength(0));
            Assert.AreEqual(TEST_HEIGHT, tiles.GetLength(1));
        }

        [Test]
        [Description("レベル1で上5マスがSkyブロック、残りがGroundまたはRockで埋められることを検証")]
        public void GenerateTerrain_Level1_HasSkyBlocksOnTop()
        {
            var random = new Random(12345);
            
            var tiles = _generator.GenerateTerrain(random, 1);
            
            // 上5マス分（y座標が25以上）がSkyブロックであることを確認
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = TEST_HEIGHT - TEST_GROUND_AREA_HEIGHT; y < TEST_HEIGHT; y++)
                {
                    Assert.AreEqual(TileType.Sky, tiles[x, y], 
                        $"座標({x}, {y})はSkyブロックであるべき。実際の値: {tiles[x, y]}");
                }
            }
            
            // 残りのマスがGroundまたはRockで埋められていることを確認
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT - TEST_GROUND_AREA_HEIGHT; y++)
                {
                    Assert.IsTrue(tiles[x, y] == TileType.Ground || tiles[x, y] == TileType.Rock, 
                        $"座標({x}, {y})はGroundまたはRockであるべき。実際の値: {tiles[x, y]}");
                }
            }
        }
        
        [Test]
        [Description("レベル2以降で全てのマスがGroundまたはRockで埋められることを検証")]
        public void GenerateTerrain_Level2AndAbove_FillsAllTilesWithoutGaps()
        {
            var random = new Random(12345);
            
            var tiles = _generator.GenerateTerrain(random, 2);
            
            // 全てのマスがGroundまたはRockタイプで埋められていることを確認
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    Assert.IsTrue(tiles[x, y] == TileType.Ground || tiles[x, y] == TileType.Rock, 
                        $"座標({x}, {y})にタイルが配置されていない。実際の値: {tiles[x, y]}");
                }
            }
        }

        [Test]
        [Description("同一シードで同一の結果が生成されることを検証")]
        public void GenerateTerrain_SameSeed_ProducesSameResults()
        {
            var random1 = new Random(12345);
            var random2 = new Random(12345); // 同一シード
            
            var tiles1 = _generator.GenerateTerrain(random1, 1);
            var tiles2 = _generator.GenerateTerrain(random2, 1);
            
            // 同一シードで同じ結果になる
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    Assert.AreEqual(tiles1[x, y], tiles2[x, y], 
                        $"座標({x}, {y})で同一シードでも結果が一致しない");
                }
            }
        }

        [Test]
        [Description("Rockの数が3~5個の範囲内で配置されることを検証")]
        public void GenerateTerrain_Always_Creates3To5Rocks()
        {
            var random = new Random(42);
            
            var tiles = _generator.GenerateTerrain(random, 2);
            
            int rockCount = 0;
            
            // Rockの数をカウント
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    if (tiles[x, y] == TileType.Rock)
                    {
                        rockCount++;
                    }
                }
            }
            
            // Rockの数が3~5個の範囲内であることを確認
            Assert.IsTrue(rockCount >= 3 && rockCount <= 5, 
                $"Rockの数が仕様範囲外: {rockCount}個 (期待: 3~5個)");
        }

        [Test]
        [Description("カスタムサイズでの地形生成が正常に動作することを検証")]
        public void GenerateTerrain_WithCustomSize_WorksCorrectly()
        {
            const int customWidth = 15;
            const int customHeight = 20;
            const int customGroundHeight = 3;
            
            var customGenerator = new ProceduralGenerator(customWidth, customHeight, customGroundHeight);
            var random = new Random(12345);
            
            var tiles = customGenerator.GenerateTerrain(random, 2);
            
            Assert.AreEqual(customWidth, tiles.GetLength(0));
            Assert.AreEqual(customHeight, tiles.GetLength(1));
            
            // 全てのマスがRockまたはGroundタイプで埋められていることを確認
            for (int x = 0; x < customWidth; x++)
            {
                for (int y = 0; y < customHeight; y++)
                {
                    Assert.IsTrue(tiles[x, y] == TileType.Rock || tiles[x, y] == TileType.Ground, 
                        $"カスタムサイズでの座標({x}, {y})にタイルが配置されていない");
                }
            }
        }

        [Test]
        [Description("複数回の生成でRock数が一定範囲内であることを検証")]
        public void GenerateTerrain_MultipleGenerations_ConsistentRockCount()
        {
            const int iterations = 10;
            
            for (int i = 0; i < iterations; i++)
            {
                var tiles = _generator.GenerateTerrain(new Random(i), 2);
                
                int rockCount = 0;
                for (int x = 0; x < TEST_WIDTH; x++)
                {
                    for (int y = 0; y < TEST_HEIGHT; y++)
                    {
                        if (tiles[x, y] == TileType.Rock)
                        {
                            rockCount++;
                        }
                    }
                }
                
                Assert.IsTrue(rockCount >= 3 && rockCount <= 5, 
                    $"反復{i + 1}: Rock数が範囲外 {rockCount}個 (期待: 3~5個)");
            }
        }

        [Test]
        [Description("現在の実装でTreasureタイルが生成されないことを検証")]
        public void GenerateTerrain_CurrentImplementation_DoesNotGenerateTreasures()
        {
            var random = new Random(42);
            
            var tiles = _generator.GenerateTerrain(random, 1);
            
            int treasureCount = 0;
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    if (tiles[x, y] == TileType.Treasure)
                    {
                        treasureCount++;
                    }
                }
            }
            
            Assert.AreEqual(0, treasureCount, 
                $"現在の実装ではTreasureタイルは生成されない予定: {treasureCount}個");
        }

        [Test]
        [Description("Skyタイルが指定された範囲にのみ生成されることを検証")]
        public void GenerateTerrain_SkyTiles_OnlyInCorrectRegion()
        {
            var random = new Random(42);
            
            var tiles = _generator.GenerateTerrain(random, 1);
            
            // Sky領域外にSkyタイルが存在しないことを確認
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT - TEST_GROUND_AREA_HEIGHT; y++)
                {
                    Assert.AreNotEqual(TileType.Sky, tiles[x, y], 
                        $"座標({x}, {y})でSky領域外にSkyタイルが存在する");
                }
            }
        }

        [Test]
        [Description("全てのタイルタイプが有効な値であることを検証")]
        public void GenerateTerrain_AllTileTypes_AreValid()
        {
            var random = new Random(42);
            
            var tiles = _generator.GenerateTerrain(random, 1);
            
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    var tileType = tiles[x, y];
                    Assert.IsTrue(
                        tileType == TileType.Sky || 
                        tileType == TileType.Empty || 
                        tileType == TileType.Ground || 
                        tileType == TileType.Rock || 
                        tileType == TileType.Treasure,
                        $"座標({x}, {y})で無効なタイルタイプ: {tileType}");
                }
            }
        }

    }
}
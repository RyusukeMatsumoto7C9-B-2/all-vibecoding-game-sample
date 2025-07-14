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
            
            var tiles = _generator.GenerateTerrain(random);
            
            Assert.AreEqual(TEST_WIDTH, tiles.GetLength(0));
            Assert.AreEqual(TEST_HEIGHT, tiles.GetLength(1));
        }

        [Test]
        [Description("全てのマスにタイルが隙間なく配置されることを検証")]
        public void GenerateTerrain_Always_FillsAllTilesWithoutGaps()
        {
            var random = new Random(12345);
            
            var tiles = _generator.GenerateTerrain(random);
            
            // 全てのマスがWallまたはGroundタイプで埋められていることを確認
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    Assert.IsTrue(tiles[x, y] == TileType.Wall || tiles[x, y] == TileType.Ground, 
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
            
            var tiles1 = _generator.GenerateTerrain(random1);
            var tiles2 = _generator.GenerateTerrain(random2);
            
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
        [Description("Wallの数が3~5個の範囲内で配置されることを検証")]
        public void GenerateTerrain_Always_Creates3To5Walls()
        {
            var random = new Random(42);
            
            var tiles = _generator.GenerateTerrain(random);
            
            int wallCount = 0;
            
            // Wallの数をカウント
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    if (tiles[x, y] == TileType.Wall)
                    {
                        wallCount++;
                    }
                }
            }
            
            // Wallの数が3~5個の範囲内であることを確認
            Assert.IsTrue(wallCount >= 3 && wallCount <= 5, 
                $"Wallの数が仕様範囲外: {wallCount}個 (期待: 3~5個)");
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
            
            var tiles = customGenerator.GenerateTerrain(random);
            
            Assert.AreEqual(customWidth, tiles.GetLength(0));
            Assert.AreEqual(customHeight, tiles.GetLength(1));
            
            // 全てのマスがWallまたはGroundタイプで埋められていることを確認
            for (int x = 0; x < customWidth; x++)
            {
                for (int y = 0; y < customHeight; y++)
                {
                    Assert.IsTrue(tiles[x, y] == TileType.Wall || tiles[x, y] == TileType.Ground, 
                        $"カスタムサイズでの座標({x}, {y})にタイルが配置されていない");
                }
            }
        }

        [Test]
        [Description("複数回の生成でWall数が一定範囲内であることを検証")]
        public void GenerateTerrain_MultipleGenerations_ConsistentWallCount()
        {
            const int iterations = 10;
            
            for (int i = 0; i < iterations; i++)
            {
                var tiles = _generator.GenerateTerrain(new Random(i));
                
                int wallCount = 0;
                for (int x = 0; x < TEST_WIDTH; x++)
                {
                    for (int y = 0; y < TEST_HEIGHT; y++)
                    {
                        if (tiles[x, y] == TileType.Wall)
                        {
                            wallCount++;
                        }
                    }
                }
                
                Assert.IsTrue(wallCount >= 3 && wallCount <= 5, 
                    $"反復{i + 1}: Wall数が範囲外 {wallCount}個 (期待: 3~5個)");
            }
        }

    }
}
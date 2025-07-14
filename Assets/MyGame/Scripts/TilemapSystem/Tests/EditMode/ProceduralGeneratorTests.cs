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
        [Description("ランダムにWallが配置され適切な割合で生成されることを検証")]
        public void GenerateTerrain_Always_CreatesReasonableWallDistribution()
        {
            var random = new Random(42);
            
            var tiles = _generator.GenerateTerrain(random);
            
            int wallCount = 0;
            int totalTiles = TEST_WIDTH * TEST_HEIGHT;
            
            // WallとGroundの数をカウント
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
            
            double wallRatio = (double)wallCount / totalTiles;
            
            // Wallの割合が10%～50%の範囲内であることを確認
            Assert.IsTrue(wallRatio >= 0.1 && wallRatio <= 0.5, 
                $"Wallの割合が適切でない: {wallRatio:P2} (期待範囲: 10%-50%)");
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
        [Description("異なるシードで異なる結果が生成されることを検証")]
        public void GenerateTerrain_DifferentSeeds_ProduceDifferentResults()
        {
            var tiles1 = _generator.GenerateTerrain(new Random(12345));
            var tiles2 = _generator.GenerateTerrain(new Random(67890));
            
            bool foundDifference = false;
            
            // 異なるシードで生成されたマップが異なることを確認
            for (int x = 0; x < TEST_WIDTH && !foundDifference; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    if (tiles1[x, y] != tiles2[x, y])
                    {
                        foundDifference = true;
                        break;
                    }
                }
            }
            
            Assert.IsTrue(foundDifference, "異なるシードで生成したマップに差異がない");
        }

    }
}
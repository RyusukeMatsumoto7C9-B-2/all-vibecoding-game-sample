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
        [Description("地上エリア（上5マス）が空間として設定されることを検証")]
        public void GenerateTerrain_Always_CreatesGroundAreaAsEmpty()
        {
            var random = new Random(12345);
            
            var tiles = _generator.GenerateTerrain(random);
            
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = TEST_HEIGHT - TEST_GROUND_AREA_HEIGHT; y < TEST_HEIGHT; y++)
                {
                    Assert.AreEqual(TileType.Empty, tiles[x, y], 
                        $"地上エリアの座標({x}, {y})が空間でない");
                }
            }
        }

        [Test]
        [Description("マップの境界が壁として設定されることを検証")]
        public void GenerateTerrain_Always_CreatesBoundaryWalls()
        {
            var random = new Random(12345);
            
            var tiles = _generator.GenerateTerrain(random);
            var undergroundHeight = TEST_HEIGHT - TEST_GROUND_AREA_HEIGHT;
            
            // 左右の境界をチェック
            for (int y = 0; y < undergroundHeight; y++)
            {
                Assert.AreEqual(TileType.Wall, tiles[0, y], $"左境界の座標(0, {y})が壁でない");
                Assert.AreEqual(TileType.Wall, tiles[TEST_WIDTH - 1, y], 
                    $"右境界の座標({TEST_WIDTH - 1}, {y})が壁でない");
            }
            
            // 下境界をチェック
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                Assert.AreEqual(TileType.Wall, tiles[x, 0], $"下境界の座標({x}, 0)が壁でない");
            }
        }

        [Test]
        [Description("同じシードで生成した地形が一致することを検証")]
        public void GenerateTerrain_WithSameSeed_ProducesSameResult()
        {
            const int seed = 42;
            var random1 = new Random(seed);
            var random2 = new Random(seed);
            
            var tiles1 = _generator.GenerateTerrain(random1);
            var tiles2 = _generator.GenerateTerrain(random2);
            
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    Assert.AreEqual(tiles1[x, y], tiles2[x, y], 
                        $"座標({x}, {y})のタイルタイプが一致しない");
                }
            }
        }

        [Test]
        [Description("異なるシードで生成した地形が異なることを検証")]
        public void GenerateTerrain_WithDifferentSeeds_ProducesDifferentResults()
        {
            var random1 = new Random(123);
            var random2 = new Random(456);
            
            var tiles1 = _generator.GenerateTerrain(random1);
            var tiles2 = _generator.GenerateTerrain(random2);
            
            bool foundDifference = false;
            for (int x = 1; x < TEST_WIDTH - 1 && !foundDifference; x++)
            {
                for (int y = 1; y < TEST_HEIGHT - TEST_GROUND_AREA_HEIGHT - 1; y++)
                {
                    if (tiles1[x, y] != tiles2[x, y])
                    {
                        foundDifference = true;
                        break;
                    }
                }
            }
            
            Assert.IsTrue(foundDifference, "異なるシードで生成した地形に差異が見つからない");
        }

        [Test]
        [Description("生成された地形に通路が確保されていることを検証")]
        public void GenerateTerrain_Always_EnsuresPassageways()
        {
            var random = new Random(12345);
            
            var tiles = _generator.GenerateTerrain(random);
            var undergroundHeight = TEST_HEIGHT - TEST_GROUND_AREA_HEIGHT;
            
            // 中央縦通路の存在確認
            int centerX = TEST_WIDTH / 2;
            bool hasVerticalPassage = true;
            
            for (int y = 1; y < undergroundHeight - 1; y++)
            {
                if (tiles[centerX, y] != TileType.Empty && 
                    tiles[centerX - 1, y] != TileType.Empty && 
                    tiles[centerX + 1, y] != TileType.Empty)
                {
                    hasVerticalPassage = false;
                    break;
                }
            }
            
            Assert.IsTrue(hasVerticalPassage, "中央縦方向の通路が確保されていない");
        }

        [Test]
        [Description("生成された地形に適切な密度の壁と空間が配置されることを検証")]
        public void GenerateTerrain_Always_CreatesDiverseTerrain()
        {
            var random = new Random(12345);
            
            var tiles = _generator.GenerateTerrain(random);
            var undergroundHeight = TEST_HEIGHT - TEST_GROUND_AREA_HEIGHT;
            
            int wallCount = 0;
            int emptyCount = 0;
            
            // 境界を除いた内部エリアをチェック
            for (int x = 1; x < TEST_WIDTH - 1; x++)
            {
                for (int y = 1; y < undergroundHeight - 1; y++)
                {
                    if (tiles[x, y] == TileType.Wall)
                        wallCount++;
                    else if (tiles[x, y] == TileType.Empty)
                        emptyCount++;
                }
            }
            
            int totalTiles = wallCount + emptyCount;
            double wallRatio = (double)wallCount / totalTiles;
            
            // 壁の比率が10%〜70%の範囲内であることを確認（極端でない）
            Assert.IsTrue(wallRatio >= 0.1 && wallRatio <= 0.7, 
                $"壁の比率が適切でない: {wallRatio:P2} (期待範囲: 10%-70%)");
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
            
            // 地上エリアの確認
            for (int x = 0; x < customWidth; x++)
            {
                for (int y = customHeight - customGroundHeight; y < customHeight; y++)
                {
                    Assert.AreEqual(TileType.Empty, tiles[x, y], 
                        $"カスタムサイズでの地上エリア座標({x}, {y})が空間でない");
                }
            }
        }

        [Test]
        [Description("複数回の生成で安定した結果を得られることを検証")]
        public void GenerateTerrain_MultipleGenerations_ProducesStableResults()
        {
            const int iterations = 10;
            var baseTiles = _generator.GenerateTerrain(new Random(12345));
            
            for (int i = 0; i < iterations; i++)
            {
                var tiles = _generator.GenerateTerrain(new Random(12345));
                
                for (int x = 0; x < TEST_WIDTH; x++)
                {
                    for (int y = 0; y < TEST_HEIGHT; y++)
                    {
                        Assert.AreEqual(baseTiles[x, y], tiles[x, y], 
                            $"反復{i}の座標({x}, {y})で不一致が発生");
                    }
                }
            }
        }
    }
}
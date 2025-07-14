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
            
            // 全てのマスがWallタイプで埋められていることを確認
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    Assert.AreEqual(TileType.Wall, tiles[x, y], 
                        $"座標({x}, {y})にタイルが配置されていない");
                }
            }
        }

        [Test]
        [Description("ランダム要素なしで決定的な結果が生成されることを検証")]
        public void GenerateTerrain_WithoutRandomElements_ProducesDeterministicResults()
        {
            var random1 = new Random(12345);
            var random2 = new Random(67890); // 異なるシードでも同じ結果になるはず
            
            var tiles1 = _generator.GenerateTerrain(random1);
            var tiles2 = _generator.GenerateTerrain(random2);
            
            // ランダム要素がないので、シードが違っても同じ結果になる
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    Assert.AreEqual(tiles1[x, y], tiles2[x, y], 
                        $"座標({x}, {y})で異なるシードでも結果が一致しない");
                }
            }
        }

        [Test]
        [Description("全てのタイルがWallタイプで配置されることを検証")]
        public void GenerateTerrain_Always_CreatesAllWallTiles()
        {
            var random = new Random(42);
            
            var tiles = _generator.GenerateTerrain(random);
            
            // 全てのタイルがWallタイプであることを確認
            for (int x = 0; x < TEST_WIDTH; x++)
            {
                for (int y = 0; y < TEST_HEIGHT; y++)
                {
                    Assert.AreEqual(TileType.Wall, tiles[x, y], 
                        $"座標({x}, {y})がWallタイプでない");
                }
            }
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
            
            // 全てのマスがWallタイプで埋められていることを確認
            for (int x = 0; x < customWidth; x++)
            {
                for (int y = 0; y < customHeight; y++)
                {
                    Assert.AreEqual(TileType.Wall, tiles[x, y], 
                        $"カスタムサイズでの座標({x}, {y})がWallタイプでない");
                }
            }
        }

        [Test]
        [Description("複数回の生成で安定した結果を得られることを検証")]
        public void GenerateTerrain_MultipleGenerations_ProducesStableResults()
        {
            const int iterations = 5;
            var baseTiles = _generator.GenerateTerrain(new Random(12345));
            
            for (int i = 0; i < iterations; i++)
            {
                var tiles = _generator.GenerateTerrain(new Random(67890)); // 異なるシードでも同じ結果
                
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
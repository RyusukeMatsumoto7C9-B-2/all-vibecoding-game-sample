using NUnit.Framework;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem.Generation;

namespace MyGame.TilemapSystem.Tests
{
    [Description("タイルマップ生成システムのテスト")]
    public class TilemapGeneratorTests
    {
        private TilemapGenerator _generator;
        private SeedManager _seedManager;

        [SetUp]
        public void SetUp()
        {
            _seedManager = new SeedManager();
            _generator = new TilemapGenerator(_seedManager);
        }

        [Test]
        [Description("指定サイズでマップが正しく生成されることを検証")]
        public void GenerateMap_WithSpecifiedSize_CreatesCorrectSizeMap()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);

            Assert.AreEqual(TilemapGenerator.MAP_WIDTH, mapData.Width);
            Assert.AreEqual(TilemapGenerator.MAP_HEIGHT, mapData.Height);
            Assert.AreEqual(level, mapData.Level);
            Assert.AreEqual(seed, mapData.Seed);
            Assert.IsNotNull(mapData.Tiles);
        }

        [Test]
        [Description("全マスがタイルで埋められることを検証（現在の実装）")]
        public void GenerateMap_AllTiles_AreFilled()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);

            // 全マスがGroundまたはWallで埋められていることを確認
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    Assert.IsTrue(mapData.Tiles[x, y] == TileType.Ground || mapData.Tiles[x, y] == TileType.Wall, 
                        $"位置({x}, {y})にタイルが配置されていない。実際の値: {mapData.Tiles[x, y]}");
                }
            }
        }

        [Test]
        [Description("同一シードで同一マップが生成されることを検証")]
        public void GenerateMap_SameSeed_GeneratesSameMap()
        {
            var level = 1;
            var seed = 12345;

            var mapData1 = _generator.GenerateMap(level, seed);
            var mapData2 = _generator.GenerateMap(level, seed);

            Assert.AreEqual(mapData1.Width, mapData2.Width);
            Assert.AreEqual(mapData1.Height, mapData2.Height);
            
            for (int x = 0; x < mapData1.Width; x++)
            {
                for (int y = 0; y < mapData1.Height; y++)
                {
                    Assert.AreEqual(mapData1.Tiles[x, y], mapData2.Tiles[x, y], 
                        $"位置({x}, {y})でタイルが異なります");
                }
            }
        }

        [Test]
        [Description("Wallの数が適切な範囲内であることを検証（現在の実装）")]
        public void GenerateMap_WallCount_IsWithinExpectedRange()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);

            int wallCount = 0;
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    if (mapData.Tiles[x, y] == TileType.Wall)
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
        [Description("マップ生成がシードベースで再現可能であることを検証")]
        public void GenerateMap_SeedBased_IsReproducible()
        {
            var level1 = 1;
            var level2 = 2;
            var seed = 12345;

            var mapData1a = _generator.GenerateMap(level1, seed);
            var mapData1b = _generator.GenerateMap(level1, seed);
            var mapData2 = _generator.GenerateMap(level2, seed);

            // 同じレベル、同じシードで同じ結果
            for (int x = 0; x < mapData1a.Width; x++)
            {
                for (int y = 0; y < mapData1a.Height; y++)
                {
                    Assert.AreEqual(mapData1a.Tiles[x, y], mapData1b.Tiles[x, y], 
                        $"同じレベル・シードで異なる結果: 位置({x}, {y})");
                }
            }

            // 異なるレベルでは異なる結果が生成される可能性が高い
            Assert.AreNotEqual(mapData1a.Level, mapData2.Level, "レベルが異なっていることを確認");
        }
    }
}
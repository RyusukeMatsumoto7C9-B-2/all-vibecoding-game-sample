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
        [Description("地上エリアが正しく空間として設定されることを検証")]
        public void GenerateMap_GroundArea_IsEmpty()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);

            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = mapData.Height - TilemapGenerator.GROUND_AREA_HEIGHT; y < mapData.Height; y++)
                {
                    Assert.AreEqual(TileType.Empty, mapData.Tiles[x, y], 
                        $"地上エリア位置({x}, {y})が空間ではありません");
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
        [Description("境界が壁で設定されることを検証")]
        public void GenerateMap_Boundaries_AreWalls()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);
            var undergroundHeight = mapData.Height - TilemapGenerator.GROUND_AREA_HEIGHT;

            // 左右の境界をチェック
            for (int y = 0; y < undergroundHeight; y++)
            {
                Assert.AreEqual(TileType.Wall, mapData.Tiles[0, y], $"左境界位置(0, {y})が壁ではありません");
                Assert.AreEqual(TileType.Wall, mapData.Tiles[mapData.Width - 1, y], $"右境界位置({mapData.Width - 1}, {y})が壁ではありません");
            }

            // 下の境界をチェック
            for (int x = 0; x < mapData.Width; x++)
            {
                Assert.AreEqual(TileType.Wall, mapData.Tiles[x, 0], $"下境界位置({x}, 0)が壁ではありません");
            }
        }

        [Test]
        [Description("中央通路が確保されることを検証")]
        public void GenerateMap_CenterPassage_IsEmpty()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);
            var centerX = mapData.Width / 2;
            var undergroundHeight = mapData.Height - TilemapGenerator.GROUND_AREA_HEIGHT;

            for (int y = 1; y < undergroundHeight - 1; y++)
            {
                Assert.AreEqual(TileType.Empty, mapData.Tiles[centerX, y], 
                    $"中央通路位置({centerX}, {y})が空間ではありません");
            }
        }
    }
}
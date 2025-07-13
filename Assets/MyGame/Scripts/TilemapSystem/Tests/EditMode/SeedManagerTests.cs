using NUnit.Framework;
using MyGame.TilemapSystem.Generation;

namespace MyGame.TilemapSystem.Tests
{
    [Description("シード管理システムのテスト")]
    public class SeedManagerTests
    {
        private SeedManager _seedManager;

        [SetUp]
        public void SetUp()
        {
            _seedManager = new SeedManager();
        }

        [Test]
        [Description("レベルごとに異なるシードが生成されることを検証")]
        public void GetSeedForLevel_DifferentLevels_ReturnsDifferentSeeds()
        {
            var seed1 = _seedManager.GetSeedForLevel(1);
            var seed2 = _seedManager.GetSeedForLevel(2);

            Assert.AreNotEqual(seed1, seed2);
        }

        [Test]
        [Description("同一レベルでは同一シードが生成されることを検証")]
        public void GetSeedForLevel_SameLevel_ReturnsSameSeed()
        {
            var seed1 = _seedManager.GetSeedForLevel(1);
            var seed2 = _seedManager.GetSeedForLevel(1);

            Assert.AreEqual(seed1, seed2);
        }

        [Test]
        [Description("ベースシード設定後に期待される値が取得できることを検証")]
        public void SetSeed_ThenGetSeedForLevel_ReturnsExpectedValue()
        {
            var baseSeed = 12345;
            var level = 2;
            var expectedSeed = baseSeed + level * 1000;

            _seedManager.SetSeed(baseSeed);
            var actualSeed = _seedManager.GetSeedForLevel(level);

            Assert.AreEqual(expectedSeed, actualSeed);
        }

        [Test]
        [Description("レベル用乱数生成器が正しく作成されることを検証")]
        public void CreateRandomForLevel_ValidLevel_ReturnsRandomInstance()
        {
            var level = 1;

            var random = _seedManager.CreateRandomForLevel(level);

            Assert.IsNotNull(random);
        }

        [Test]
        [Description("同一レベル用乱数生成器が同じ値を生成することを検証")]
        public void CreateRandomForLevel_SameLevel_GeneratesSameSequence()
        {
            var level = 1;

            var random1 = _seedManager.CreateRandomForLevel(level);
            var random2 = _seedManager.CreateRandomForLevel(level);

            var value1 = random1.Next(100);
            var value2 = random2.Next(100);

            Assert.AreEqual(value1, value2);
        }
    }
}
using NUnit.Framework;
using UnityEngine;
using MyGame.TilemapSystem.Core;

namespace MyGame.Player.Tests
{
    [TestFixture]
    [Description("プレイヤー移動制約機能の統合テスト：ブロックタイプによる移動制約がPlayer仕様に従って動作することを検証")]
    public class PlayerMovementConstraintTests
    {
        private TileBehavior _tileBehavior;

        [SetUp]
        public void SetUp()
        {
            _tileBehavior = new TileBehavior();
        }

        [Test]
        [Description("Emptyブロック上は移動可能であることを検証")]
        public void CanPlayerPassThrough_EmptyBlock_ShouldReturnTrue()
        {
            // Act
            var result = _tileBehavior.CanPlayerPassThrough(BlockType.Empty);

            // Assert
            Assert.IsTrue(result, "Emptyブロック上は移動可能であるべき");
        }

        [Test]
        [Description("Groundブロック上は移動可能であることを検証")]
        public void CanPlayerPassThrough_GroundBlock_ShouldReturnTrue()
        {
            // Act
            var result = _tileBehavior.CanPlayerPassThrough(BlockType.Ground);

            // Assert
            Assert.IsTrue(result, "Groundブロック上は移動可能であるべき");
        }

        [Test]
        [Description("Rockブロック上は移動不可能であることを検証")]
        public void CanPlayerPassThrough_RockBlock_ShouldReturnFalse()
        {
            // Act
            var result = _tileBehavior.CanPlayerPassThrough(BlockType.Rock);

            // Assert
            Assert.IsFalse(result, "Rockブロック上は移動不可能であるべき");
        }

        [Test]
        [Description("Skyブロック上は移動不可能であることを検証（仕様変更）")]
        public void CanPlayerPassThrough_SkyBlock_ShouldReturnFalse()
        {
            // Act
            var result = _tileBehavior.CanPlayerPassThrough(BlockType.Sky);

            // Assert
            Assert.IsFalse(result, "Skyブロック上は移動不可能であるべき（プレイヤー仕様に従い）");
        }

        [Test]
        [Description("Treasureブロック上は移動可能であることを検証")]
        public void CanPlayerPassThrough_TreasureBlock_ShouldReturnTrue()
        {
            // Act
            var result = _tileBehavior.CanPlayerPassThrough(BlockType.Treasure);

            // Assert
            Assert.IsTrue(result, "Treasureブロック上は移動可能であるべき");
        }

        [TestCase(BlockType.Empty, true, TestName = "Empty通過可能")]
        [TestCase(BlockType.Ground, true, TestName = "Ground通過可能")]
        [TestCase(BlockType.Rock, false, TestName = "Rock通過不可")]
        [TestCase(BlockType.Sky, false, TestName = "Sky通過不可")]
        [TestCase(BlockType.Treasure, true, TestName = "Treasure通過可能")]
        [Description("各ブロックタイプの移動制約をパラメータ化テストで検証")]
        public void CanPlayerPassThrough_AllBlockTypes_ShouldFollowSpecification(BlockType blockType, bool expectedResult)
        {
            // Act
            var result = _tileBehavior.CanPlayerPassThrough(blockType);

            // Assert
            Assert.AreEqual(expectedResult, result, 
                $"{blockType}ブロックの移動制約が仕様と異なります。期待値: {expectedResult}, 実際の値: {result}");
        }
    }
}
using NUnit.Framework;
using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem;

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

        // TileBehaviorの個別テストはTilemapSystem.Tests.TileBehaviorTestsで実施されているため削除
        // Player側では新しい統合メソッドのテストのみ実施

        [TestCase(BlockType.Empty, true, TestName = "Empty通過可能")]
        [TestCase(BlockType.Ground, true, TestName = "Ground通過可能")]
        [TestCase(BlockType.Treasure, true, TestName = "Treasure通過可能")]
        [TestCase(BlockType.Rock, false, TestName = "Rock通過不可")]
        [TestCase(BlockType.Sky, false, TestName = "Sky通過不可")]
        [Description("新しいCanPassThroughメソッドによる統合移動制約をパラメータ化テストで検証")]
        public void CanPassThrough_NewUnifiedMethod_ShouldFollowNewSpecification(BlockType blockType, bool expectedResult)
        {
            // Arrange
            var mockManager = new MockTilemapManager(true);
            mockManager.SetBlockType(new Vector2Int(0, 0), blockType);

            // Act
            var result = mockManager.CanPassThrough(new Vector2Int(0, 0), 0);

            // Assert
            Assert.AreEqual(expectedResult, result, 
                $"新しいCanPassThroughメソッドで{blockType}ブロックの移動制約が期待値と異なります。期待値: {expectedResult}, 実際の値: {result}");
        }
    }
}
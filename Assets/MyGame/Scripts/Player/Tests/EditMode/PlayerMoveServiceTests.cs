using NUnit.Framework;
using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.Common;
using MyGame.TilemapSystem;

namespace MyGame.Player.Tests
{

    [TestFixture]
    [Description("PlayerMoveServiceクラスの移動ロジックとして、4方向移動と位置設定の正確性をテストする")]
    public class PlayerMoveServiceTests
    {
        private PlayerMoveService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new PlayerMoveService();
        }

        [Test]
        [Description("上方向への移動時に座標のY値が1増加することを検証（TilemapManager設定済み）")]
        public void Move_WithUpDirection_ShouldUpdatePositionUpward()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            // TilemapManagerを設定（移動可能とする）
            var testTilemapManager = new MockTilemapManager(true);
            _service.SetTilemapManager(testTilemapManager, 0);

            // Act
            _service.Move(Direction.Up);

            // Assert
            Assert.AreEqual(new Vector2Int(0, 1), _service.CurrentPosition);
        }

        [Test]
        [Description("下方向への移動時に座標のY値が1減少することを検証（TilemapManager設定済み）")]
        public void Move_WithDownDirection_ShouldUpdatePositionDownward()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 1);
            _service.SetPosition(initialPosition);

            // TilemapManagerを設定（移動可能とする）
            var testTilemapManager = new MockTilemapManager(true);
            _service.SetTilemapManager(testTilemapManager, 0);

            // Act
            _service.Move(Direction.Down);

            // Assert
            Assert.AreEqual(new Vector2Int(0, 0), _service.CurrentPosition);
        }

        [Test]
        [Description("左方向への移動時に座標のX値が1減少することを検証（TilemapManager設定済み）")]
        public void Move_WithLeftDirection_ShouldUpdatePositionLeft()
        {
            // Arrange
            var initialPosition = new Vector2Int(1, 0);
            _service.SetPosition(initialPosition);

            // TilemapManagerを設定（移動可能とする）
            var testTilemapManager = new MockTilemapManager(true);
            _service.SetTilemapManager(testTilemapManager, 0);

            // Act
            _service.Move(Direction.Left);

            // Assert
            Assert.AreEqual(new Vector2Int(0, 0), _service.CurrentPosition);
        }

        [Test]
        [Description("右方向への移動時に座標のX値が1増加することを検証（TilemapManager設定済み）")]
        public void Move_WithRightDirection_ShouldUpdatePositionRight()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            // TilemapManagerを設定（移動可能とする）
            var testTilemapManager = new MockTilemapManager(true);
            _service.SetTilemapManager(testTilemapManager, 0);

            // Act
            _service.Move(Direction.Right);

            // Assert
            Assert.AreEqual(new Vector2Int(1, 0), _service.CurrentPosition);
        }

        [Test]
        [Description("SetPositionメソッドで指定した座標にCurrentPositionが正確に設定されることを検証")]
        public void SetPosition_ShouldUpdateCurrentPosition()
        {
            // Arrange
            var newPosition = new Vector2Int(5, 3);

            // Act
            _service.SetPosition(newPosition);

            // Assert
            Assert.AreEqual(newPosition, _service.CurrentPosition);
        }

        [Test]
        [Description("初期配置座標（X10, Y15）が正確に設定されることを検証")]
        public void SetPosition_WithInitialPlayerPosition_ShouldSetCorrectPosition()
        {
            // Arrange
            var initialPlayerPosition = new Vector2Int(10, 15);

            // Act
            _service.SetPosition(initialPlayerPosition);

            // Assert
            Assert.AreEqual(initialPlayerPosition, _service.CurrentPosition);
            Assert.AreEqual(10, _service.CurrentPosition.x, "初期X座標が仕様と異なります");
            Assert.AreEqual(15, _service.CurrentPosition.y, "初期Y座標が仕様と異なります");
        }

        [Test]
        [Description("TilemapManagerが設定されていない場合、移動は失敗することを検証（安全性修正後）")]
        public void Move_WithoutTilemapManager_ShouldFailSafely()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            // Act
            var result = _service.Move(Direction.Up);

            // Assert
            Assert.IsFalse(result, "TilemapManagerが未設定の場合は安全のため移動が失敗すべき");
            Assert.AreEqual(initialPosition, _service.CurrentPosition, "移動失敗時は位置が変更されないべき");
        }

        [Test]
        [Description("移動先がEmpty/Groundブロックの場合、移動が成功することを検証")]
        public void Move_ToPassableTile_ShouldSucceed()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            var testTilemapManager = new MockTilemapManager(true);
            _service.SetTilemapManager(testTilemapManager, 0);

            // Act
            var result = _service.Move(Direction.Up);

            // Assert
            Assert.IsTrue(result, "移動可能なタイルへの移動は成功すべき");
            Assert.AreEqual(new Vector2Int(0, 1), _service.CurrentPosition);
        }

        [Test]
        [Description("移動先がRock/Skyブロックの場合、移動が失敗することを検証")]
        public void Move_ToImpassableTile_ShouldFail()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            var testTilemapManager = new MockTilemapManager(false);
            _service.SetTilemapManager(testTilemapManager, 0);

            // Act
            var result = _service.Move(Direction.Up);

            // Assert
            Assert.IsFalse(result, "移動不可能なタイルへの移動は失敗すべき");
            Assert.AreEqual(initialPosition, _service.CurrentPosition, "位置は変更されないべき");
        }

        [Test]
        [Description("CanMoveメソッドが正しく動作することを検証")]
        public void CanMove_WithPassableTile_ShouldReturnTrue()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            var testTilemapManager = new MockTilemapManager(true);
            _service.SetTilemapManager(testTilemapManager, 0);

            // Act
            var canMove = _service.CanMove(Direction.Right);

            // Assert
            Assert.IsTrue(canMove, "移動可能なタイルに対してCanMoveはtrueを返すべき");
        }

        [Test]
        [Description("CanMoveメソッドがRockブロックに対して移動不可を返すことを検証")]
        public void CanMove_WithImpassableTile_ShouldReturnFalse()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            var testTilemapManager = new MockTilemapManager(false);
            _service.SetTilemapManager(testTilemapManager, 0);

            // Act
            var canMove = _service.CanMove(Direction.Right);

            // Assert
            Assert.IsFalse(canMove, "移動不可能なタイルに対してCanMoveはfalseを返すべき");
        }

        [Test]
        [Description("Groundブロックに移動した場合、OnPlayerHitTileが呼ばれることを検証")]
        public void Move_ToGroundBlock_ShouldCallOnPlayerHitTile()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            var testTilemapManager = new MockTilemapManager(true, BlockType.Ground);
            _service.SetTilemapManager(testTilemapManager, 0);

            // Act
            var result = _service.Move(Direction.Right);

            // Assert
            Assert.IsTrue(result, "Groundブロックへの移動は成功すべき");
            Assert.AreEqual(new Vector2Int(1, 0), _service.CurrentPosition, "移動位置が正しく更新されるべき");
            Assert.IsTrue(testTilemapManager.OnPlayerHitTileCalled, "Groundブロック移動時にOnPlayerHitTileが呼ばれるべき");
            Assert.AreEqual(new Vector2Int(1, 0), testTilemapManager.LastHitPosition, "破壊処理の座標が正しいべき");
            Assert.AreEqual(0, testTilemapManager.LastHitLevel, "破壊処理のレベルが正しいべき");
        }

        [Test]
        [Description("Emptyブロックに移動した場合、OnPlayerHitTileが呼ばれないことを検証")]
        public void Move_ToEmptyBlock_ShouldNotCallOnPlayerHitTile()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            var testTilemapManager = new MockTilemapManager(true, BlockType.Empty);
            _service.SetTilemapManager(testTilemapManager, 0);

            // Act
            var result = _service.Move(Direction.Right);

            // Assert
            Assert.IsTrue(result, "Emptyブロックへの移動は成功すべき");
            Assert.AreEqual(new Vector2Int(1, 0), _service.CurrentPosition, "移動位置が正しく更新されるべき");
            Assert.IsFalse(testTilemapManager.OnPlayerHitTileCalled, "Emptyブロック移動時はOnPlayerHitTileが呼ばれないべき");
        }

        [Test]
        [Description("Treasureブロックに移動した場合、OnPlayerHitTileが呼ばれないことを検証")]
        public void Move_ToTreasureBlock_ShouldNotCallOnPlayerHitTile()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            var testTilemapManager = new MockTilemapManager(true, BlockType.Treasure);
            _service.SetTilemapManager(testTilemapManager, 0);

            // Act
            var result = _service.Move(Direction.Right);

            // Assert
            Assert.IsTrue(result, "Treasureブロックへの移動は成功すべき");
            Assert.AreEqual(new Vector2Int(1, 0), _service.CurrentPosition, "移動位置が正しく更新されるべき");
            Assert.IsFalse(testTilemapManager.OnPlayerHitTileCalled, "Treasureブロック移動時はOnPlayerHitTileが呼ばれないべき");
        }
    }
}
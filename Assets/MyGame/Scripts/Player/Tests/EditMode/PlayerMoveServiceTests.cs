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
        private MockTilemapManager _mockTilemapManager;

        [SetUp]
        public void SetUp()
        {
            _mockTilemapManager = new MockTilemapManager(true);
            _service = new PlayerMoveService(_mockTilemapManager);
        }

        [Test]
        [Description("上方向への移動時に座標のY値が1増加することを検証（TilemapManager設定済み）")]
        public void Move_WithUpDirection_ShouldUpdatePositionUpward()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            // TilemapManagerは既にコンストラクタで設定済み

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

            // TilemapManagerは既にコンストラクタで設定済み

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

            // TilemapManagerは既にコンストラクタで設定済み

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

            // TilemapManagerは既にコンストラクタで設定済み

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
        [Description("移動不可能な設定のTilemapManagerの場合、移動は失敗することを検証")]
        public void Move_WithImpassableTilemapManager_ShouldFailSafely()
        {
            // Arrange
            var impassableManager = new MockTilemapManager(false);
            var serviceWithImpassableManager = new PlayerMoveService(impassableManager);
            
            var initialPosition = new Vector2Int(0, 0);
            serviceWithImpassableManager.SetPosition(initialPosition);

            // Act
            var result = serviceWithImpassableManager.Move(Direction.Up);

            // Assert
            Assert.IsFalse(result, "移動不可能な設定の場合は移動が失敗すべき");
            Assert.AreEqual(initialPosition, serviceWithImpassableManager.CurrentPosition, "移動失敗時は位置が変更されないべき");
        }

        [Test]
        [Description("移動先がEmpty/Groundブロックの場合、移動が成功することを検証")]
        public void Move_ToPassableTile_ShouldSucceed()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            // TilemapManagerは既にコンストラクタで設定済み

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
            var impassableManager = new MockTilemapManager(false);
            var serviceWithImpassable = new PlayerMoveService(impassableManager);
            serviceWithImpassable.SetPosition(initialPosition);

            // Act
            var result = serviceWithImpassable.Move(Direction.Up);

            // Assert
            Assert.IsFalse(result, "移動不可能なタイルへの移動は失敗すべき");
            Assert.AreEqual(initialPosition, serviceWithImpassable.CurrentPosition, "位置は変更されないべき");
        }

        [Test]
        [Description("CanMoveメソッドが正しく動作することを検証")]
        public void CanMove_WithPassableTile_ShouldReturnTrue()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            // TilemapManagerは既にコンストラクタで設定済み

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
            var impassableManager = new MockTilemapManager(false);
            var serviceWithImpassable = new PlayerMoveService(impassableManager);
            serviceWithImpassable.SetPosition(initialPosition);

            // Act
            var canMove = serviceWithImpassable.CanMove(Direction.Right);

            // Assert
            Assert.IsFalse(canMove, "移動不可能なタイルに対してCanMoveはfalseを返すべき");
        }

        [Test]
        [Description("Groundブロックに移動した場合、OnPlayerHitTileが呼ばれることを検証")]
        public void Move_ToGroundBlock_ShouldCallOnPlayerHitTile()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            var groundManager = new MockTilemapManager(true, BlockType.Ground);
            var serviceWithGround = new PlayerMoveService(groundManager);
            serviceWithGround.SetPosition(initialPosition);

            // Act
            var result = serviceWithGround.Move(Direction.Right);

            // Assert
            Assert.IsTrue(result, "Groundブロックへの移動は成功すべき");
            Assert.AreEqual(new Vector2Int(1, 0), serviceWithGround.CurrentPosition, "移動位置が正しく更新されるべき");
            Assert.IsTrue(groundManager.OnPlayerHitTileCalled, "Groundブロック移動時にOnPlayerHitTileが呼ばれるべき");
            Assert.AreEqual(new Vector2Int(1, 0), groundManager.LastHitPosition, "破壊処理の座標が正しいべき");
            Assert.AreEqual(0, groundManager.LastHitLevel, "破壊処理のレベルが正しいべき");
        }

        [Test]
        [Description("CanPassThroughメソッドが新機能として正しく動作することを検証")]
        public void CanMove_UsesNewCanPassThroughMethod()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            var mockManager = new MockTilemapManager(true);
            mockManager.SetBlockType(new Vector2Int(1, 0), BlockType.Rock); // 右側にRockブロック設定
            
            var service = new PlayerMoveService(mockManager);
            service.SetPosition(initialPosition);

            // Act
            var canMoveRight = service.CanMove(Direction.Right); // Rockブロックなので移動不可
            var canMoveUp = service.CanMove(Direction.Up); // 制限なしなので移動可能

            // Assert
            Assert.IsFalse(canMoveRight, "Rockブロックへの移動はCanPassThroughで制限されるべき");
            Assert.IsTrue(canMoveUp, "制限のない方向への移動は可能であるべき");
        }

        [Test]
        [Description("新機能CanPassThroughによるSkyブロック制限の検証")]
        public void CanMove_BlocksSkyTileMovementWithNewMethod()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            var mockManager = new MockTilemapManager(true);
            mockManager.SetBlockType(new Vector2Int(0, 1), BlockType.Sky); // 上側にSkyブロック設定
            
            var service = new PlayerMoveService(mockManager);
            service.SetPosition(initialPosition);

            // Act
            var canMoveUp = service.CanMove(Direction.Up); // Skyブロックなので移動不可（新機能）

            // Assert
            Assert.IsFalse(canMoveUp, "Skyブロックへの移動は新しいCanPassThroughで制限されるべき");
        }
    }
}
using NUnit.Framework;
using UnityEngine;

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
        [Description("上方向への移動時に座標のY値が1増加することを検証")]
        public void Move_WithUpDirection_ShouldUpdatePositionUpward()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

            // Act
            _service.Move(Direction.Up);

            // Assert
            Assert.AreEqual(new Vector2Int(0, 1), _service.CurrentPosition);
        }

        [Test]
        [Description("下方向への移動時に座標のY値が1減少することを検証")]
        public void Move_WithDownDirection_ShouldUpdatePositionDownward()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 1);
            _service.SetPosition(initialPosition);

            // Act
            _service.Move(Direction.Down);

            // Assert
            Assert.AreEqual(new Vector2Int(0, 0), _service.CurrentPosition);
        }

        [Test]
        [Description("左方向への移動時に座標のX値が1減少することを検証")]
        public void Move_WithLeftDirection_ShouldUpdatePositionLeft()
        {
            // Arrange
            var initialPosition = new Vector2Int(1, 0);
            _service.SetPosition(initialPosition);

            // Act
            _service.Move(Direction.Left);

            // Assert
            Assert.AreEqual(new Vector2Int(0, 0), _service.CurrentPosition);
        }

        [Test]
        [Description("右方向への移動時に座標のX値が1増加することを検証")]
        public void Move_WithRightDirection_ShouldUpdatePositionRight()
        {
            // Arrange
            var initialPosition = new Vector2Int(0, 0);
            _service.SetPosition(initialPosition);

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
    }
}
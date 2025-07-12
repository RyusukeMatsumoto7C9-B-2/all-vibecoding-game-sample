using NUnit.Framework;
using UnityEngine;

namespace MyGame.Player.Tests
{
    [TestFixture]
    public class PlayerMoveServiceTests
    {
        private PlayerMoveService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new PlayerMoveService();
        }

        [Test]
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
        public void SetPosition_ShouldUpdateCurrentPosition()
        {
            // Arrange
            var newPosition = new Vector2Int(5, 3);

            // Act
            _service.SetPosition(newPosition);

            // Assert
            Assert.AreEqual(newPosition, _service.CurrentPosition);
        }
    }
}
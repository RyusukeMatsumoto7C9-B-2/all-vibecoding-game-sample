using NUnit.Framework;
using UnityEngine;
using MyGame.Enemy;
using MyGame.TilemapSystem.Core;
using MyGame.Common;

namespace MyGame.Enemy.Tests
{
    [TestFixture]
    public class EnemyMoveServiceTests
    {
        private EnemyMoveService _enemyMoveService;
        private MockTilemapManager _mockTilemapManager;

        [SetUp]
        public void SetUp()
        {
            _enemyMoveService = new EnemyMoveService();
            _mockTilemapManager = new MockTilemapManager();
            _enemyMoveService.SetTilemapManager(_mockTilemapManager, 0);
        }

        [Test]
        public void SetPosition_ValidPosition_SetsCurrentPosition()
        {
            var position = new Vector2Int(5, 5);
            
            _enemyMoveService.SetPosition(position);
            
            Assert.AreEqual(position, _enemyMoveService.CurrentPosition);
        }

        [Test]
        public void CanMove_ValidDirection_ReturnsTrue()
        {
            _enemyMoveService.SetPosition(new Vector2Int(5, 5));
            _mockTilemapManager.SetBlockType(new Vector2Int(5, 6), BlockType.Empty);
            
            var canMove = _enemyMoveService.CanMove(Direction.Up);
            
            Assert.IsTrue(canMove);
        }

        [Test]
        public void CanMove_RockBlock_ReturnsFalse()
        {
            _enemyMoveService.SetPosition(new Vector2Int(5, 5));
            _mockTilemapManager.SetBlockType(new Vector2Int(5, 6), BlockType.Rock);
            
            var canMove = _enemyMoveService.CanMove(Direction.Up);
            
            Assert.IsFalse(canMove);
        }

        [Test]
        public void CanMove_NoTilemapManager_ReturnsFalse()
        {
            var serviceWithoutManager = new EnemyMoveService();
            serviceWithoutManager.SetPosition(new Vector2Int(5, 5));
            
            var canMove = serviceWithoutManager.CanMove(Direction.Up);
            
            Assert.IsFalse(canMove);
        }

        [Test]
        public void Move_ValidDirection_UpdatesPosition()
        {
            _enemyMoveService.SetPosition(new Vector2Int(5, 5));
            _mockTilemapManager.SetBlockType(new Vector2Int(6, 5), BlockType.Empty);
            
            var moved = _enemyMoveService.Move(Direction.Right);
            
            Assert.IsTrue(moved);
            Assert.AreEqual(new Vector2Int(6, 5), _enemyMoveService.CurrentPosition);
        }

        [Test]
        public void Move_InvalidDirection_DoesNotUpdatePosition()
        {
            var originalPosition = new Vector2Int(5, 5);
            _enemyMoveService.SetPosition(originalPosition);
            _mockTilemapManager.SetBlockType(new Vector2Int(6, 5), BlockType.Rock);
            
            var moved = _enemyMoveService.Move(Direction.Right);
            
            Assert.IsFalse(moved);
            Assert.AreEqual(originalPosition, _enemyMoveService.CurrentPosition);
        }

        [TestCase(Direction.Up, 5, 6)]
        [TestCase(Direction.Down, 5, 4)]
        [TestCase(Direction.Left, 4, 5)]
        [TestCase(Direction.Right, 6, 5)]
        public void Move_AllDirections_MovesToCorrectPosition(Direction direction, int expectedX, int expectedY)
        {
            _enemyMoveService.SetPosition(new Vector2Int(5, 5));
            var expectedPosition = new Vector2Int(expectedX, expectedY);
            _mockTilemapManager.SetBlockType(expectedPosition, BlockType.Empty);
            
            var moved = _enemyMoveService.Move(direction);
            
            Assert.IsTrue(moved);
            Assert.AreEqual(expectedPosition, _enemyMoveService.CurrentPosition);
        }
    }
}
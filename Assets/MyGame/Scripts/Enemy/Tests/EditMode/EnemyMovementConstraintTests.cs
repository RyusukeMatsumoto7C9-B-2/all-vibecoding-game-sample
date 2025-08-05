using NUnit.Framework;
using UnityEngine;
using MyGame.Enemy;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem;
using MyGame.Common;

namespace MyGame.Enemy.Tests
{
    [TestFixture]
    public class EnemyMovementConstraintTests
    {
        private EnemyMovementConstraint _constraint;
        private MockTilemapManager _mockTilemapManager;

        [SetUp]
        public void SetUp()
        {
            _mockTilemapManager = new MockTilemapManager(true, BlockType.Empty);
            _constraint = new EnemyMovementConstraint(_mockTilemapManager, 0);
        }

        [Test]
        public void CanMoveToPosition_EmptyBlock_ReturnsTrue()
        {
            var position = new Vector2Int(5, 5);
            _mockTilemapManager.SetBlockType(position, BlockType.Empty);
            
            var canMove = _constraint.CanMoveToPosition(position);
            
            Assert.IsTrue(canMove);
        }

        [Test]
        public void CanMoveToPosition_RockBlock_ReturnsFalse()
        {
            var position = new Vector2Int(5, 5);
            _mockTilemapManager.SetBlockType(position, BlockType.Rock);
            
            var canMove = _constraint.CanMoveToPosition(position);
            
            Assert.IsFalse(canMove);
        }

        [Test]
        public void CanMoveToPosition_GroundBlock_ReturnsTrue()
        {
            var position = new Vector2Int(5, 5);
            _mockTilemapManager.SetBlockType(position, BlockType.Ground);
            
            var canMove = _constraint.CanMoveToPosition(position);
            
            Assert.IsTrue(canMove);
        }

        [Test]
        public void CanMoveInDirection_ValidDirection_ReturnsTrue()
        {
            var currentPosition = new Vector2Int(5, 5);
            var targetPosition = new Vector2Int(5, 6);
            _mockTilemapManager.SetBlockType(targetPosition, BlockType.Empty);
            
            var canMove = _constraint.CanMoveInDirection(currentPosition, Direction.Up);
            
            Assert.IsTrue(canMove);
        }

        [Test]
        public void CanMoveInDirection_InvalidDirection_ReturnsFalse()
        {
            var currentPosition = new Vector2Int(5, 5);
            var targetPosition = new Vector2Int(5, 6);
            _mockTilemapManager.SetBlockType(targetPosition, BlockType.Rock);
            
            var canMove = _constraint.CanMoveInDirection(currentPosition, Direction.Up);
            
            Assert.IsFalse(canMove);
        }

        [TestCase(Direction.Up, 5, 6)]
        [TestCase(Direction.Down, 5, 4)]
        [TestCase(Direction.Left, 4, 5)]
        [TestCase(Direction.Right, 6, 5)]
        public void GetTargetPosition_AllDirections_ReturnsCorrectPosition(Direction direction, int expectedX, int expectedY)
        {
            var currentPosition = new Vector2Int(5, 5);
            var expectedPosition = new Vector2Int(expectedX, expectedY);
            
            var targetPosition = _constraint.GetTargetPosition(currentPosition, direction);
            
            Assert.AreEqual(expectedPosition, targetPosition);
        }

        [Test]
        public void GetBlockTypeAt_ValidPosition_ReturnsCorrectBlockType()
        {
            var position = new Vector2Int(5, 5);
            _mockTilemapManager.SetBlockType(position, BlockType.Rock);
            
            var blockType = _constraint.GetBlockTypeAt(position);
            
            Assert.AreEqual(BlockType.Rock, blockType);
        }

        [Test]
        public void Constructor_NullTilemapManager_HandlesGracefully()
        {
            var constraintWithNull = new EnemyMovementConstraint(null, 0);
            
            var canMove = constraintWithNull.CanMoveToPosition(new Vector2Int(5, 5));
            
            Assert.IsFalse(canMove);
        }
    }
}
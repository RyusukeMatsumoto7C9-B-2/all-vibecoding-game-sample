using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.Common;

namespace MyGame.Enemy
{
    public class EnemyMovementConstraint
    {
        private readonly ITilemapService _tilemapManager;
        private readonly int _level;

        public EnemyMovementConstraint(ITilemapService tilemapManager, int level = 0)
        {
            _tilemapManager = tilemapManager;
            _level = level;
        }

        public bool CanMoveToPosition(Vector2Int position)
        {
            if (_tilemapManager == null)
            {
                return false;
            }

            return _tilemapManager.CanPlayerPassThrough(position, _level);
        }

        public bool CanMoveInDirection(Vector2Int currentPosition, Direction direction)
        {
            var targetPosition = GetTargetPosition(currentPosition, direction);
            return CanMoveToPosition(targetPosition);
        }

        public Vector2Int GetTargetPosition(Vector2Int currentPosition, Direction direction)
        {
            return direction switch
            {
                Direction.Up => new Vector2Int(currentPosition.x, currentPosition.y + 1),
                Direction.Down => new Vector2Int(currentPosition.x, currentPosition.y - 1),
                Direction.Left => new Vector2Int(currentPosition.x - 1, currentPosition.y),
                Direction.Right => new Vector2Int(currentPosition.x + 1, currentPosition.y),
                _ => currentPosition
            };
        }

        public BlockType GetBlockTypeAt(Vector2Int position)
        {
            return _tilemapManager?.GetBlockTypeAt(position, _level) ?? BlockType.Empty;
        }
    }
}
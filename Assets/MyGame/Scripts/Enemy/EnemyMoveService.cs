using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.Common;

namespace MyGame.Enemy
{
    public class EnemyMoveService
    {
        public Vector2Int CurrentPosition { get; private set; }
        
        private ITilemapManager _tilemapManager;
        private int _currentLevel = 0;

        public void SetPosition(Vector2Int position)
        {
            CurrentPosition = position;
        }

        public void SetTilemapManager(ITilemapManager tilemapManager, int level = 0)
        {
            _tilemapManager = tilemapManager;
            _currentLevel = level;
        }

        public bool CanMove(Direction direction)
        {
            if (_tilemapManager == null)
            {
                return false;
            }

            var targetPosition = GetTargetPosition(direction);
            return _tilemapManager.CanPlayerPassThrough(targetPosition, _currentLevel);
        }

        public bool Move(Direction direction)
        {
            if (!CanMove(direction))
            {
                return false;
            }

            var targetPosition = GetTargetPosition(direction);
            CurrentPosition = targetPosition;
            
            return true;
        }

        private Vector2Int GetTargetPosition(Direction direction)
        {
            return direction switch
            {
                Direction.Up => new Vector2Int(CurrentPosition.x, CurrentPosition.y + 1),
                Direction.Down => new Vector2Int(CurrentPosition.x, CurrentPosition.y - 1),
                Direction.Left => new Vector2Int(CurrentPosition.x - 1, CurrentPosition.y),
                Direction.Right => new Vector2Int(CurrentPosition.x + 1, CurrentPosition.y),
                _ => CurrentPosition
            };
        }
    }
}
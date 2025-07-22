using UnityEngine;
using MyGame.TilemapSystem.Core;

namespace MyGame.Player
{
    public class PlayerMoveService
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
                // TilemapManagerが設定されていない場合は安全のため移動不可とする
                Debug.LogWarning("[PlayerMoveService] TilemapManagerが未設定のため、移動を制限します");
                return false;
            }

            var targetPosition = GetTargetPosition(direction);
            var canPass = _tilemapManager.CanPlayerPassThrough(targetPosition, _currentLevel);
            
            Debug.Log($"[PlayerMoveService] 移動制約チェック: {direction}方向 → 座標({targetPosition.x}, {targetPosition.y}) " +
                     $"Level: {_currentLevel} → 結果: {(canPass ? "移動可能" : "移動不可")}");
            
            return canPass;
        }

        public bool Move(Direction direction)
        {
            if (!CanMove(direction))
            {
                return false; // 移動不可能
            }

            var targetPosition = GetTargetPosition(direction);
            CurrentPosition = targetPosition;
            return true; // 移動成功
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
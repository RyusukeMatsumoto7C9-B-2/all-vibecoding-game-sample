using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.Common;

namespace MyGame.Player
{
    public class PlayerMover
    {
        public Vector2Int CurrentPosition { get; private set; }
        
        private readonly ITilemapService _tilemapManager;
        private int _currentLevel = 0;

        public PlayerMover(ITilemapService tilemapManager, int currentLevel = 0)
        {
            _tilemapManager = tilemapManager ?? throw new System.ArgumentNullException(nameof(tilemapManager));
            _currentLevel = currentLevel;
        }

        public void SetPosition(Vector2Int position)
        {
            CurrentPosition = position;
        }

        public void SetCurrentLevel(int level)
        {
            _currentLevel = level;
        }

        public bool CanMove(Direction direction)
        {
            var targetPosition = GetTargetPosition(direction);
            var canPass = _tilemapManager.CanPassThrough(targetPosition, _currentLevel);
            
            Debug.Log($"[PlayerMoveService] 移動制約チェック: {direction}方向 → 座標({targetPosition.x}, {targetPosition.y}) " +
                     $"Level: {_currentLevel} → 結果: {(canPass ? "移動可能" : "移動不可")}");
            
            return canPass;
        }

        public bool Move(Direction direction)
        {
            if (!CanMove(direction))
            {
                return false;
            }

            var targetPosition = GetTargetPosition(direction);
            
            // 移動先のブロックタイプを取得
            var blockType = _tilemapManager.GetBlockTypeAt(targetPosition, _currentLevel);
            
            CurrentPosition = targetPosition;
            
            // Groundブロックの場合、破壊処理を実行
            if (blockType == BlockType.Ground)
            {
                Debug.Log($"[PlayerMoveService] Groundブロック破壊: 座標({targetPosition.x}, {targetPosition.y})");
                _tilemapManager.OnPlayerHitTile(targetPosition, _currentLevel);
            }
            
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
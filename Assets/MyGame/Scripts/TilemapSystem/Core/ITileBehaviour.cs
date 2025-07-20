using UnityEngine;

namespace MyGame.TilemapSystem.Core
{
    public interface ITileBehavior
    {
        bool CanPlayerPassThrough(BlockType blockType);
        BlockType OnPlayerHit(BlockType blockType, Vector2Int position, out int scoreGained);
        void OnTimeUpdate(Vector2Int position, BlockType[,] tiles, float deltaTime);
    }
}
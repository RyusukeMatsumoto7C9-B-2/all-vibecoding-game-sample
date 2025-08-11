using UnityEngine;

namespace MyGame.TilemapSystem.Core
{
    /// <summary>
    /// タイルの振る舞いを定義するインターフェース
    /// </summary>
    public interface ITileBehavior
    {
        /// <summary>
        /// プレイヤーが指定されたブロックタイプを通過できるかを判定します
        /// </summary>
        /// <param name="blockType">判定するブロックタイプ</param>
        /// <returns>通過可能な場合はtrue</returns>
        bool CanPlayerPassThrough(BlockType blockType);
        
        /// <summary>
        /// プレイヤーがブロックに衝突した時の処理を実行します
        /// </summary>
        /// <param name="blockType">衝突したブロックタイプ</param>
        /// <param name="position">衝突位置</param>
        /// <param name="scoreGained">獲得スコア</param>
        /// <returns>衝突後のブロックタイプ</returns>
        BlockType OnPlayerHit(BlockType blockType, Vector2Int position, out int scoreGained);
        
        /// <summary>
        /// 時間経過による更新処理を実行します
        /// </summary>
        /// <param name="position">対象位置</param>
        /// <param name="tiles">全タイル配列</param>
        /// <param name="deltaTime">経過時間</param>
        void OnTimeUpdate(Vector2Int position, BlockType[,] tiles, float deltaTime);
    }
}
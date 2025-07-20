using System;
using UnityEngine;

namespace MyGame.TilemapSystem.Core
{
    public interface ITileBehavior
    {
        bool CanPlayerPassThrough(BlockType blockType);
        BlockType OnPlayerHit(BlockType blockType, Vector2Int position, out int scoreGained);
        void OnTimeUpdate(Vector2Int position, BlockType[,] tiles, float deltaTime);
    }

    public class TileBehavior : ITileBehavior
    {
        public bool CanPlayerPassThrough(BlockType blockType)
        {
            return blockType switch
            {
                BlockType.Sky => true,
                BlockType.Empty => true,
                BlockType.Ground => true,
                BlockType.Rock => false,     // Playerは通過できない
                BlockType.Treasure => true,
                _ => true
            };
        }

        public BlockType OnPlayerHit(BlockType blockType, Vector2Int position, out int scoreGained)
        {
            scoreGained = 0;
            
            switch (blockType)
            {
                case BlockType.Ground:
                    // Playerがヒットした場合、タイルはEmptyタイルに変化する
                    scoreGained = 0;
                    return BlockType.Empty;
                    
                case BlockType.Treasure:
                    // Playerとヒットしたらスコアが加算される
                    scoreGained = 100;
                    return BlockType.Empty;
                    
                default:
                    return blockType;
            }
        }

        public void OnTimeUpdate(Vector2Int position, BlockType[,] tiles, float deltaTime)
        {
            // Rock属性のブロックの落下処理を実装
            if (tiles[position.x, position.y] == BlockType.Rock)
            {
                ProcessRockFalling(position, tiles);
            }
        }

        private void ProcessRockFalling(Vector2Int position, BlockType[,] tiles)
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            
            // 一つ下のブロックを確認
            if (position.y - 1 >= 0)
            {
                var belowTile = tiles[position.x, position.y - 1];
                
                // 一つ下のブロックがGroundブロックでかつその下がEmptyブロックの場合
                if (belowTile == BlockType.Ground && position.y - 2 >= 0 && tiles[position.x, position.y - 2] == BlockType.Empty)
                {
                    // 2秒後に一つ下のEmptyブロックをEmptyブロックに変更（既にEmpty）
                    // 次のGroundブロックに到達するまで落下する
                    StartRockFalling(position, tiles);
                }
                // 一つ下のブロックがEmptyになったら即座に落下開始
                else if (belowTile == BlockType.Empty)
                {
                    StartRockFalling(position, tiles);
                }
            }
        }

        private void StartRockFalling(Vector2Int position, BlockType[,] tiles)
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            
            // 直下のマスがGroundまたはRockになるまで落下し続ける
            int fallTargetY = position.y - 1;
            
            while (fallTargetY >= 0)
            {
                var targetTile = tiles[position.x, fallTargetY];
                if (targetTile == BlockType.Ground || targetTile == BlockType.Rock)
                {
                    break;
                }
                fallTargetY--;
            }
            
            // 落下先が見つかった場合、Rockを移動
            if (fallTargetY >= 0 && fallTargetY < position.y - 1)
            {
                // 元の位置をEmptyに
                tiles[position.x, position.y] = BlockType.Empty;
                // 落下先の一つ上にRockを配置
                tiles[position.x, fallTargetY + 1] = BlockType.Rock;
            }
        }
    }
}
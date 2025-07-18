using System;
using UnityEngine;

namespace MyGame.TilemapSystem.Core
{
    public interface ITileBehavior
    {
        bool CanPlayerPassThrough(TileType tileType);
        TileType OnPlayerHit(TileType tileType, Vector2Int position, out int scoreGained);
        void OnTimeUpdate(Vector2Int position, TileType[,] tiles, float deltaTime);
    }

    public class TileBehavior : ITileBehavior
    {
        public bool CanPlayerPassThrough(TileType tileType)
        {
            return tileType switch
            {
                TileType.Sky => true,
                TileType.Empty => true,
                TileType.Ground => true,
                TileType.Rock => false,     // Playerは通過できない
                TileType.Treasure => true,
                _ => true
            };
        }

        public TileType OnPlayerHit(TileType tileType, Vector2Int position, out int scoreGained)
        {
            scoreGained = 0;
            
            switch (tileType)
            {
                case TileType.Ground:
                    // Playerがヒットした場合、タイルはEmptyタイルに変化する
                    scoreGained = 0;
                    return TileType.Empty;
                    
                case TileType.Treasure:
                    // Playerとヒットしたらスコアが加算される
                    scoreGained = 100;
                    return TileType.Empty;
                    
                default:
                    return tileType;
            }
        }

        public void OnTimeUpdate(Vector2Int position, TileType[,] tiles, float deltaTime)
        {
            // Rock属性のブロックの落下処理を実装
            if (tiles[position.x, position.y] == TileType.Rock)
            {
                ProcessRockFalling(position, tiles);
            }
        }

        private void ProcessRockFalling(Vector2Int position, TileType[,] tiles)
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            
            // 一つ下のブロックを確認
            if (position.y - 1 >= 0)
            {
                var belowTile = tiles[position.x, position.y - 1];
                
                // 一つ下のブロックがGroundブロックでかつその下がEmptyブロックの場合
                if (belowTile == TileType.Ground && position.y - 2 >= 0 && tiles[position.x, position.y - 2] == TileType.Empty)
                {
                    // 2秒後に一つ下のEmptyブロックをEmptyブロックに変更（既にEmpty）
                    // 次のGroundブロックに到達するまで落下する
                    StartRockFalling(position, tiles);
                }
                // 一つ下のブロックがEmptyになったら即座に落下開始
                else if (belowTile == TileType.Empty)
                {
                    StartRockFalling(position, tiles);
                }
            }
        }

        private void StartRockFalling(Vector2Int position, TileType[,] tiles)
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            
            // 直下のマスがGroundまたはRockになるまで落下し続ける
            int fallTargetY = position.y - 1;
            
            while (fallTargetY >= 0)
            {
                var targetTile = tiles[position.x, fallTargetY];
                if (targetTile == TileType.Ground || targetTile == TileType.Rock)
                {
                    break;
                }
                fallTargetY--;
            }
            
            // 落下先が見つかった場合、Rockを移動
            if (fallTargetY >= 0 && fallTargetY < position.y - 1)
            {
                // 元の位置をEmptyに
                tiles[position.x, position.y] = TileType.Empty;
                // 落下先の一つ上にRockを配置
                tiles[position.x, fallTargetY + 1] = TileType.Rock;
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.TilemapSystem.Core
{
    public interface ITilemapManager
    {
        bool CanPlayerPassThrough(Vector2Int position, int level);
        void OnPlayerHitTile(Vector2Int position, int level);
        bool IsMapLoaded(int level);
        MapData GetLoadedMap(int level);
        BlockType GetBlockTypeAt(Vector2Int position, int level);
        
        /// <summary>
        /// グリッド座標をワールド座標に変換します
        /// </summary>
        /// <param name="x">グリッドX座標</param>
        /// <param name="y">グリッドY座標</param>
        /// <returns>ワールド座標</returns>
        Vector3 GetPosition(int x, int y);
        
        /// <summary>
        /// 指定位置が通過可能かを判定します（Player/Enemy共通）
        /// </summary>
        /// <param name="position">グリッド座標</param>
        /// <param name="level">レベル</param>
        /// <returns>通過可能な場合はtrue</returns>
        bool CanPassThrough(Vector2Int position, int level);
        
        /// <summary>
        /// メモリ最適化を実行します
        /// </summary>
        /// <param name="currentLevel">現在のレベル</param>
        void OptimizeMemory(int currentLevel);
        
        /// <summary>
        /// 重複保護機能付きでタイルを配置します
        /// </summary>
        /// <param name="mapData">マップデータ</param>
        /// <param name="overlapHeight">重複エリアの高さ</param>
        void PlaceTilesWithOverlapProtection(MapData mapData, int overlapHeight = 5);
        
        /// <summary>
        /// 指定レベルのタイルオブジェクトリストを取得します
        /// </summary>
        /// <param name="level">レベル</param>
        /// <returns>タイルオブジェクトのリスト</returns>
        List<GameObject> GetTilesForLevel(int level);
    }
}
using UnityEngine;
using MyGame.TilemapSystem.Core;
using System.Collections.Generic;

namespace MyGame.TilemapSystem
{
    public class MockTilemapService : ITilemapService
    {
        private readonly bool _allowMovement;
        private readonly BlockType _blockType;
        private readonly Dictionary<Vector2Int, BlockType> _blockTypes = new Dictionary<Vector2Int, BlockType>();
        
        // テスト用マップ境界情報
        private int _mapWidth = -1;
        private int _mapHeight = -1;
        
        // テスト用スパイ機能
        public bool OnPlayerHitTileCalled { get; private set; }
        public Vector2Int LastHitPosition { get; private set; }
        public int LastHitLevel { get; private set; }

        public MockTilemapService(bool allowMovement, BlockType blockType = BlockType.Empty)
        {
            _allowMovement = allowMovement;
            _blockType = blockType;
        }

        public bool CanPlayerPassThrough(Vector2Int position, int level)
        {
            if (_blockTypes.ContainsKey(position))
            {
                var blockType = _blockTypes[position];
                return blockType != BlockType.Rock;
            }
            return _allowMovement;
        }

        public void OnPlayerHitTile(Vector2Int position, int level)
        {
            // テスト用スパイ機能
            OnPlayerHitTileCalled = true;
            LastHitPosition = position;
            LastHitLevel = level;
        }

        public bool IsMapLoaded(int level)
        {
            // テスト用では常にマップがロードされているものとする
            return true;
        }

        public MapData GetLoadedMap(int level)
        {
            // テスト用のダミーMapDataを返す
            var tiles = new BlockType[1, 1];
            tiles[0, 0] = _blockType;
            return new MapData(1, 1, tiles, 0, level);
        }

        public BlockType GetBlockTypeAt(Vector2Int position, int level)
        {
            if (_blockTypes.ContainsKey(position))
            {
                return _blockTypes[position];
            }
            return _blockType;
        }

        // テスト用メソッド
        public void SetBlockType(Vector2Int position, BlockType blockType)
        {
            _blockTypes[position] = blockType;
        }
        
        // テスト用マップサイズ設定メソッド
        public void SetMapSize(int width, int height)
        {
            _mapWidth = width;
            _mapHeight = height;
        }


        /// <summary>
        /// グリッド座標をワールド座標に変換します
        /// </summary>
        /// <param name="x">グリッドX座標</param>
        /// <param name="y">グリッドY座標</param>
        /// <returns>ワールド座標</returns>
        public Vector3 GetPosition(int x, int y)
        {
            return new Vector3(x, y, 0);
        }


        /// <summary>
        /// 指定位置が通過可能かを判定します（Player/Enemy共通）
        /// </summary>
        /// <param name="position">グリッド座標</param>
        /// <param name="level">レベル</param>
        /// <returns>通過可能な場合はtrue</returns>
        public bool CanPassThrough(Vector2Int position, int level)
        {
            // マップサイズが設定されている場合は境界チェックを実行
            if (_mapWidth > 0 && _mapHeight > 0)
            {
                if (position.x < 0 || position.x >= _mapWidth || position.y < 0 || position.y >= _mapHeight)
                {
                    return false; // 境界外は移動不可
                }
            }
            
            if (_blockTypes.ContainsKey(position))
            {
                var blockType = _blockTypes[position];
                // 新仕様: Sky=不可, Empty=可, Ground=可, Rock=不可, Treasure=可
                return blockType == BlockType.Empty || blockType == BlockType.Ground || blockType == BlockType.Treasure;
            }
            return _allowMovement;
        }
        
        /// <summary>
        /// タイルを配置します
        /// </summary>
        /// <param name="mapData">配置するマップデータ</param>
        public void PlaceTiles(MapData mapData)
        {
            // Mock実装では何もしない
        }
        
        /// <summary>
        /// 重複保護機能付きでタイルを配置します
        /// </summary>
        /// <param name="mapData">配置するマップデータ</param>
        /// <param name="overlapHeight">重複エリアの高さ</param>
        public void PlaceTilesWithOverlapProtection(MapData mapData, int overlapHeight = 5)
        {
            // Mock実装では何もしない
        }
        
        /// <summary>
        /// メモリ最適化を実行します
        /// </summary>
        /// <param name="currentLevel">現在のレベル</param>
        public void OptimizeMemory(int currentLevel)
        {
            // Mock実装では何もしない
        }
        
        /// <summary>
        /// 指定レベルのタイル一覧を取得します
        /// </summary>
        /// <param name="level">レベル</param>
        /// <returns>タイルのGameObjectリスト</returns>
        public List<GameObject> GetTilesForLevel(int level)
        {
            // Mock実装では空のリストを返す
            return new List<GameObject>();
        }
        
    }
}
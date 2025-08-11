using UnityEngine;
using MyGame.TilemapSystem.Core;
using System.Collections.Generic;

namespace MyGame.TilemapSystem
{
    public class MockTilemapManager : ITilemapManager
    {
        private readonly bool _allowMovement;
        private BlockType _blockType;
        private readonly Dictionary<Vector2Int, BlockType> _blockTypes = new Dictionary<Vector2Int, BlockType>();
        
        // テスト用スパイ機能
        public bool OnPlayerHitTileCalled { get; private set; }
        public Vector2Int LastHitPosition { get; private set; }
        public int LastHitLevel { get; private set; }

        public MockTilemapManager(bool allowMovement, BlockType blockType = BlockType.Empty)
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
            if (_blockTypes.ContainsKey(position))
            {
                var blockType = _blockTypes[position];
                return blockType != BlockType.Rock && blockType != BlockType.Ground;
            }
            return _allowMovement;
        }
    }
}
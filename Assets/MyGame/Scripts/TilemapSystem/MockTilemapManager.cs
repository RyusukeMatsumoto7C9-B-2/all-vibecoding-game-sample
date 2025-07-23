using UnityEngine;
using MyGame.TilemapSystem.Core;

namespace MyGame.TilemapSystem
{
    public class MockTilemapManager : ITilemapManager
    {
        private readonly bool _allowMovement;
        private readonly BlockType _blockType;
        
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
            return _blockType;
        }
    }
}
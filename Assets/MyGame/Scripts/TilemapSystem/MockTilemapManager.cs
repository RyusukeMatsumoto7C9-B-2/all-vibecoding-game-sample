using UnityEngine;
using MyGame.TilemapSystem.Core;

namespace MyGame.TilemapSystem
{
    public class MockTilemapManager : ITilemapManager
    {
        private readonly bool _allowMovement;

        public MockTilemapManager(bool allowMovement)
        {
            _allowMovement = allowMovement;
        }

        public bool CanPlayerPassThrough(Vector2Int position, int level)
        {
            return _allowMovement;
        }

        public void OnPlayerHitTile(Vector2Int position, int level)
        {
            // テスト用のため何もしない
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
            tiles[0, 0] = BlockType.Empty;
            return new MapData(1, 1, tiles, 0, level);
        }
    }
}
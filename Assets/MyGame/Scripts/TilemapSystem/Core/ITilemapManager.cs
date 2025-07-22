using UnityEngine;

namespace MyGame.TilemapSystem.Core
{
    public interface ITilemapManager
    {
        bool CanPlayerPassThrough(Vector2Int position, int level);
        void OnPlayerHitTile(Vector2Int position, int level);
        bool IsMapLoaded(int level);
        MapData GetLoadedMap(int level);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyGame.TilemapSystem.Core
{
    public class TilemapManager
    {
        private readonly Tilemap _tilemap;
        private readonly Dictionary<TileType, TileBase> _tileAssets;
        private readonly Dictionary<int, MapData> _loadedMaps;

        public event Action<MapData> OnMapGenerated;
        public event Action<int> OnMemoryOptimized;

        public TilemapManager(Tilemap tilemap, Dictionary<TileType, TileBase> tileAssets)
        {
            _tilemap = tilemap ?? throw new ArgumentNullException(nameof(tilemap));
            _tileAssets = tileAssets ?? throw new ArgumentNullException(nameof(tileAssets));
            _loadedMaps = new Dictionary<int, MapData>();
        }

        public void PlaceTiles(MapData mapData)
        {
            if (mapData.Tiles == null)
            {
                Debug.LogError("MapData.Tiles is null");
                return;
            }

            var positions = new List<Vector3Int>();
            var tilesToPlace = new List<TileBase>();

            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    var tileType = mapData.Tiles[x, y];
                    
                    if (tileType != TileType.Empty && _tileAssets.ContainsKey(tileType))
                    {
                        var position = new Vector3Int(x, y, 0);
                        positions.Add(position);
                        tilesToPlace.Add(_tileAssets[tileType]);
                    }
                }
            }

            if (positions.Count > 0)
            {
                _tilemap.SetTiles(positions.ToArray(), tilesToPlace.ToArray());
            }

            _loadedMaps[mapData.Level] = mapData;
            OnMapGenerated?.Invoke(mapData);
        }

        public void ClearTiles(int level)
        {
            if (!_loadedMaps.ContainsKey(level))
            {
                return;
            }

            var mapData = _loadedMaps[level];
            var positions = new List<Vector3Int>();

            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    var position = new Vector3Int(x, y, 0);
                    positions.Add(position);
                }
            }

            if (positions.Count > 0)
            {
                _tilemap.SetTiles(positions.ToArray(), new TileBase[positions.Count]);
            }

            _loadedMaps.Remove(level);
        }

        public void OptimizeMemory(int currentLevel)
        {
            var levelsToRemove = new List<int>();
            
            foreach (var level in _loadedMaps.Keys)
            {
                // 現在レベル±2以外のマップを削除対象とする
                if (Math.Abs(level - currentLevel) > 2)
                {
                    levelsToRemove.Add(level);
                }
            }

            foreach (var level in levelsToRemove)
            {
                ClearTiles(level);
            }

            OnMemoryOptimized?.Invoke(currentLevel);
        }

        public bool IsMapLoaded(int level)
        {
            return _loadedMaps.ContainsKey(level);
        }

        public MapData GetLoadedMap(int level)
        {
            return _loadedMaps.ContainsKey(level) ? _loadedMaps[level] : default;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.TilemapSystem.Core
{
    public class TilemapManager
    {
        private readonly Transform _parentTransform;
        private readonly Dictionary<TileType, GameObject> _tilePrefabs;
        private readonly Dictionary<int, MapData> _loadedMaps;
        private readonly Dictionary<int, List<GameObject>> _instantiatedTiles;

        public event Action<MapData> OnMapGenerated;
        public event Action<int> OnMemoryOptimized;

        public TilemapManager(Transform parentTransform, Dictionary<TileType, GameObject> tilePrefabs)
        {
            _parentTransform = parentTransform ?? throw new ArgumentNullException(nameof(parentTransform));
            _tilePrefabs = tilePrefabs ?? throw new ArgumentNullException(nameof(tilePrefabs));
            _loadedMaps = new Dictionary<int, MapData>();
            _instantiatedTiles = new Dictionary<int, List<GameObject>>();
        }

        public void PlaceTiles(MapData mapData)
        {
            if (mapData.Tiles == null)
            {
                Debug.LogError("MapData.Tiles is null");
                return;
            }

            // 既存のタイルを削除
            if (_instantiatedTiles.ContainsKey(mapData.Level))
            {
                ClearTiles(mapData.Level);
            }

            var tileInstances = new List<GameObject>();

            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    var tileType = mapData.Tiles[x, y];
                    
                    if (tileType != TileType.Empty && _tilePrefabs.ContainsKey(tileType))
                    {
                        // 2DSprite座標系: スプライト中心が原点、隙間なく並べる
                        var position = new Vector3(x, y, 0);
                        var tileInstance = UnityEngine.Object.Instantiate(_tilePrefabs[tileType], position, Quaternion.identity, _parentTransform);
                        
                        // タイルにレベル情報を設定（デバッグ用）
                        tileInstance.name = $"{tileType}_Level{mapData.Level}_{x}_{y}";
                        
                        tileInstances.Add(tileInstance);
                    }
                }
            }

            _instantiatedTiles[mapData.Level] = tileInstances;
            _loadedMaps[mapData.Level] = mapData;
            OnMapGenerated?.Invoke(mapData);
        }

        public void ClearTiles(int level)
        {
            if (!_instantiatedTiles.ContainsKey(level))
            {
                return;
            }

            var tileInstances = _instantiatedTiles[level];
            
            foreach (var tileInstance in tileInstances)
            {
                if (tileInstance != null)
                {
                    UnityEngine.Object.DestroyImmediate(tileInstance);
                }
            }

            _instantiatedTiles.Remove(level);
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

        public List<GameObject> GetTilesForLevel(int level)
        {
            return _instantiatedTiles.ContainsKey(level) ? _instantiatedTiles[level] : null;
        }
    }
}
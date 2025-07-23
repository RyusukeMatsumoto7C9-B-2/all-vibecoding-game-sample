using System;
using System.Collections.Generic;
using UnityEngine;
using R3;

namespace MyGame.TilemapSystem.Core
{
    public class TilemapManager : ITilemapManager
    {
        private readonly Transform _parentTransform;
        private readonly Dictionary<BlockType, GameObject> _tilePrefabs;
        private readonly Dictionary<int, MapData> _loadedMaps;
        private readonly Dictionary<int, List<GameObject>> _instantiatedTiles;
        private readonly ITileBehavior _tileBehavior;
        
        public Observable<MapData> OnMapGenerated => _onMapGenerated;
        public Observable<int> OnMemoryOptimized => _onMemoryOptimized;
        public Observable<(Vector2Int, BlockType, int)> OnTileHit => _onTileHit; // position, oldType, scoreGained
        
        private readonly Subject<MapData> _onMapGenerated = new Subject<MapData>();
        private readonly Subject<int> _onMemoryOptimized = new Subject<int>();
        private readonly Subject<(Vector2Int, BlockType, int)> _onTileHit = new Subject<(Vector2Int, BlockType, int)>();
        
        
        public TilemapManager(Transform parentTransform, Dictionary<BlockType, GameObject> tilePrefabs, ITileBehavior tileBehavior = null)
        {
            _parentTransform = parentTransform ?? throw new ArgumentNullException(nameof(parentTransform));
            _tilePrefabs = tilePrefabs ?? throw new ArgumentNullException(nameof(tilePrefabs));
            _tileBehavior = tileBehavior ?? new TileBehavior();
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
                    
                    if (tileType != BlockType.Empty && _tilePrefabs.ContainsKey(tileType))
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
            _onMapGenerated?.OnNext(mapData);
        }

        public void PlaceTilesWithOverlapProtection(MapData mapData, int overlapHeight = 5)
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
                    
                    if (tileType != BlockType.Empty && _tilePrefabs.ContainsKey(tileType))
                    {
                        // 重複エリア（下5マス）での既存ブロック保護チェック
                        var position = new Vector3(x, y, 0);
                        
                        // 重複エリアで既存のWallブロックとの衝突チェック
                        if (y < overlapHeight && IsExistingWallBlockAtPosition(position))
                        {
                            Debug.Log($"重複エリアで既存Wallブロックを保護: Level{mapData.Level} 位置({x}, {y})");
                            continue; // 既存Wallブロックを保護するため、新しいタイルを生成しない
                        }
                        
                        var tileInstance = UnityEngine.Object.Instantiate(_tilePrefabs[tileType], position, Quaternion.identity, _parentTransform);
                        
                        // タイルにレベル情報を設定（デバッグ用）
                        tileInstance.name = $"{tileType}_Level{mapData.Level}_{x}_{y}";
                        
                        tileInstances.Add(tileInstance);
                    }
                }
            }

            _instantiatedTiles[mapData.Level] = tileInstances;
            _loadedMaps[mapData.Level] = mapData;
            _onMapGenerated?.OnNext(mapData);
        }

        private bool IsExistingWallBlockAtPosition(Vector3 position)
        {
            // 全てのレベルの既存タイルをチェック
            foreach (var levelTiles in _instantiatedTiles.Values)
            {
                foreach (var tile in levelTiles)
                {
                    if (tile != null)
                    {
                        // 座標の完全一致チェック（浮動小数点誤差を考慮）
                        var tilePos = tile.transform.position;
                        bool isExactMatch = Mathf.Approximately(tilePos.x, position.x) && 
                                          Mathf.Approximately(tilePos.y, position.y);
                        
                        if (isExactMatch)
                        {
                            // タイル名からタイプを判定（Wallブロック系の保護）
                            var tileName = tile.name.ToLower();
                            if (tileName.Contains("rock") || tileName.Contains("wall") || tileName.Contains("ground"))
                            {
                                Debug.Log($"座標重複を検出して保護: {tileName} at ({position.x}, {position.y})");
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
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

            _onMemoryOptimized?.OnNext(currentLevel);
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

        public virtual bool CanPlayerPassThrough(Vector2Int position, int level)
        {
            if (!_loadedMaps.ContainsKey(level))
            {
                Debug.Log($"[TilemapManager] Level {level}のマップが読み込まれていません → 移動可能");
                return true;
            }

            var mapData = _loadedMaps[level];
            if (position.x < 0 || position.x >= mapData.Width || position.y < 0 || position.y >= mapData.Height)
            {
                Debug.Log($"[TilemapManager] 座標({position.x}, {position.y})はマップ範囲外 " +
                         $"(0-{mapData.Width-1}, 0-{mapData.Height-1}) → 移動不可");
                return false;
            }

            var tileType = mapData.Tiles[position.x, position.y];
            var canPass = _tileBehavior.CanPlayerPassThrough(tileType);
            
            Debug.Log($"[TilemapManager] 座標({position.x}, {position.y}) Level{level}: {tileType}ブロック → " +
                     $"{(canPass ? "移動可能" : "移動不可")}");
            
            return canPass;
        }

        public void OnPlayerHitTile(Vector2Int position, int level)
        {
            if (!_loadedMaps.ContainsKey(level))
                return;

            var mapData = _loadedMaps[level];
            if (position.x < 0 || position.x >= mapData.Width || position.y < 0 || position.y >= mapData.Height)
                return;

            var oldTileType = mapData.Tiles[position.x, position.y];
            var newTileType = _tileBehavior.OnPlayerHit(oldTileType, position, out int scoreGained);

            if (oldTileType != newTileType)
            {
                // MapDataは読み取り専用なので、新しいMapDataを作成
                var newTiles = (BlockType[,])mapData.Tiles.Clone();
                newTiles[position.x, position.y] = newTileType;
                
                var newMapData = new MapData(mapData.Width, mapData.Height, newTiles, mapData.Seed, mapData.Level);
                _loadedMaps[level] = newMapData;

                // タイルの表示を更新
                UpdateTileDisplay(position, newTileType, level);
                _onTileHit?.OnNext((position, oldTileType, scoreGained));
            }
        }

        private void UpdateTileDisplay(Vector2Int position, BlockType newBlockType, int level)
        {
            if (!_instantiatedTiles.ContainsKey(level))
                return;

            var tileInstances = _instantiatedTiles[level];
            
            // 既存のタイルを削除
            for (int i = tileInstances.Count - 1; i >= 0; i--)
            {
                var tileInstance = tileInstances[i];
                if (tileInstance != null && tileInstance.name.Contains($"_{position.x}_{position.y}"))
                {
                    UnityEngine.Object.DestroyImmediate(tileInstance);
                    tileInstances.RemoveAt(i);
                    break;
                }
            }

            // 新しいタイルを生成（Emptyの場合は生成しない）
            if (newBlockType != BlockType.Empty && _tilePrefabs.ContainsKey(newBlockType))
            {
                var worldPosition = new Vector3(position.x, position.y, 0);
                var tileInstance = UnityEngine.Object.Instantiate(_tilePrefabs[newBlockType], worldPosition, Quaternion.identity, _parentTransform);
                tileInstance.name = $"{newBlockType}_Level{level}_{position.x}_{position.y}";
                tileInstances.Add(tileInstance);
            }
        }

        public void UpdateTilesWithTime(int level, float deltaTime)
        {
            if (!_loadedMaps.ContainsKey(level))
                return;

            var mapData = _loadedMaps[level];
            
            // Rock属性ブロックの時間更新処理
            for (int x = 0; x < mapData.Width; x++)
            {
                for (int y = 0; y < mapData.Height; y++)
                {
                    var position = new Vector2Int(x, y);
                    _tileBehavior.OnTimeUpdate(position, mapData.Tiles, deltaTime);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem.Generation;
using Cysharp.Threading.Tasks;
using R3;

namespace MyGame.TilemapSystem
{
    public class TilemapSystemController : MonoBehaviour, ITilemapService
    {
        [Header("Universal Tile Prefab")]
        [SerializeField] private GameObject universalTilePrefab;
        
        [Header("System Settings")]
        [SerializeField] private int initialLevel = 1;
        [SerializeField] private int initialSeed = 12345;
        
        [Header("Auto Scroll Settings")]
        [SerializeField] private bool autoScrollEnabled = true;
        [SerializeField] private float autoScrollInterval = 3.0f;

        private TilemapGenerator _generator;
        private SeedManager _seedManager;
        private TilemapScrollController _scrollController;
        
        //private readonly Dictionary<int, MapData> _loadedMaps = new Dictionary<int, MapData>();
        private readonly Dictionary<int, List<GameObject>> _instantiatedTiles = new Dictionary<int, List<GameObject>>();
        private ITileBehavior _tileBehavior;
        
        public TilemapScrollController ScrollController => _scrollController;
        public int CurrentLevel => _scrollController?.CurrentLevel ?? initialLevel;
        
        
        private void Start()
        {
            InitializeSystem();
            GenerateInitialMap();
            
            // 自動スクロールを開始
            if (autoScrollEnabled)
            {
                StartAutoScroll().Forget();
            }
        }

        private void InitializeSystem()
        {
            if (universalTilePrefab == null)
            {
                Debug.LogError("[TilemapSystemController] UniversalTilePrefabが設定されていません");
                return;
            }

            _seedManager = new SeedManager(initialSeed);
            _generator = new TilemapGenerator(_seedManager);

            _tileBehavior = new TileBehavior();
            
            // スクロールコントローラーを初期化（thisを渡す）
            _scrollController = new TilemapScrollController(_generator, this, transform);
            _scrollController.OnScrollStarted?.Subscribe(OnScrollStarted).AddTo(this);
            _scrollController.OnScrollCompleted?.Subscribe(OnScrollCompleted).AddTo(this);
            _scrollController.OnNewLevelGenerated?.Subscribe(OnNewLevelGenerated).AddTo(this);
        }

        private void GenerateInitialMap()
        {
            if (_generator == null)
            {
                Debug.LogError("TilemapGenerator is not initialized");
                return;
            }

            var mapData = _generator.GenerateMap(initialLevel, initialSeed);
            _scrollController.SetMap(initialLevel, mapData);
            
            PlaceTiles(mapData);

            Debug.Log($"初期マップ生成完了: Level={mapData.Level}, Seed={mapData.Seed}, Size={mapData.Width}x{mapData.Height}");
        }
        
        private async UniTask StartAutoScroll()
        {
            await UniTask.Delay((int)(autoScrollInterval * 1000)); // 最初の待機
            
            while (autoScrollEnabled && _scrollController != null)
            {
                Debug.Log($"自動スクロール開始: レベル {_scrollController.CurrentLevel}");
                await _scrollController.StartScrollAsync();
                
                // 次のスクロールまで待機
                await UniTask.Delay((int)(autoScrollInterval * 1000));
            }
        }
        
        private void OnScrollStarted(int currentLevel)
        {
            Debug.Log($"スクロール開始: レベル {currentLevel}");
        }
        
        private void OnScrollCompleted(int newLevel)
        {
            Debug.Log($"スクロール完了: 新しいレベル {newLevel}");
        }
        
        private void OnNewLevelGenerated(int level)
        {
            Debug.Log($"新しいレベル生成: レベル {level}");
        }
        
        [ContextMenu("手動スクロール実行")]
        public void StartManualScroll()
        {
            if (_scrollController != null)
            {
                _scrollController.StartScrollAsync().Forget();
            }
        }
        
        [ContextMenu("自動スクロール停止")]
        public void StopAutoScroll()
        {
            autoScrollEnabled = false;
            Debug.Log("自動スクロールを停止しました");
        }
        
        [ContextMenu("自動スクロール開始")]
        public void StartAutoScrollManual()
        {
            if (!autoScrollEnabled)
            {
                autoScrollEnabled = true;
                StartAutoScroll().Forget();
                Debug.Log("自動スクロールを開始しました");
            }
        }
        
        public bool IsMapLoaded(int level)
        {
            return _scrollController.IsMapLoaded(level);
        }
        
        public MapData GetLoadedMap(int level)
        {
            return _scrollController.GetLoadedMap(level);
        }
        
        public BlockType GetBlockTypeAt(Vector2Int position, int level)
        {
            return _scrollController.GetBlockTypeAt(position, level);
        }
        
        // TODO: プレイヤー限定じゃなくする ( Enemyも通過可能か判定したい ).
        public virtual bool CanPlayerPassThrough(Vector2Int position, int level)
        {
            return CanPassThrough(position, level);
        }
        
        public void OnPlayerHitTile(Vector2Int position, int level)
        {
            if (!_scrollController.IsMapLoaded(level))
                return;

            var mapData = _scrollController.GetLoadedMap(level);
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
                _scrollController.SetMap(level, newMapData);

                // タイルの表示を更新
                UpdateTileDisplay(position, newTileType, level);
            }
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
            var mapData = _scrollController.GetLoadedMap(level);
            var tileType = mapData.Tiles[position.x, position.y];
            
            // 新仕様: Sky=不可, Empty=可, Ground=可, Rock=不可, Treasure=可
            return tileType == BlockType.Empty || tileType == BlockType.Ground || tileType == BlockType.Treasure;
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
                    
                    // 2DSprite座標系: スプライト中心が原点、隙間なく並べる
                    var position = new Vector3(x, y, 0);
                    var tileInstance = UnityEngine.Object.Instantiate(universalTilePrefab, position, Quaternion.identity, transform);
                    
                    // TileControllerでBlockTypeを設定
                    var tileController = tileInstance.GetComponent<TileController>();
                    if (tileController != null)
                    {
                        tileController.BlockType = tileType;
                    }
                    
                    // タイルにレベル情報を設定（デバッグ用）
                    tileInstance.name = $"{tileType}_Level{mapData.Level}_{x}_{y}";
                    
                    tileInstances.Add(tileInstance);
                }
            }

            _instantiatedTiles[mapData.Level] = tileInstances;
            _scrollController.SetMap(mapData.Level, mapData);
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
                    
                    // 重複エリア（下5マス）での既存ブロック保護チェック
                    var position = new Vector3(x, y, 0);
                    
                    // 重複エリアで既存のWallブロックとの衝突チェック
                    if (y < overlapHeight && IsExistingWallBlockAtPosition(position))
                    {
                        Debug.Log($"重複エリアで既存Wallブロックを保護: Level{mapData.Level} 位置({x}, {y})");
                        continue; // 既存Wallブロックを保護するため、新しいタイルを生成しない
                    }
                    
                    var tileInstance = UnityEngine.Object.Instantiate(universalTilePrefab, position, Quaternion.identity, transform);
                    
                    // TileControllerでBlockTypeを設定
                    var tileController = tileInstance.GetComponent<TileController>();
                    if (tileController != null)
                    {
                        tileController.BlockType = tileType;
                    }
                    
                    // タイルにレベル情報を設定（デバッグ用）
                    tileInstance.name = $"{tileType}_Level{mapData.Level}_{x}_{y}";
                    
                    tileInstances.Add(tileInstance);
                }
            }

            _instantiatedTiles[mapData.Level] = tileInstances;
            _scrollController.SetMap(mapData.Level, mapData);
            PlaceTilesWithOverlapProtection(mapData, overlapHeight: 5);
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
            _scrollController.RemoveMap(level);
        }

        public void OptimizeMemory(int currentLevel)
        {
            // いったん何もしない.
        }

        public List<GameObject> GetTilesForLevel(int level)
        {
            return _instantiatedTiles.ContainsKey(level) ? _instantiatedTiles[level] : null;
        }

        private void UpdateTileDisplay(Vector2Int position, BlockType newBlockType, int level)
        {
            if (!_instantiatedTiles.ContainsKey(level))
                return;

            var tileInstances = _instantiatedTiles[level];
            
            // 該当するタイルを検索してTileControllerでBlockTypeを更新
            for (int i = 0; i < tileInstances.Count; i++)
            {
                var tileInstance = tileInstances[i];
                if (tileInstance != null && tileInstance.name.Contains($"_{position.x}_{position.y}"))
                {
                    var tileController = tileInstance.GetComponent<TileController>();
                    if (tileController != null)
                    {
                        tileController.BlockType = newBlockType;
                        tileInstance.name = $"{newBlockType}_Level{level}_{position.x}_{position.y}";
                    }
                    break;
                }
            }
        }
    }
}
using System;
using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem.Generation;
using MyGame.Player;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;

namespace MyGame.TilemapSystem
{
    public class TilemapSystemController : MonoBehaviour
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
        private TilemapManager _manager;
        private SeedManager _seedManager;
        private TilemapScrollController _scrollController;
        
        public TilemapManager Manager => _manager;
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

            _manager = new TilemapManager(transform, universalTilePrefab);
            _manager.OnMapGenerated.Subscribe(OnMapGenerated).AddTo(this);
            
            // スクロールコントローラーを初期化
            _scrollController = new TilemapScrollController(_generator, _manager, transform);
            _scrollController.OnScrollStarted?.Subscribe(OnScrollStarted).AddTo(this);
            _scrollController.OnScrollCompleted?.Subscribe(OnScrollCompleted).AddTo(this);
            _scrollController.OnNewLevelGenerated?.Subscribe(OnNewLevelGenerated).AddTo(this);
            
            // PlayerControllerを検索して設定
            SetupPlayerController();
        }

        private void GenerateInitialMap()
        {
            if (_generator == null)
            {
                Debug.LogError("TilemapGenerator is not initialized");
                return;
            }

            var mapData = _generator.GenerateMap(initialLevel, initialSeed);
            
            if (_manager != null)
            {
                _manager.PlaceTiles(mapData);
            }

            Debug.Log($"初期マップ生成完了: Level={mapData.Level}, Seed={mapData.Seed}, Size={mapData.Width}x{mapData.Height}");
        }

        private void OnMapGenerated(MapData mapData)
        {
            Debug.Log($"マップ配置完了: Level {mapData.Level}");
        }

        [ContextMenu("新しいマップを生成")]
        public void GenerateNewMap()
        {
            var newLevel = CurrentLevel + 1;
            var mapData = _generator.GenerateMap(newLevel, initialSeed);
            _manager.PlaceTiles(mapData);
            Debug.Log($"新しいマップ生成: Level {newLevel}");
        }

        [ContextMenu("メモリ最適化実行")]
        public void OptimizeMemory()
        {
            if (_manager != null)
            {
                _manager.OptimizeMemory(CurrentLevel);
                Debug.Log($"メモリ最適化実行: Current Level {CurrentLevel}");
            }
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
        
        private void SetupPlayerController()
        {
            // シーン内のPlayerControllerを検索
            var playerController = FindFirstObjectByType<PlayerController>();
            
            if (playerController != null)
            {
                // TilemapManagerをPlayerControllerに設定
                playerController.SetTilemapManager(_manager, CurrentLevel);
                Debug.Log($"[TilemapSystemController] PlayerControllerにTilemapManagerを設定しました - Level: {CurrentLevel}");
            }
            else
            {
                Debug.LogWarning("[TilemapSystemController] PlayerControllerが見つかりません。プレイヤーの移動制約が機能しない可能性があります。");
            }
        }
        
        [ContextMenu("PlayerController再設定")]
        public void ResetupPlayerController()
        {
            SetupPlayerController();
        }
        
        // 公開メソッド - 他のシステムから利用可能
        public void SetAutoScrollEnabled(bool autoScrollState)
        {
            autoScrollEnabled = autoScrollState;
            if (autoScrollState)
            {
                StartAutoScrollManual();
            }
        }
        
        public void SetAutoScrollInterval(float interval)
        {
            autoScrollInterval = interval;
        }
        
        public bool IsMapLoaded(int level)
        {
            return _manager?.IsMapLoaded(level) ?? false;
        }
        
        public MapData GetLoadedMap(int level)
        {
            return _manager?.GetLoadedMap(level) ?? default;
        }
        
        public BlockType GetBlockTypeAt(Vector2Int position, int level)
        {
            return _manager?.GetBlockTypeAt(position, level) ?? BlockType.Empty;
        }
        
        public bool CanPlayerPassThrough(Vector2Int position, int level)
        {
            return _manager?.CanPlayerPassThrough(position, level) ?? true;
        }
        
        public void OnPlayerHitTile(Vector2Int position, int level)
        {
            _manager?.OnPlayerHitTile(position, level);
        }
    }
}
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
    public class TilemapSystemTester : MonoBehaviour
    {
        [Header("Tile Prefabs")]
        [SerializeField] private GameObject skyTilePrefab;
        [SerializeField] private GameObject groundTilePrefab;
        [SerializeField] private GameObject rockTilePrefab;
        [SerializeField] private GameObject treasureTilePrefab;
        
        [Header("Test Settings")]
        [SerializeField] private int testLevel = 1;
        [SerializeField] private int testSeed = 12345;
        [Header("Auto Scroll Settings")]
        [SerializeField] private bool autoScrollEnabled = true;
        [SerializeField] private float autoScrollInterval = 3.0f;

        private TilemapGenerator _generator;
        private TilemapManager _manager;
        private SeedManager _seedManager;
        private TilemapScrollController _scrollController;
        
        private void Start()
        {
            InitializeSystem();
            GenerateTestMap();
            
            // 自動スクロールを開始
            if (autoScrollEnabled)
            {
                StartAutoScroll().Forget();
            }
        }

        private void InitializeSystem()
        {
            _seedManager = new SeedManager(testSeed);
            _generator = new TilemapGenerator(_seedManager);

            var tilePrefabs = new Dictionary<BlockType, GameObject>
            {
                { BlockType.Sky, skyTilePrefab },
                { BlockType.Ground, groundTilePrefab },
                { BlockType.Rock, rockTilePrefab },
                { BlockType.Treasure, treasureTilePrefab }
            };

            _manager = new TilemapManager(transform, tilePrefabs);
            _manager.OnMapGenerated.Subscribe(OnMapGenerated).AddTo(this);
            
            // スクロールコントローラーを初期化
            _scrollController = new TilemapScrollController(_generator, _manager, transform);
            _scrollController.OnScrollStarted?.Subscribe(OnScrollStarted).AddTo(this);
            _scrollController.OnScrollCompleted?.Subscribe(OnScrollCompleted).AddTo(this);
            _scrollController.OnNewLevelGenerated?.Subscribe(OnNewLevelGenerated).AddTo(this);
            
            // PlayerControllerを検索して設定
            SetupPlayerController();
        }

        private void GenerateTestMap()
        {
            if (_generator == null)
            {
                Debug.LogError("TilemapGenerator is not initialized");
                return;
            }

            var mapData = _generator.GenerateMap(testLevel, testSeed);
            
            if (_manager != null)
            {
                _manager.PlaceTiles(mapData);
            }

            Debug.Log($"マップ生成完了: Level={mapData.Level}, Seed={mapData.Seed}, Size={mapData.Width}x{mapData.Height}");
        }

        private void OnMapGenerated(MapData mapData)
        {
            Debug.Log($"マップ配置完了: Level {mapData.Level}");
        }

        [ContextMenu("新しいマップを生成")]
        private void GenerateNewMap()
        {
            testLevel++;
            GenerateTestMap();
        }

        [ContextMenu("メモリ最適化テスト")]
        private void TestMemoryOptimization()
        {
            if (_manager != null)
            {
                _manager.OptimizeMemory(testLevel);
                Debug.Log($"メモリ最適化実行: Current Level {testLevel}");
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
        private void ManualScroll()
        {
            if (_scrollController != null)
            {
                _scrollController.StartScrollAsync().Forget();
            }
        }
        
        [ContextMenu("自動スクロール停止")]
        private void StopAutoScroll()
        {
            autoScrollEnabled = false;
            Debug.Log("自動スクロールを停止しました");
        }
        
        [ContextMenu("自動スクロール開始")]
        private void StartAutoScrollManual()
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
            var playerController = FindObjectOfType<PlayerController>();
            
            if (playerController != null)
            {
                // TilemapManagerをPlayerControllerに設定
                playerController.SetTilemapManager(_manager, testLevel);
                Debug.Log($"[TilemapSystemTester] PlayerControllerにTilemapManagerを設定しました - Level: {testLevel}");
            }
            else
            {
                Debug.LogWarning("[TilemapSystemTester] PlayerControllerが見つかりません。プレイヤーの移動制約が機能しない可能性があります。");
            }
        }
        
        [ContextMenu("PlayerController再設定")]
        private void ResetupPlayerController()
        {
            SetupPlayerController();
        }
    }
}
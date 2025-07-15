using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem.Generation;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace MyGame.TilemapSystem
{
    public class TilemapSystemTester : MonoBehaviour
    {
        [Header("Tile Prefabs")]
        [SerializeField] private GameObject wallTilePrefab;
        [SerializeField] private GameObject groundTilePrefab;
        
        [Header("Test Settings")]
        [SerializeField] private int testLevel = 1;
        [SerializeField] private int testSeed = 12345;

        private TilemapGenerator _generator;
        private TilemapManager _manager;
        private SeedManager _seedManager;
        private TilemapScrollController _scrollController;

        private void Start()
        {
            InitializeSystem();
            GenerateTestMap();
            
            // 3秒後にスクロール開始
            StartScrollAfterDelay().Forget();
        }

        private void InitializeSystem()
        {
            _seedManager = new SeedManager(testSeed);
            _generator = new TilemapGenerator(_seedManager);

            var tilePrefabs = new Dictionary<TileType, GameObject>
            {
                { TileType.Wall, wallTilePrefab },
                { TileType.Ground, groundTilePrefab }
            };

            _manager = new TilemapManager(transform, tilePrefabs);
            _manager.OnMapGenerated += OnMapGenerated;
            
            // スクロールコントローラーを初期化
            _scrollController = new TilemapScrollController(_generator, _manager, transform);
            _scrollController.OnScrollStarted += OnScrollStarted;
            _scrollController.OnScrollCompleted += OnScrollCompleted;
            _scrollController.OnNewLevelGenerated += OnNewLevelGenerated;
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

        private async UniTask StartScrollAfterDelay()
        {
            await UniTask.Delay(3000); // 3秒待機
            
            if (_scrollController != null)
            {
                Debug.Log("3秒経過、スクロールを開始します");
                await _scrollController.StartScrollAsync();
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

        private void OnDestroy()
        {
            if (_manager != null)
            {
                _manager.OnMapGenerated -= OnMapGenerated;
            }
            
            if (_scrollController != null)
            {
                _scrollController.OnScrollStarted -= OnScrollStarted;
                _scrollController.OnScrollCompleted -= OnScrollCompleted;
                _scrollController.OnNewLevelGenerated -= OnNewLevelGenerated;
            }
        }
    }
}
using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem.Generation;
using Cysharp.Threading.Tasks;
using R3;
using VContainer;

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

        [Inject]
        private ITilemapManager _tilemapManager;
        
        private TilemapGenerator _generator;
        private SeedManager _seedManager;
        private TilemapScrollController _scrollController;
        
        public ITilemapManager TilemapManager => _tilemapManager;
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
            
            // TilemapServiceはVContainerから注入されるため、ここでは初期化しない
            // OnMapGeneratedイベントのサブスクライブは、TilemapServiceがインターフェースになったため削除
            
            // スクロールコントローラーを初期化
            _scrollController = new TilemapScrollController(_generator, _tilemapManager, transform);
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
            
            if (_tilemapManager != null)
            {
                // 初期マップ生成時は重複保護不要なので、PlaceTilesWithOverlapProtectionを使用
                _tilemapManager.PlaceTilesWithOverlapProtection(mapData, 0);
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
            _tilemapManager.PlaceTilesWithOverlapProtection(mapData, 0);
            Debug.Log($"新しいマップ生成: Level {newLevel}");
        }

        [ContextMenu("メモリ最適化実行")]
        public void OptimizeMemory()
        {
            if (_tilemapManager != null)
            {
                _tilemapManager.OptimizeMemory(CurrentLevel);
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
        
        // ITilemapManagerの実装は削除（VContainerによる注入を使用）
    }
}
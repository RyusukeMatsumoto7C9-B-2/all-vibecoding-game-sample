using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace MyGame.TilemapSystem.Core
{
    public class TilemapScrollController
    {
        private readonly TilemapGenerator _generator;
        private readonly TilemapManager _manager;
        private readonly Transform _tilemapParent;
        
        private int _currentLevel = 1;
        private bool _isScrolling = false;
        private float _scrollSpeed = 5.0f;
        private float _scrollDistance = 25.0f; // 25マス分
        private IScrollTrigger _scrollTrigger;
        
        public event Action<int> OnScrollStarted;
        public event Action<int> OnScrollCompleted;
        public event Action<int> OnNewLevelGenerated;
        
        public bool IsScrolling => _isScrolling;
        public int CurrentLevel => _currentLevel;
        public float ScrollSpeed => _scrollSpeed;
        public float ScrollDistance => _scrollDistance;
        
        public TilemapScrollController(TilemapGenerator generator, TilemapManager manager, Transform tilemapParent)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _tilemapParent = tilemapParent ?? throw new ArgumentNullException(nameof(tilemapParent));
        }
        
        public void SetScrollSpeed(float speed)
        {
            if (speed <= 0)
            {
                Debug.LogWarning("ScrollSpeed must be positive");
                return;
            }
            _scrollSpeed = speed;
        }
        
        public void SetScrollDistance(float distance)
        {
            if (distance <= 0)
            {
                Debug.LogWarning("ScrollDistance must be positive");
                return;
            }
            _scrollDistance = distance;
        }
        
        public async UniTask StartScrollAsync()
        {
            if (_isScrolling)
            {
                Debug.LogWarning("Already scrolling");
                return;
            }
            
            _isScrolling = true;
            OnScrollStarted?.Invoke(_currentLevel);
            
            try
            {
                // 次のレベルを事前生成
                await GenerateNextLevelAsync();
                
                // スクロール実行
                await PerformScrollAsync();
                
                // 現在レベルを更新
                _currentLevel++;
                
                // メモリ最適化
                _manager.OptimizeMemory(_currentLevel);
                
                OnScrollCompleted?.Invoke(_currentLevel);
            }
            catch (Exception e)
            {
                Debug.LogError($"Scroll failed: {e.Message}");
                throw;
            }
            finally
            {
                _isScrolling = false;
            }
        }
        
        private async UniTask GenerateNextLevelAsync()
        {
            int nextLevel = _currentLevel + 1;
            
            // 既に生成済みの場合はスキップ
            if (_manager.IsMapLoaded(nextLevel))
            {
                return;
            }
            
            await UniTask.Yield();
            
            // 次のレベルのマップを生成
            var nextMapData = _generator.GenerateMap(nextLevel, _generator.GetSeedForLevel(nextLevel));
            
            // 重複エリアの適切な処理
            // スクロール前の配置: 新しいレベルを下側に配置し、スクロール後に正しい位置に来るようにする
            int overlapHeight = 5; // 重複エリア：5マス
            int levelOffset = TilemapGenerator.MAP_HEIGHT - overlapHeight; // 25マス分
            
            // 次のレベルのタイルを生成し、重複エリアを考慮してオフセット
            _manager.PlaceTiles(nextMapData);
            
            // 次のレベルのタイルを正しい位置に配置
            // スクロール後に重複エリアが適切に配置されるよう、-25マス分オフセットする
            float correctOffset = -_scrollDistance; // -25マス分のオフセット
            
            OffsetTilesForLevel(nextLevel, new Vector3(0, correctOffset, 0));
            
            OnNewLevelGenerated?.Invoke(nextLevel);
        }
        
        private void OffsetTilesForLevel(int level, Vector3 offset)
        {
            var tiles = _manager.GetTilesForLevel(level);
            if (tiles == null) return;
            
            foreach (var tile in tiles)
            {
                if (tile != null)
                {
                    // 座標系の正確な計算 - レベル間の相対位置を考慮
                    var currentPos = tile.transform.position;
                    var newPos = new Vector3(
                        currentPos.x, 
                        currentPos.y + offset.y, 
                        currentPos.z
                    );
                    tile.transform.position = newPos;
                }
            }
        }
        
        private async UniTask PerformScrollAsync()
        {
            float elapsedTime = 0;
            float duration = _scrollDistance / _scrollSpeed;
            Vector3 startPosition = _tilemapParent.position;
            Vector3 targetPosition = startPosition + new Vector3(0, _scrollDistance, 0);
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                
                // スムーズなスクロール補間
                _tilemapParent.position = Vector3.Lerp(startPosition, targetPosition, t);
                
                await UniTask.Yield();
            }
            
            // 最終位置に確実に設定
            _tilemapParent.position = targetPosition;
        }
        
        public void SetCurrentLevel(int level)
        {
            if (level < 1)
            {
                Debug.LogWarning("Level must be 1 or greater");
                return;
            }
            
            _currentLevel = level;
        }
        
        /// <summary>
        /// スクロールトリガーを登録
        /// </summary>
        /// <param name="trigger">スクロールトリガー</param>
        public void RegisterScrollTrigger(IScrollTrigger trigger)
        {
            if (trigger == null)
            {
                Debug.LogWarning("ScrollTrigger cannot be null");
                return;
            }
            
            // 既存のトリガーがあれば解除
            UnregisterScrollTrigger();
            
            _scrollTrigger = trigger;
            _scrollTrigger.OnScrollStarted += HandleScrollStarted;
            _scrollTrigger.OnScrollPositionChanged += HandleScrollPositionChanged;
            _scrollTrigger.OnScrollCompleted += HandleScrollCompleted;
        }
        
        /// <summary>
        /// スクロールトリガーを解除
        /// </summary>
        public void UnregisterScrollTrigger()
        {
            if (_scrollTrigger != null)
            {
                _scrollTrigger.OnScrollStarted -= HandleScrollStarted;
                _scrollTrigger.OnScrollPositionChanged -= HandleScrollPositionChanged;
                _scrollTrigger.OnScrollCompleted -= HandleScrollCompleted;
                _scrollTrigger = null;
            }
        }
        
        /// <summary>
        /// スクロール開始イベントハンドラー
        /// </summary>
        private void HandleScrollStarted()
        {
            if (!_isScrolling)
            {
                StartScrollAsync().Forget();
            }
        }
        
        /// <summary>
        /// スクロール位置変更イベントハンドラー
        /// </summary>
        /// <param name="scrollPosition">スクロール位置</param>
        private void HandleScrollPositionChanged(float scrollPosition)
        {
            // 必要に応じて、スクロール位置に応じた処理を追加
            // 例: 特定の位置に達したら次のレベルを生成
            if (scrollPosition >= _currentLevel * _scrollDistance)
            {
                if (!_isScrolling)
                {
                    StartScrollAsync().Forget();
                }
            }
        }
        
        /// <summary>
        /// スクロール完了イベントハンドラー
        /// </summary>
        private void HandleScrollCompleted()
        {
            // スクロール完了時の追加処理
            Debug.Log($"Scroll completed for level {_currentLevel}");
        }
    }
}
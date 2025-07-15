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
            
            // 現在のマップの上に配置（Y座標をオフセット）
            var offsetMapData = new MapData(
                nextMapData.Width,
                nextMapData.Height,
                nextMapData.Tiles,
                nextMapData.Seed,
                nextMapData.Level
            );
            
            _manager.PlaceTiles(offsetMapData);
            
            // 次のレベルのタイルを上方向にオフセット
            OffsetTilesForLevel(nextLevel, new Vector3(0, _scrollDistance, 0));
            
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
                    tile.transform.position += offset;
                }
            }
        }
        
        private async UniTask PerformScrollAsync()
        {
            float elapsedTime = 0;
            float duration = _scrollDistance / _scrollSpeed;
            Vector3 startPosition = _tilemapParent.position;
            Vector3 targetPosition = startPosition + new Vector3(0, -_scrollDistance, 0);
            
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
    }
}
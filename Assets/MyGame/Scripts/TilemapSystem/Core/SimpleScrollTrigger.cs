using System;
using UnityEngine;

namespace MyGame.TilemapSystem.Core
{
    /// <summary>
    /// シンプルなスクロールトリガー実装
    /// キー入力でスクロール動作をトリガーする
    /// </summary>
    public class SimpleScrollTrigger : MonoBehaviour, IScrollTrigger
    {
        public event Action<float> OnScrollPositionChanged;
        public event Action OnScrollCompleted;
        public event Action OnScrollStarted;
        
        [SerializeField] private float _scrollSpeed = 10.0f;
        [SerializeField] private float _scrollDistance = 25.0f;
        [SerializeField] private KeyCode _scrollKey = KeyCode.Space;
        
        private float _currentScrollPosition = 0f;
        private bool _isScrolling = false;
        private float _scrollStartTime;
        private float _scrollStartPosition;
        
        public float CurrentScrollPosition => _currentScrollPosition;
        public bool IsScrolling => _isScrolling;
        
        void Update()
        {
            if (Input.GetKeyDown(_scrollKey) && !_isScrolling)
            {
                StartScroll();
            }
            
            if (_isScrolling)
            {
                UpdateScroll();
            }
        }
        
        private void StartScroll()
        {
            _isScrolling = true;
            _scrollStartTime = Time.time;
            _scrollStartPosition = _currentScrollPosition;
            OnScrollStarted?.Invoke();
        }
        
        private void UpdateScroll()
        {
            float elapsedTime = Time.time - _scrollStartTime;
            float duration = _scrollDistance / _scrollSpeed;
            
            if (elapsedTime >= duration)
            {
                // スクロール完了
                _currentScrollPosition = _scrollStartPosition + _scrollDistance;
                _isScrolling = false;
                OnScrollPositionChanged?.Invoke(_currentScrollPosition);
                OnScrollCompleted?.Invoke();
            }
            else
            {
                // スクロール中
                float progress = elapsedTime / duration;
                _currentScrollPosition = _scrollStartPosition + (_scrollDistance * progress);
                OnScrollPositionChanged?.Invoke(_currentScrollPosition);
            }
        }
        
        /// <summary>
        /// 手動でスクロール開始
        /// </summary>
        public void TriggerScroll()
        {
            if (!_isScrolling)
            {
                StartScroll();
            }
        }
        
        /// <summary>
        /// スクロール位置をリセット
        /// </summary>
        public void ResetScrollPosition()
        {
            _currentScrollPosition = 0f;
            _isScrolling = false;
        }
    }
}
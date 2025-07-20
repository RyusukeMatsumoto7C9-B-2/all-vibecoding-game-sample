using UnityEngine;
using R3;

namespace MyGame.TilemapSystem.Core
{
    /// <summary>
    /// シンプルなスクロールトリガー実装
    /// キー入力でスクロール動作をトリガーする
    /// </summary>
    public class SimpleScrollTrigger : MonoBehaviour, IScrollTrigger
    {

        public Observable<float> OnScrollPositionChanged => _onScrollPositionChanged;
        public Observable<Unit> OnScrollCompleted => _onScrollCompleted;
        public Observable<Unit> OnScrollStarted => _onScrollStarted;
        
        [SerializeField] private float _scrollSpeed = 10.0f;
        [SerializeField] private float _scrollDistance = 25.0f;
        [SerializeField] private KeyCode _scrollKey = KeyCode.Space;
        
        private readonly Subject<float> _onScrollPositionChanged = new Subject<float>();
        private readonly Subject<Unit> _onScrollCompleted = new Subject<Unit>();
        private readonly Subject<Unit> _onScrollStarted = new Subject<Unit>();
        private float _currentScrollPosition = 0f;
        private bool _isScrolling = false;
        private float _scrollStartTime;
        private float _scrollStartPosition;
        
        public float CurrentScrollPosition => _currentScrollPosition;
        public bool IsScrolling => _isScrolling;
        
        
        private void Update()
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

        private void OnDestroy()
        {
            _onScrollPositionChanged.Dispose();
        }

        private void StartScroll()
        {
            _isScrolling = true;
            _scrollStartTime = Time.time;
            _scrollStartPosition = _currentScrollPosition;
            _onScrollStarted.OnNext(Unit.Default);
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
                _onScrollPositionChanged?.OnNext(_currentScrollPosition);
                _onScrollCompleted?.OnNext(Unit.Default);
            }
            else
            {
                // スクロール中
                float progress = elapsedTime / duration;
                _currentScrollPosition = _scrollStartPosition + (_scrollDistance * progress);
                _onScrollPositionChanged?.OnNext(_currentScrollPosition);
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
using System;

namespace MyGame.TilemapSystem.Core
{
    /// <summary>
    /// スクロールトリガーインターフェース
    /// 外部スクロールシステムとタイルマップシステムの連携を提供
    /// </summary>
    public interface IScrollTrigger
    {
        /// <summary>
        /// スクロール位置変更時のイベント
        /// </summary>
        event Action<float> OnScrollPositionChanged;
        
        /// <summary>
        /// スクロール完了時のイベント
        /// </summary>
        event Action OnScrollCompleted;
        
        /// <summary>
        /// スクロール開始時のイベント
        /// </summary>
        event Action OnScrollStarted;
        
        /// <summary>
        /// 現在のスクロール位置
        /// </summary>
        float CurrentScrollPosition { get; }
        
        /// <summary>
        /// スクロール中かどうか
        /// </summary>
        bool IsScrolling { get; }
    }
}
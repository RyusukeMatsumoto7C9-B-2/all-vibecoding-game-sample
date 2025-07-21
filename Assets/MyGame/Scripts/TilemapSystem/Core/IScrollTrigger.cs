using System;
using R3;

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
        Observable<float> OnScrollPositionChanged { get; }

        /// <summary>
        /// スクロール完了時のイベント
        /// </summary>
        Observable<Unit> OnScrollCompleted { get; }

        /// <summary>
        /// スクロール開始時のイベント
        /// </summary>
        Observable<Unit> OnScrollStarted { get; }

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
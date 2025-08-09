using UnityEngine;

namespace MyGame.Enemy.Spawn
{
    [CreateAssetMenu(fileName = "SpawnConfig", menuName = "MyGame/ScriptableObject/EnemySpawnConfig", order = 0)]
    public class EnemySpawnConfig : ScriptableObject, IEnemySpawnConfig
    {
        /// <summary>
        /// 敵の基本スポーン数
        /// </summary>
        [field: SerializeField] public int BaseEnemyCount { get; internal set; }

        /// <summary>
        /// 敵増加のレベル間隔
        /// </summary>
        [field: SerializeField] public int EnemyIncreaseInterval { get; internal set; }

        /// <summary>
        /// レベルアップ時の敵増加数
        /// </summary>
        [field: SerializeField] public int EnemyIncreaseAmount { get; internal set; }

        /// <summary>
        /// 敵の最大スポーン数
        /// </summary>
        [field: SerializeField] public int MaxEnemyCount { get; internal set; }

        /// <summary>
        /// スポーンエリアのマージン
        /// </summary>
        [field: SerializeField] public int SpawnAreaMargin { get; internal set; }

        /// <summary>
        /// プレイヤーからの最小スポーン距離
        /// </summary>
        [field: SerializeField] public int MinSpawnDistanceFromPlayer { get; internal set; }

        
        /// <summary>
        /// 指定されたレベルに応じた敵のスポーン数を計算する
        /// </summary>
        /// <param name="level">現在のレベル</param>
        /// <returns>スポーンする敵の数</returns>
        public int GetEnemyCountForLevel(int level)
        {
            if (level <= 0)
            {
                return BaseEnemyCount;
            }
            
            // レベル増加に応じた追加敵数の計算式: (レベル - 1) ÷ 増加間隔 × 増加量
            var additionalEnemies = ((level - 1) / EnemyIncreaseInterval) * EnemyIncreaseAmount;
            var totalEnemies = BaseEnemyCount + additionalEnemies;
            
            return Mathf.Min(totalEnemies, MaxEnemyCount);
        }


        /// <summary>
        /// EnemySpawnConfigインスタンスを作成する
        /// </summary>
        /// <param name="baseEnemyCount">基本敵数</param>
        /// <param name="enemyIncreaseInterval">敵増加間隔</param>
        /// <param name="enemyIncreaseAmount">敵増加量</param>
        /// <param name="maxEnemyCount">最大敵数</param>
        /// <param name="spawnAreaMargin">スポーンエリアマージン</param>
        /// <param name="minSpawnDistanceFromPlayer">プレイヤーからの最小スポーン距離</param>
        /// <returns>設定済みのEnemySpawnConfigインスタンス</returns>
        public static EnemySpawnConfig Create(int baseEnemyCount, int enemyIncreaseInterval, int enemyIncreaseAmount, int maxEnemyCount, int spawnAreaMargin = 0, int minSpawnDistanceFromPlayer = 0)
        {
            var config = CreateInstance<EnemySpawnConfig>();
            config.BaseEnemyCount = baseEnemyCount;
            config.EnemyIncreaseInterval = enemyIncreaseInterval;
            config.EnemyIncreaseAmount = enemyIncreaseAmount;
            config.MaxEnemyCount = maxEnemyCount;
            config.SpawnAreaMargin = spawnAreaMargin;
            config.MinSpawnDistanceFromPlayer = minSpawnDistanceFromPlayer;
            
            return config;
        }
    }
}
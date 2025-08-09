using UnityEngine;

namespace MyGame.Enemy.Spawn
{
    [CreateAssetMenu(fileName = "SpawnConfig", menuName = "MyGame/ScriptableObject/EnemySpawnConfig", order = 0)]
    public class EnemySpawnConfig : ScriptableObject, IEnemySpawnConfig
    {
        [field: SerializeField] public int BaseEnemyCount { get; private set; }
        [field: SerializeField] public int EnemyIncreaseInterval { get; private set; }
        [field: SerializeField] public int EnemyIncreaseAmount { get; private set; }
        [field: SerializeField] public int MaxEnemyCount { get; private set; }
        [field: SerializeField] public int SpawnAreaMargin { get; private set; }
        [field: SerializeField] public int MinSpawnDistanceFromPlayer { get; private set; }

        
        public int GetEnemyCountForLevel(int level)
        {
            if (level <= 0)
            {
                return BaseEnemyCount;
            }
            
            var additionalEnemies = ((level - 1) / EnemyIncreaseInterval) * EnemyIncreaseAmount;
            var totalEnemies = BaseEnemyCount + additionalEnemies;
            
            return Mathf.Min(totalEnemies, MaxEnemyCount);
        }


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
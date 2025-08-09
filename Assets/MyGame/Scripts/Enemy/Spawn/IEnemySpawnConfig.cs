namespace MyGame.Enemy.Spawn
{
    /// <summary>
    /// EnemySpawn用の設定ファイルインターフェース.
    /// </summary>
    public interface IEnemySpawnConfig
    {
        public int BaseEnemyCount { get; }
        public int EnemyIncreaseInterval { get; }
        public int EnemyIncreaseAmount { get; }
        public int MaxEnemyCount { get; }
        public int SpawnAreaMargin { get; }
        public int MinSpawnDistanceFromPlayer { get; }


        /// <summary>
        /// レベル別出現数計算.
        /// </summary>
        /// <param name="level"> 現在のプレイヤーレベル. </param>
        /// <returns>指定したレベルで生成可能なエネミー数.</returns>
        int GetEnemyCountForLevel(int level);
    }
}
using UnityEngine;


namespace MyGame.Enemy.Spawn.Tests
{
    /// <summary>
    /// EnemySpawnConfigテスト用ホルダークラス.
    /// テストコードから参照を得るためだけのヘルパークラスでありゲーム本編では利用しない.
    /// </summary>
    public class EnemySpawnConfigHolder : MonoBehaviour
    {
        [field: SerializeField]
        public EnemySpawnConfig Config { get; private set; }
    }
}

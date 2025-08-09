using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyGame.Enemy.Spawn
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private EnemySpawnConfig _spawnConfig;
        [SerializeField] private Transform _enemyContainer;
        
        private readonly List<GameObject> _activeEnemies = new List<GameObject>();

        /// <summary>
        /// 現在アクティブな敵の数を取得する
        /// </summary>
        public int ActiveEnemyCount
        {
            get
            {
                // 破棄された敵オブジェクトをリストから除去
                _activeEnemies.RemoveAll(enemy => enemy == null);
                return _activeEnemies.Count;
            }
        }

        /// <summary>
        /// 指定されたレベルに応じて敵をスポーンする
        /// </summary>
        /// <param name="level">現在のレベル（0以上の整数）</param>
        public void SpawnEnemiesForLevel(int level)
        {
            if (level < 0)
            {
                Debug.LogError("設定するレベルは0以上の整数です、エネミーの生成を実行しません.");
                return;
            }
            
            if (_enemyPrefab == null || _spawnConfig == null)
            {
                Debug.LogError("EnemyPrefab または SpawnConfig が設定されていません.");
                return;
            }
            
            var enemyCount = _spawnConfig.GetEnemyCountForLevel(level);
            
            for (var i = 0; i < enemyCount; i++)
            {
                var spawnPosition = CalculateSpawnPosition(i, enemyCount);
                var enemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity, _enemyContainer);
                _activeEnemies.Add(enemy);
            }
        }

        /// <summary>
        /// 現在スポーンされているすべての敵を削除する
        /// </summary>
        public void ClearAllEnemies()
        {
            foreach (var enemy in _activeEnemies.Where(enemy => enemy != null))
            {
                Destroy(enemy);
            }
            
            _activeEnemies.Clear();
        }
        
        private Vector3 CalculateSpawnPosition(int index, int totalCount)
        {
            var camera = Camera.main;
            if (camera == null)
            {
                Debug.LogWarning("Main Camera が見つかりません. デフォルト位置に生成します.");
                return Vector3.zero;
            }
            
            var screenHeight = camera.orthographicSize * 2;
            var screenWidth = screenHeight * camera.aspect;
            
            var margin = _spawnConfig.SpawnAreaMargin;
            
            // 敵を左右交互に配置するための判定
            var isLeftSide = index % 2 == 0;
            var xPosition = isLeftSide ? -screenWidth / 2 - margin : screenWidth / 2 + margin;
            
            // Y座標のオフセット計算：敵を縦方向に均等分散配置
            var yOffset = (index / 2f - (totalCount - 1) / 4f) * 2f;
            var yPosition = Mathf.Clamp(yOffset, -screenHeight / 2 + 1, screenHeight / 2 - 1);
            
            return new Vector3(xPosition, yPosition, 0) + camera.transform.position;
        }
    }
}
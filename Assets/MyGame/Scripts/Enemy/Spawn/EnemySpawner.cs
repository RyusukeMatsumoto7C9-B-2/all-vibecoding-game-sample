using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyGame.Enemy.Spawn
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private EnemySpawnConfig spawnConfig;
        [SerializeField] private Transform enemyContainer;
        
        private readonly List<GameObject> activeEnemies = new List<GameObject>();

        public int ActiveEnemyCount
        {
            get
            {
                activeEnemies.RemoveAll(enemy => enemy == null);
                return activeEnemies.Count;
            }
        }

        public void SpawnEnemiesForLevel(int level)
        {
            if (level < 0)
            {
                Debug.LogError("設定するレベルは0以上の整数です、エネミーの生成を実行しません.");
                return;
            }
            
            if (enemyPrefab == null || spawnConfig == null)
            {
                Debug.LogError("EnemyPrefab または SpawnConfig が設定されていません.");
                return;
            }
            
            var enemyCount = spawnConfig.GetEnemyCountForLevel(level);
            
            for (var i = 0; i < enemyCount; i++)
            {
                var spawnPosition = CalculateSpawnPosition(i, enemyCount);
                var enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, enemyContainer);
                activeEnemies.Add(enemy);
            }
        }

        public void ClearAllEnemies()
        {
            foreach (var enemy in activeEnemies.Where(enemy => enemy != null))
            {
                Destroy(enemy);
            }
            
            activeEnemies.Clear();
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
            
            var margin = spawnConfig.SpawnAreaMargin;
            
            var isLeftSide = index % 2 == 0;
            var xPosition = isLeftSide ? -screenWidth / 2 - margin : screenWidth / 2 + margin;
            
            var yOffset = (index / 2f - (totalCount - 1) / 4f) * 2f;
            var yPosition = Mathf.Clamp(yOffset, -screenHeight / 2 + 1, screenHeight / 2 - 1);
            
            return new Vector3(xPosition, yPosition, 0) + camera.transform.position;
        }
    }
}
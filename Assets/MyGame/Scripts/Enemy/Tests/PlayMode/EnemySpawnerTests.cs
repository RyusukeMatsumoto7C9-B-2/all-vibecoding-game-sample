using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace MyGame.Enemy.Spawn.Tests
{
    [Description("エネミースポーナーの動作をテストする")]
    public class EnemySpawnerTests
    {

        [UnityTest]
        [Description("初期化時にアクティブな敵の数がゼロであることを検証")]
        public IEnumerator Test_ActiveEnemyCount_WhenInitialized_ReturnsZero()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);
            
            var spawner = GetComponent();

            Assert.Zero(spawner.ActiveEnemyCount);
        }
        
        
        [UnityTest]
        [Description("有効なレベルでスポーン実行時にアクティブな敵の数が増加することを検証")]
        public IEnumerator Test_SpawnEnemiesForLevel_WithValidLevel_IncreasesActiveEnemyCount()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);
            
            var spawner = GetComponent();

            spawner.SpawnEnemiesForLevel(1);
            
            Assert.NotZero(spawner.ActiveEnemyCount);
        }

        
        [UnityTest]
        [Description("敵クリア実行後にアクティブな敵の数がゼロにリセットされることを検証")]
        public IEnumerator Test_ClearAllEnemies_AfterSpawning_ResetsActiveEnemyCountToZero()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);
            
            var spawner = GetComponent();

            spawner.SpawnEnemiesForLevel(1);
            Assert.NotZero(spawner.ActiveEnemyCount);

            spawner.ClearAllEnemies();
            Assert.Zero(spawner.ActiveEnemyCount);
        }

        
        [UnityTest]
        [Description("レベル1で5体の敵がスポーンされることを検証")]
        public IEnumerator Test_SpawnEnemiesForLevel_WithLevel1_Spawns5Enemies()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);

            var spawner = GetComponent();

            spawner.SpawnEnemiesForLevel(1);
            Assert.AreEqual(5, spawner.ActiveEnemyCount);
        }

        
        [UnityTest]
        [Description("レベル5で5体の敵がスポーンされることを検証")]
        public IEnumerator Test_SpawnEnemiesForLevel_WithLevel5_Spawns5Enemies()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);

            var spawner = GetComponent();

            spawner.SpawnEnemiesForLevel(5);
            Assert.AreEqual(5, spawner.ActiveEnemyCount);
        }

        
        [UnityTest]
        [Description("レベル6で6体の敵がスポーンされることを検証")]
        public IEnumerator Test_SpawnEnemiesForLevel_WithLevel6_Spawns6Enemies()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);

            var spawner = GetComponent();

            spawner.SpawnEnemiesForLevel(6);
            Assert.AreEqual(6, spawner.ActiveEnemyCount);
        }

        
        [UnityTest]
        [Description("レベル26で10体（上限）の敵がスポーンされることを検証")]
        public IEnumerator Test_SpawnEnemiesForLevel_WithLevel26_Spawns10Enemies()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);

            var spawner = GetComponent();

            spawner.SpawnEnemiesForLevel(26);
            Assert.AreEqual(10, spawner.ActiveEnemyCount);
        }

        [UnityTest]
        [Description("負のレベル値でエラーログが出力され敵がスポーンされないことを検証")]
        public IEnumerator Test_SpawnEnemiesForLevel_WithNegativeLevel_LogsErrorAndDoesNotSpawn()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);

            var spawner = GetComponent();

            LogAssert.Expect(LogType.Error, "設定するレベルは0以上の整数です、エネミーの生成を実行しません.");
            spawner.SpawnEnemiesForLevel(-1);
            
            Assert.Zero(spawner.ActiveEnemyCount);
        }


        [UnityTest]
        [Description("敵が存在しない状態での敵クリア実行が安全に動作することを検証")]
        public IEnumerator Test_ClearAllEnemies_WhenNoEnemiesExist_ExecutesSafely()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);

            var spawner = GetComponent();
            
            Assert.Zero(spawner.ActiveEnemyCount);
            spawner.ClearAllEnemies();
            LogAssert.NoUnexpectedReceived();
        }


        private EnemySpawner GetComponent()
        {
            return GameObject.FindFirstObjectByType<EnemySpawner>();
        }

        private AsyncOperation LoadScene()
        {
            return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/MyGame/Scripts/Enemy/Tests/PlayMode/TestScene/EnemySpawnerTestScene.unity", new LoadSceneParameters(LoadSceneMode.Single));
        }
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace MyGame.Enemy.Spawn.Tests
{
    public class EnemySpawnerTests
    {

        [UnityTest]
        public IEnumerator Test_1()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);
            
            var spawner = GetComponent();
            
            Assert.Zero(spawner.ActiveEnemyCount);
        }
        
        
        [UnityTest]
        public IEnumerator Test_2()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);
            
            var spawner = GetComponent();

            spawner.SpawnEnemiesForLevel(1);
            
            Assert.NotZero(spawner.ActiveEnemyCount);
        }

        
        [UnityTest]
        public IEnumerator Test_3()
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
        [TestCase(1, 5)]
        [TestCase(5, 5)]
        [TestCase(6, 6)]
        [TestCase(26, 10)]
        public IEnumerator Test_4(int level, int expectedEnemyCount)
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);

            var spawner = GetComponent();

            spawner.SpawnEnemiesForLevel(level);
            Assert.AreEqual(expectedEnemyCount, spawner.ActiveEnemyCount);
        }

        [UnityTest]
        public IEnumerator Test_5()
        {
            yield return LoadScene();
            yield return new WaitForSeconds(1f);

            var spawner = GetComponent();

            LogAssert.Expect(LogType.Error, "設定するレベルは0以上の整数です、エネミーの生成を実行しません.");
            spawner.SpawnEnemiesForLevel(-1);
            
            Assert.Zero(spawner.ActiveEnemyCount);
        }


        [UnityTest]
        public IEnumerator Test_6()
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

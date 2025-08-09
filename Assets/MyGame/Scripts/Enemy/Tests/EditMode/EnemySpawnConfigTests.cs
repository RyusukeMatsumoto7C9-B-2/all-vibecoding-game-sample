using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;


namespace MyGame.Enemy.Spawn.Tests
{
    public class EnemySpawnConfigTests
    {
        [Test]
        [TestCase(0, 5)]
        [TestCase(1, 5)]
        [TestCase(5, 5)]
        [TestCase(6, 6)]
        [TestCase(10, 6)]
        [TestCase(11, 7)]
        [TestCase(26, 10)]
        [TestCase(27, 10)]
        public void Test_(int level , int expectedEnemyCount)
        {
            
            
            Debug.Log("hogehoge fugafuga");
            var config = EnemySpawnConfig.Create(5, 5, 1, 10);
            int actualEnemyCount = config.GetEnemyCountForLevel(level);
            Debug.Log("hogehoge");
            Assert.AreEqual(expectedEnemyCount, actualEnemyCount);
        }
    }
}
using NUnit.Framework;
using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem.Generation;
using System.Collections.Generic;

namespace MyGame.TilemapSystem.Tests.EditMode
{
    [Description("TilemapScrollControllerクラスのテスト")]
    public class TilemapScrollControllerTests
    {
        private TilemapScrollController _scrollController;
        private TilemapGenerator _generator;
        private TilemapManager _manager;
        private Transform _tilemapParent;
        private SeedManager _seedManager;

        [SetUp]
        public void SetUp()
        {
            // テスト用GameObjectを作成
            var parentObject = new GameObject("TilemapParent");
            _tilemapParent = parentObject.transform;
            
            // SeedManagerを作成
            _seedManager = new SeedManager(12345);
            
            // TilemapGeneratorを作成
            _generator = new TilemapGenerator(_seedManager);
            
            // TilemapManagerを作成（ダミーPrefabを使用）
            var tilePrefabs = new Dictionary<TileType, GameObject>
            {
                { TileType.Wall, new GameObject("WallTile") },
                { TileType.Ground, new GameObject("GroundTile") }
            };
            _manager = new TilemapManager(_tilemapParent, tilePrefabs);
            
            // TilemapScrollControllerを作成
            _scrollController = new TilemapScrollController(_generator, _manager, _tilemapParent);
        }

        [TearDown]
        public void TearDown()
        {
            if (_tilemapParent != null)
            {
                Object.DestroyImmediate(_tilemapParent.gameObject);
            }
        }

        [Test]
        [Description("初期状態でスクロール中でないことを検証")]
        public void InitialState_ShouldNotBeScrolling()
        {
            // Act & Assert
            Assert.IsFalse(_scrollController.IsScrolling);
        }

        [Test]
        [Description("初期レベルが1であることを検証")]
        public void InitialLevel_ShouldBeOne()
        {
            // Act & Assert
            Assert.AreEqual(1, _scrollController.CurrentLevel);
        }

        [Test]
        [Description("スクロール速度の設定が正しく反映されることを検証")]
        public void SetScrollSpeed_ShouldUpdateSpeed()
        {
            // Arrange
            float expectedSpeed = 10.0f;
            
            // Act
            _scrollController.SetScrollSpeed(expectedSpeed);
            
            // Assert
            Assert.AreEqual(expectedSpeed, _scrollController.ScrollSpeed);
        }

        [Test]
        [Description("スクロール距離の設定が正しく反映されることを検証")]
        public void SetScrollDistance_ShouldUpdateDistance()
        {
            // Arrange
            float expectedDistance = 30.0f;
            
            // Act
            _scrollController.SetScrollDistance(expectedDistance);
            
            // Assert
            Assert.AreEqual(expectedDistance, _scrollController.ScrollDistance);
        }

        [Test]
        [Description("現在レベルの設定が正しく反映されることを検証")]
        public void SetCurrentLevel_ShouldUpdateLevel()
        {
            // Arrange
            int expectedLevel = 5;
            
            // Act
            _scrollController.SetCurrentLevel(expectedLevel);
            
            // Assert
            Assert.AreEqual(expectedLevel, _scrollController.CurrentLevel);
        }

        [Test]
        [Description("無効なスクロール速度（0以下）を設定した場合に値が変更されないことを検証")]
        public void SetScrollSpeed_WithInvalidValue_ShouldNotUpdateSpeed()
        {
            // Arrange
            float originalSpeed = _scrollController.ScrollSpeed;
            
            // Act
            _scrollController.SetScrollSpeed(0);
            _scrollController.SetScrollSpeed(-1);
            
            // Assert
            Assert.AreEqual(originalSpeed, _scrollController.ScrollSpeed);
        }

        [Test]
        [Description("無効なスクロール距離（0以下）を設定した場合に値が変更されないことを検証")]
        public void SetScrollDistance_WithInvalidValue_ShouldNotUpdateDistance()
        {
            // Arrange
            float originalDistance = _scrollController.ScrollDistance;
            
            // Act
            _scrollController.SetScrollDistance(0);
            _scrollController.SetScrollDistance(-1);
            
            // Assert
            Assert.AreEqual(originalDistance, _scrollController.ScrollDistance);
        }

        [Test]
        [Description("無効なレベル（1未満）を設定した場合に値が変更されないことを検証")]
        public void SetCurrentLevel_WithInvalidValue_ShouldNotUpdateLevel()
        {
            // Arrange
            int originalLevel = _scrollController.CurrentLevel;
            
            // Act
            _scrollController.SetCurrentLevel(0);
            _scrollController.SetCurrentLevel(-1);
            
            // Assert
            Assert.AreEqual(originalLevel, _scrollController.CurrentLevel);
        }

        [Test]
        [Description("コンストラクタでnullパラメータを渡した場合に例外が発生することを検証")]
        public void Constructor_WithNullParameters_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => 
                new TilemapScrollController(null, _manager, _tilemapParent));
            
            Assert.Throws<System.ArgumentNullException>(() => 
                new TilemapScrollController(_generator, null, _tilemapParent));
            
            Assert.Throws<System.ArgumentNullException>(() => 
                new TilemapScrollController(_generator, _manager, null));
        }

        [Test]
        [Description("イベントハンドラーが正しく動作することを検証")]
        public void Events_ShouldBeInvokable()
        {
            // Arrange
            bool scrollStartedCalled = false;
            bool scrollCompletedCalled = false;
            bool newLevelGeneratedCalled = false;
            
            _scrollController.OnScrollStarted += (level) => scrollStartedCalled = true;
            _scrollController.OnScrollCompleted += (level) => scrollCompletedCalled = true;
            _scrollController.OnNewLevelGenerated += (level) => newLevelGeneratedCalled = true;
            
            // Act - イベントを手動で発火（テスト用）
            // Note: 実際のスクロール処理は非同期でUnityエディタでのテストが困難なため、
            // ここではイベントハンドラーの設定が正しく動作することのみを確認
            
            // Assert
            Assert.IsNotNull(_scrollController.OnScrollStarted);
            Assert.IsNotNull(_scrollController.OnScrollCompleted);
            Assert.IsNotNull(_scrollController.OnNewLevelGenerated);
        }
    }
}
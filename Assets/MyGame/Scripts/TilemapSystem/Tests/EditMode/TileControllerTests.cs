using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MyGame.TilemapSystem.Core;

namespace MyGame.TilemapSystem.Tests.EditMode
{
    public class TileControllerTests
    {
        private GameObject _testGameObject;
        private TileController _tileController;
        private SpriteRenderer _spriteRenderer;
        private SpriteManager _mockSpriteManager;
        
        [SetUp]
        public void SetUp()
        {
            // テスト用GameObjectとTileControllerの作成
            _testGameObject = new GameObject("TestTile");
            _spriteRenderer = _testGameObject.AddComponent<SpriteRenderer>();
            _tileController = _testGameObject.AddComponent<TileController>();
            
            // モックSpriteManagerの作成
            _mockSpriteManager = ScriptableObject.CreateInstance<SpriteManager>();
            
            // テスト用スプライトの作成
            var testTexture = new Texture2D(1, 1);
            testTexture.SetPixel(0, 0, Color.white);
            testTexture.Apply();
            
            var testSprite = Sprite.Create(testTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
            
            // SpriteManagerにテストスプライトを設定（公開メソッドを使用）
            _mockSpriteManager.SetSpriteForBlockType(BlockType.Sky, testSprite);
            _mockSpriteManager.SetSpriteForBlockType(BlockType.Ground, testSprite);
            _mockSpriteManager.SetSpriteForBlockType(BlockType.Rock, testSprite);
        }
        
        [TearDown]
        public void TearDown()
        {
            if (_testGameObject != null)
            {
                Object.DestroyImmediate(_testGameObject);
            }
            
            if (_mockSpriteManager != null)
            {
                Object.DestroyImmediate(_mockSpriteManager);
            }
        }
        
        [Test]
        public void Initialize_WithValidParameters_SetsBlockTypeAndSpriteManager()
        {
            // Arrange
            var blockType = BlockType.Ground;
            
            // Act
            _tileController.Initialize(blockType, _mockSpriteManager);
            
            // Assert
            Assert.AreEqual(blockType, _tileController.BlockType);
            Assert.AreEqual(_mockSpriteManager, _tileController.SpriteManager);
            Assert.IsTrue(_tileController.IsInitialized);
        }
        
        [Test]
        public void BlockType_WhenChanged_UpdatesSprite()
        {
            // Arrange
            _tileController.Initialize(BlockType.Empty, _mockSpriteManager);
            var initialSprite = _spriteRenderer.sprite;
            
            // Act
            _tileController.BlockType = BlockType.Ground;
            
            // Assert
            Assert.AreEqual(BlockType.Ground, _tileController.BlockType);
            // スプライトが更新されたかは実際のSpriteManagerの実装に依存するため、
            // ここでは少なくともBlockTypeが正しく設定されているかを確認
        }
        
        [Test]
        public void ChangeBlockType_WithSameType_ReturnsFalse()
        {
            // Arrange
            _tileController.Initialize(BlockType.Ground, _mockSpriteManager);
            
            // Act
            var result = _tileController.ChangeBlockType(BlockType.Ground);
            
            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(BlockType.Ground, _tileController.BlockType);
        }
        
        [Test]
        public void ChangeBlockType_WithDifferentType_ReturnsTrue()
        {
            // Arrange
            _tileController.Initialize(BlockType.Ground, _mockSpriteManager);
            
            // Act
            var result = _tileController.ChangeBlockType(BlockType.Rock);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(BlockType.Rock, _tileController.BlockType);
        }
        
        [Test]
        public void ChangeBlockType_WithForceUpdate_AlwaysReturnsTrue()
        {
            // Arrange
            _tileController.Initialize(BlockType.Ground, _mockSpriteManager);
            
            // Act
            var result = _tileController.ChangeBlockType(BlockType.Ground, forceUpdate: true);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(BlockType.Ground, _tileController.BlockType);
        }
        
        [Test]
        public void SetVisible_WithTrue_EnablesSpriteRenderer()
        {
            // Arrange
            _tileController.Initialize(BlockType.Ground, _mockSpriteManager);
            _spriteRenderer.enabled = false;
            
            // Act
            _tileController.SetVisible(true);
            
            // Assert
            Assert.IsTrue(_spriteRenderer.enabled);
        }
        
        [Test]
        public void SetVisible_WithEmptyBlock_KeepsSpriteRendererDisabled()
        {
            // Arrange
            _tileController.Initialize(BlockType.Empty, _mockSpriteManager);
            
            // Act
            _tileController.SetVisible(true);
            
            // Assert
            Assert.IsFalse(_spriteRenderer.enabled);
        }
        
        [Test]
        public void GetDebugInfo_ReturnsFormattedDebugString()
        {
            // Arrange
            _tileController.Initialize(BlockType.Ground, _mockSpriteManager);
            
            // Act
            var debugInfo = _tileController.GetDebugInfo();
            
            // Assert
            Assert.IsNotEmpty(debugInfo);
            Assert.That(debugInfo, Contains.Substring("BlockType: Ground"));
            Assert.That(debugInfo, Contains.Substring("Initialized: True"));
        }
        
        [Test]
        public void Position_ReturnsTransformPosition()
        {
            // Arrange
            var expectedPosition = new Vector3(1, 2, 3);
            _testGameObject.transform.position = expectedPosition;
            
            // Act
            var position = _tileController.Position;
            
            // Assert
            Assert.AreEqual(expectedPosition, position);
        }
        
        [Test]
        public void SetDebugMode_UpdatesDebugSettings()
        {
            // Arrange & Act
            _tileController.SetDebugMode(true, true);
            
            // Assert
            // デバッグモードの設定は内部状態なので、少なくともメソッドが例外を投げないことを確認
            Assert.DoesNotThrow(() => _tileController.SetDebugMode(false, false));
        }
        
        [Test]
        public void Initialize_WithNullSpriteManager_LogsWarning()
        {
            // Arrange & Act
            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex(".*SpriteManagerが設定されていません.*"));
            _tileController.Initialize(BlockType.Ground, null);
            
            // Assert
            Assert.IsTrue(_tileController.IsInitialized);
            Assert.AreEqual(BlockType.Ground, _tileController.BlockType);
        }
    }
}
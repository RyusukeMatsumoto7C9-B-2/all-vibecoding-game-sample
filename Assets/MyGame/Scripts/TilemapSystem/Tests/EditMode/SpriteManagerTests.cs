using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MyGame.TilemapSystem.Core;

namespace MyGame.TilemapSystem.Tests.EditMode
{
    public class SpriteManagerTests
    {
        private SpriteManager _spriteManager;
        private Sprite _testSprite;
        
        [SetUp]
        public void SetUp()
        {
            _spriteManager = ScriptableObject.CreateInstance<SpriteManager>();
            
            // テスト用スプライトの作成
            var testTexture = new Texture2D(1, 1);
            testTexture.SetPixel(0, 0, Color.white);
            testTexture.Apply();
            _testSprite = Sprite.Create(testTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
        }
        
        [TearDown]
        public void TearDown()
        {
            if (_spriteManager != null)
            {
                Object.DestroyImmediate(_spriteManager);
            }
            
            if (_testSprite != null)
            {
                Object.DestroyImmediate(_testSprite.texture);
                Object.DestroyImmediate(_testSprite);
            }
        }
        
        [Test]
        public void GetSpriteForBlockType_WithUnsetSprite_ReturnsNull()
        {
            // Act
            var sprite = _spriteManager.GetSpriteForBlockType(BlockType.Ground);
            
            // Assert
            Assert.IsNull(sprite);
        }
        
        [Test]
        public void GetSpriteForBlockType_WithInvalidBlockType_ReturnsNullAndLogsWarning()
        {
            // Arrange
            var invalidBlockType = (BlockType)999;
            
            // Act & Assert
            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex(".*に対応するSpriteが見つかりません.*"));
            var sprite = _spriteManager.GetSpriteForBlockType(invalidBlockType);
            Assert.IsNull(sprite);
        }
        
        [Test]
        public void SetSpriteForBlockType_WithValidBlockType_StoresSprite()
        {
            // Act
            _spriteManager.SetSpriteForBlockType(BlockType.Ground, _testSprite);
            
            // Assert
            var retrievedSprite = _spriteManager.GetSpriteForBlockType(BlockType.Ground);
            Assert.AreEqual(_testSprite, retrievedSprite);
        }
        
        [Test]
        public void SetSpriteForBlockType_WithInvalidBlockType_LogsWarning()
        {
            // Arrange
            var invalidBlockType = (BlockType)999;
            
            // Act & Assert
            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex(".*の設定項目が見つかりません.*"));
            _spriteManager.SetSpriteForBlockType(invalidBlockType, _testSprite);
        }
        
        [Test]
        public void HasSpriteForBlockType_WithSetSprite_ReturnsTrue()
        {
            // Arrange
            _spriteManager.SetSpriteForBlockType(BlockType.Ground, _testSprite);
            
            // Act
            var hasSprite = _spriteManager.HasSpriteForBlockType(BlockType.Ground);
            
            // Assert
            Assert.IsTrue(hasSprite);
        }
        
        [Test]
        public void HasSpriteForBlockType_WithUnsetSprite_ReturnsFalse()
        {
            // Act
            var hasSprite = _spriteManager.HasSpriteForBlockType(BlockType.Ground);
            
            // Assert
            Assert.IsFalse(hasSprite);
        }
        
        [Test]
        public void HasSpriteForBlockType_WithInvalidBlockType_ReturnsFalse()
        {
            // Arrange
            var invalidBlockType = (BlockType)999;
            
            // Act
            var hasSprite = _spriteManager.HasSpriteForBlockType(invalidBlockType);
            
            // Assert
            Assert.IsFalse(hasSprite);
        }
        
        [Test]
        public void GetAllBlockTypeSprites_ReturnsAllConfiguredTypes()
        {
            // Act
            var allSprites = _spriteManager.GetAllBlockTypeSprites();
            
            // Assert
            Assert.IsNotNull(allSprites);
            Assert.AreEqual(5, allSprites.Length); // Sky, Empty, Ground, Rock, Treasure
            
            // 全てのBlockTypeが含まれているかチェック
            var blockTypes = new[] { BlockType.Sky, BlockType.Empty, BlockType.Ground, BlockType.Rock, BlockType.Treasure };
            foreach (var expectedType in blockTypes)
            {
                bool found = false;
                foreach (var sprite in allSprites)
                {
                    if (sprite.blockType == expectedType)
                    {
                        found = true;
                        break;
                    }
                }
                Assert.IsTrue(found, $"BlockType {expectedType} was not found in GetAllBlockTypeSprites result");
            }
        }
        
        [Test]
        public void SpriteSwitch_Integration_WorksCorrectly()
        {
            // Arrange
            var groundSprite = _testSprite;
            var rockTexture = new Texture2D(1, 1);
            rockTexture.SetPixel(0, 0, Color.gray);
            rockTexture.Apply();
            var rockSprite = Sprite.Create(rockTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
            
            // Act - Set different sprites for different block types
            _spriteManager.SetSpriteForBlockType(BlockType.Ground, groundSprite);
            _spriteManager.SetSpriteForBlockType(BlockType.Rock, rockSprite);
            
            // Assert - Verify sprites are correctly stored and retrieved
            Assert.AreEqual(groundSprite, _spriteManager.GetSpriteForBlockType(BlockType.Ground));
            Assert.AreEqual(rockSprite, _spriteManager.GetSpriteForBlockType(BlockType.Rock));
            Assert.IsNull(_spriteManager.GetSpriteForBlockType(BlockType.Empty));
            
            Assert.IsTrue(_spriteManager.HasSpriteForBlockType(BlockType.Ground));
            Assert.IsTrue(_spriteManager.HasSpriteForBlockType(BlockType.Rock));
            Assert.IsFalse(_spriteManager.HasSpriteForBlockType(BlockType.Empty));
            
            // Cleanup
            Object.DestroyImmediate(rockTexture);
            Object.DestroyImmediate(rockSprite);
        }
    }
}
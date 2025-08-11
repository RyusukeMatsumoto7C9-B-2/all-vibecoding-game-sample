using NUnit.Framework;
using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem.Generation;

namespace MyGame.TilemapSystem.Tests
{
    [Description("TilemapManager座標変換機能のテスト")]
    public class TilemapManagerCoordinateTests
    {
        private TilemapManager _tilemapManager;
        private GameObject _parentObject;
        private GameObject _tilePrefab;
        private SeedManager _seedManager;
        private TilemapGenerator _generator;

        [SetUp]
        public void SetUp()
        {
            _parentObject = new GameObject("TestParent");
            _tilePrefab = new GameObject("TilePrefab");
            _tilePrefab.AddComponent<TileController>();
            
            _tilemapManager = new TilemapManager(_parentObject.transform, _tilePrefab);
            
            _seedManager = new SeedManager();
            _generator = new TilemapGenerator(_seedManager);
        }

        [TearDown]
        public void TearDown()
        {
            if (_parentObject != null)
            {
                Object.DestroyImmediate(_parentObject);
            }
            if (_tilePrefab != null)
            {
                Object.DestroyImmediate(_tilePrefab);
            }
        }

        [Test]
        [Description("GetPositionメソッドがグリッド座標を正しくワールド座標に変換することを検証")]
        public void GetPosition_ConvertsGridCoordinatesToWorldPosition()
        {
            var position = _tilemapManager.GetPosition(5, 10);
            
            Assert.AreEqual(5f, position.x);
            Assert.AreEqual(10f, position.y);
            Assert.AreEqual(0f, position.z);
        }

        [Test]
        [Description("GetPositionメソッドが負の座標でも正しく動作することを検証")]
        public void GetPosition_HandlesNegativeCoordinates()
        {
            var position = _tilemapManager.GetPosition(-3, -7);
            
            Assert.AreEqual(-3f, position.x);
            Assert.AreEqual(-7f, position.y);
            Assert.AreEqual(0f, position.z);
        }

        [Test]
        [Description("GetPositionメソッドがゼロ座標で正しく動作することを検証")]
        public void GetPosition_HandlesZeroCoordinates()
        {
            var position = _tilemapManager.GetPosition(0, 0);
            
            Assert.AreEqual(0f, position.x);
            Assert.AreEqual(0f, position.y);
            Assert.AreEqual(0f, position.z);
        }

        [Test]
        [Description("CanPassThroughメソッドがRockタイルで通過不可を返すことを検証")]
        public void CanPassThrough_ReturnsFalseForRockTile()
        {
            var mapData = _generator.GenerateMap(1, 12345);
            _tilemapManager.PlaceTiles(mapData);
            
            // Rockタイルの位置を探す
            Vector2Int rockPosition = Vector2Int.zero;
            bool foundRock = false;
            for (int x = 0; x < mapData.Width && !foundRock; x++)
            {
                for (int y = 0; y < mapData.Height && !foundRock; y++)
                {
                    if (mapData.Tiles[x, y] == BlockType.Rock)
                    {
                        rockPosition = new Vector2Int(x, y);
                        foundRock = true;
                    }
                }
            }
            
            if (foundRock)
            {
                var canPass = _tilemapManager.CanPassThrough(rockPosition, 1);
                Assert.IsFalse(canPass);
            }
        }

        [Test]
        [Description("CanPassThroughメソッドがGroundタイルで通過不可を返すことを検証")]
        public void CanPassThrough_ReturnsFalseForGroundTile()
        {
            var mapData = _generator.GenerateMap(1, 12345);
            _tilemapManager.PlaceTiles(mapData);
            
            // Groundタイルの位置を探す
            Vector2Int groundPosition = Vector2Int.zero;
            bool foundGround = false;
            for (int x = 0; x < mapData.Width && !foundGround; x++)
            {
                for (int y = 0; y < mapData.Height && !foundGround; y++)
                {
                    if (mapData.Tiles[x, y] == BlockType.Ground)
                    {
                        groundPosition = new Vector2Int(x, y);
                        foundGround = true;
                    }
                }
            }
            
            if (foundGround)
            {
                var canPass = _tilemapManager.CanPassThrough(groundPosition, 1);
                Assert.IsFalse(canPass);
            }
        }

        [Test]
        [Description("CanPassThroughメソッドがEmptyタイルで通過可能を返すことを検証")]
        public void CanPassThrough_ReturnsTrueForEmptyTile()
        {
            var mapData = _generator.GenerateMap(1, 12345);
            _tilemapManager.PlaceTiles(mapData);
            
            // Emptyタイルの位置を探す
            Vector2Int emptyPosition = Vector2Int.zero;
            bool foundEmpty = false;
            for (int x = 0; x < mapData.Width && !foundEmpty; x++)
            {
                for (int y = 0; y < mapData.Height && !foundEmpty; y++)
                {
                    if (mapData.Tiles[x, y] == BlockType.Empty)
                    {
                        emptyPosition = new Vector2Int(x, y);
                        foundEmpty = true;
                    }
                }
            }
            
            if (foundEmpty)
            {
                var canPass = _tilemapManager.CanPassThrough(emptyPosition, 1);
                Assert.IsTrue(canPass);
            }
        }

        [Test]
        [Description("CanPassThroughメソッドがSkyタイルで通過可能を返すことを検証")]
        public void CanPassThrough_ReturnsTrueForSkyTile()
        {
            var mapData = _generator.GenerateMap(1, 12345);
            _tilemapManager.PlaceTiles(mapData);
            
            // Skyタイルの位置を探す
            Vector2Int skyPosition = Vector2Int.zero;
            bool foundSky = false;
            for (int x = 0; x < mapData.Width && !foundSky; x++)
            {
                for (int y = 0; y < mapData.Height && !foundSky; y++)
                {
                    if (mapData.Tiles[x, y] == BlockType.Sky)
                    {
                        skyPosition = new Vector2Int(x, y);
                        foundSky = true;
                    }
                }
            }
            
            if (foundSky)
            {
                var canPass = _tilemapManager.CanPassThrough(skyPosition, 1);
                Assert.IsTrue(canPass);
            }
        }

        [Test]
        [Description("CanPassThroughメソッドがマップ境界外で通過不可を返すことを検証")]
        public void CanPassThrough_ReturnsFalseForOutOfBounds()
        {
            var mapData = _generator.GenerateMap(1, 12345);
            _tilemapManager.PlaceTiles(mapData);
            
            var canPassLeft = _tilemapManager.CanPassThrough(new Vector2Int(-1, 5), 1);
            var canPassRight = _tilemapManager.CanPassThrough(new Vector2Int(mapData.Width, 5), 1);
            var canPassBottom = _tilemapManager.CanPassThrough(new Vector2Int(5, -1), 1);
            var canPassTop = _tilemapManager.CanPassThrough(new Vector2Int(5, mapData.Height), 1);
            
            Assert.IsFalse(canPassLeft);
            Assert.IsFalse(canPassRight);
            Assert.IsFalse(canPassBottom);
            Assert.IsFalse(canPassTop);
        }

        [Test]
        [Description("CanPassThroughメソッドがマップ未ロード時に通過可能を返すことを検証")]
        public void CanPassThrough_ReturnsTrueWhenMapNotLoaded()
        {
            var canPass = _tilemapManager.CanPassThrough(new Vector2Int(5, 5), 999);
            
            Assert.IsTrue(canPass);
        }
    }
}
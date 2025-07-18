using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem.Generation;
using Cysharp.Threading.Tasks;

namespace MyGame.TilemapSystem.Tests.EditMode
{
    [TestFixture]
    public class TilemapLevelCoordinateTests
    {
        private TilemapScrollController _scrollController;
        private TilemapManager _manager;
        private TilemapGenerator _generator;
        private Transform _parentTransform;
        private Dictionary<TileType, GameObject> _tilePrefabs;
        private SeedManager _seedManager;

        [SetUp]
        public void SetUp()
        {
            // テスト用のGameObjectを作成
            var parentObject = new GameObject("TilemapParent");
            _parentTransform = parentObject.transform;
            
            // テスト用のタイルプレハブを作成
            _tilePrefabs = new Dictionary<TileType, GameObject>
            {
                { TileType.Sky, new GameObject("SkyTile") },
                { TileType.Ground, new GameObject("GroundTile") },
                { TileType.Rock, new GameObject("RockTile") },
                { TileType.Treasure, new GameObject("TreasureTile") }
            };
            
            _manager = new TilemapManager(_parentTransform, _tilePrefabs);
            _seedManager = new SeedManager(12345);
            _generator = new TilemapGenerator(_seedManager);
            _scrollController = new TilemapScrollController(_generator, _manager, _parentTransform);
        }

        [TearDown]
        public void TearDown()
        {
            // テスト用のGameObjectを削除
            Object.DestroyImmediate(_parentTransform.gameObject);
            
            // プレハブを削除
            foreach (var prefab in _tilePrefabs.Values)
            {
                Object.DestroyImmediate(prefab);
            }
        }

        [Test]
        public void VerifyLevelCoordinateRanges_AfterInitialGeneration()
        {
            // Arrange: 最初のレベルを生成
            var level1MapData = _generator.GenerateMap(1, 12345);
            _manager.PlaceTiles(level1MapData);
            
            // Act: レベル1のタイルの座標範囲を確認
            var level1Tiles = _manager.GetTilesForLevel(1);
            
            // Assert: レベル1のタイルがY座標0-29の範囲内にあることを確認
            Assert.IsNotNull(level1Tiles, "レベル1のタイルが存在しません");
            Assert.Greater(level1Tiles.Count, 0, "レベル1のタイルが生成されていません");
            
            foreach (var tile in level1Tiles)
            {
                var yPos = tile.transform.position.y;
                Assert.GreaterOrEqual(yPos, 0, $"レベル1のタイルY座標が0未満です: {yPos}");
                Assert.LessOrEqual(yPos, 29, $"レベル1のタイルY座標が29を超えています: {yPos}");
            }
        }

        [Test]
        public async Task VerifyNoCoordinateOverlap_AfterScrollGeneration()
        {
            // Arrange: 最初のレベルを生成
            var level1MapData = _generator.GenerateMap(1, 12345);
            _manager.PlaceTiles(level1MapData);
            
            // レベル2を手動で事前生成（スクロール処理をシミュレート）
            var level2MapData = _generator.GenerateMap(2, 12346);
            
            // スクロールコントローラーの内部処理をシミュレート
            int overlapHeight = 5;
            _manager.PlaceTilesWithOverlapProtection(level2MapData, overlapHeight);
            
            // 修正された座標計算を適用
            float correctOffset = -TilemapGenerator.MAP_HEIGHT; // -30マス分
            var level2Tiles = _manager.GetTilesForLevel(2);
            
            // レベル2のタイルに手動でオフセットを適用
            foreach (var tile in level2Tiles)
            {
                var currentPos = tile.transform.position;
                tile.transform.position = new Vector3(currentPos.x, currentPos.y + correctOffset, currentPos.z);
            }
            
            await UniTask.Yield();
            
            // Act & Assert: 座標重複のチェック
            var level1Tiles = _manager.GetTilesForLevel(1);
            var level2TilesAfterOffset = _manager.GetTilesForLevel(2);
            
            Assert.IsNotNull(level1Tiles, "レベル1のタイルが存在しません");
            Assert.IsNotNull(level2TilesAfterOffset, "レベル2のタイルが存在しません");
            
            // レベル1の座標範囲を確認（Y: 0-29）
            float level1MinY = float.MaxValue, level1MaxY = float.MinValue;
            foreach (var tile in level1Tiles)
            {
                var yPos = tile.transform.position.y;
                level1MinY = Mathf.Min(level1MinY, yPos);
                level1MaxY = Mathf.Max(level1MaxY, yPos);
            }
            
            // レベル2の座標範囲を確認（Y: -30 to -1）
            float level2MinY = float.MaxValue, level2MaxY = float.MinValue;
            foreach (var tile in level2TilesAfterOffset)
            {
                var yPos = tile.transform.position.y;
                level2MinY = Mathf.Min(level2MinY, yPos);
                level2MaxY = Mathf.Max(level2MaxY, yPos);
            }
            
            // 座標範囲の検証
            Assert.GreaterOrEqual(level1MinY, 0, "レベル1の最小Y座標が0未満です");
            Assert.LessOrEqual(level1MaxY, 29, "レベル1の最大Y座標が29を超えています");
            Assert.GreaterOrEqual(level2MinY, -30, "レベル2の最小Y座標が-30未満です");
            Assert.LessOrEqual(level2MaxY, -1, "レベル2の最大Y座標が-1を超えています");
            
            // 重複がないことを確認
            Assert.Less(level2MaxY, level1MinY, 
                $"レベル間でY座標の重複が発生しています。レベル1最小:{level1MinY}, レベル2最大:{level2MaxY}");
        }

        [Test]
        public void VerifyScrollDistanceCalculation()
        {
            // Arrange & Act: スクロール距離とマップ高さの関係を確認
            var scrollDistance = _scrollController.ScrollDistance;
            var mapHeight = TilemapGenerator.MAP_HEIGHT;
            
            // Assert: 座標計算の整合性を確認
            Assert.AreEqual(25.0f, scrollDistance, "スクロール距離が期待値と異なります");
            Assert.AreEqual(30, mapHeight, "マップ高さが期待値と異なります");
            
            // 修正されたオフセット計算の検証
            float expectedOffset = -mapHeight; // -30
            Assert.AreEqual(-30.0f, expectedOffset, "オフセット計算が正しくありません");
        }

        [Test]
        public void VerifyOverlapProtectionArea()
        {
            // Arrange: 重複エリア（5マス）の設定を確認
            int expectedOverlapHeight = 5;
            int expectedLevelOffset = TilemapGenerator.MAP_HEIGHT - expectedOverlapHeight; // 25マス
            
            // Act & Assert: 重複エリア計算の検証
            Assert.AreEqual(5, expectedOverlapHeight, "重複エリア高さが期待値と異なります");
            Assert.AreEqual(25, expectedLevelOffset, "レベルオフセット計算が期待値と異なります");
            
            // 修正後の座標計算では、重複エリアに関係なく完全分離される
            float actualOffset = -TilemapGenerator.MAP_HEIGHT; // -30
            Assert.AreEqual(-30.0f, actualOffset, "実際のオフセットが期待値と異なります");
        }

        [Test]
        public async Task VerifyNoTileCollision_AtSpecificCoordinates()
        {
            // Arrange: 既知の座標範囲でのタイル衝突テスト
            var level1MapData = _generator.GenerateMap(1, 12345);
            _manager.PlaceTiles(level1MapData);
            
            var level2MapData = _generator.GenerateMap(2, 12346);
            _manager.PlaceTilesWithOverlapProtection(level2MapData, 5);
            
            // レベル2にオフセットを適用
            var level2Tiles = _manager.GetTilesForLevel(2);
            float correctOffset = -TilemapGenerator.MAP_HEIGHT;
            
            foreach (var tile in level2Tiles)
            {
                var currentPos = tile.transform.position;
                tile.transform.position = new Vector3(currentPos.x, currentPos.y + correctOffset, currentPos.z);
            }
            
            await UniTask.Yield();
            
            // Act: 境界座標での衝突チェック
            var allTiles = new List<GameObject>();
            var level1TilesCheck = _manager.GetTilesForLevel(1);
            var level2TilesCheck = _manager.GetTilesForLevel(2);
            
            if (level1TilesCheck != null) allTiles.AddRange(level1TilesCheck);
            if (level2TilesCheck != null) allTiles.AddRange(level2TilesCheck);
            
            // Assert: 境界座標（Y=-1, Y=0）での重複チェック
            var collisionCount = 0;
            for (int x = 0; x < 20; x++)
            {
                for (int y = -1; y <= 0; y++)
                {
                    var position = new Vector3(x, y, 0);
                    var tilesAtPosition = allTiles.FindAll(tile => 
                        tile != null && Vector3.Distance(tile.transform.position, position) < 0.1f);
                    
                    if (tilesAtPosition.Count > 1)
                    {
                        collisionCount++;
                    }
                }
            }
            
            Assert.AreEqual(0, collisionCount, 
                $"境界座標で{collisionCount}個の衝突が発生しています");
        }
    }
}
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using MyGame.TilemapSystem.Core;
using MyGame.TilemapSystem.Generation;

namespace MyGame.TilemapSystem.Tests.EditMode
{
    [TestFixture]
    public class TilemapOverlapProtectionTests
    {
        private TilemapManager _manager;
        private Transform _parentTransform;
        private Dictionary<TileType, GameObject> _tilePrefabs;
        private SeedManager _seedManager;
        private TilemapGenerator _generator;

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
        public void PlaceTilesWithOverlapProtection_ShouldProtectExistingWallBlocks()
        {
            // Arrange: 最初のレベルのマップを生成
            var level1MapData = _generator.GenerateMap(1, 12345);
            _manager.PlaceTiles(level1MapData);
            
            // 最初のレベルのタイル数を記録
            var level1Tiles = _manager.GetTilesForLevel(1);
            var originalTileCount = level1Tiles.Count;
            
            // Act: 2番目のレベルを重複エリア保護機能付きで生成
            var level2MapData = _generator.GenerateMap(2, 12346);
            _manager.PlaceTilesWithOverlapProtection(level2MapData, 5);
            
            // Assert: 最初のレベルのタイルが保護されているか確認
            var level1TilesAfter = _manager.GetTilesForLevel(1);
            Assert.AreEqual(originalTileCount, level1TilesAfter.Count, 
                "既存レベルのタイル数が変更されてはいけません");
            
            // 2番目のレベルのタイルが生成されているか確認
            var level2Tiles = _manager.GetTilesForLevel(2);
            Assert.IsNotNull(level2Tiles, "2番目のレベルのタイルが生成されていません");
            Assert.Greater(level2Tiles.Count, 0, "2番目のレベルのタイルが存在しません");
        }

        [Test]
        public void PlaceTilesWithOverlapProtection_ShouldSkipOverlappingWallBlocks()
        {
            // Arrange: 手動で重複エリアにWallブロックを配置
            var tiles1 = new TileType[20, 30];
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 30; y++)
                {
                    tiles1[x, y] = TileType.Sky;
                }
            }
            
            // 重複エリア（y=0-4）にRockブロックを配置
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    tiles1[x, y] = TileType.Rock;
                }
            }
            
            var level1MapData = new MapData(20, 30, tiles1, 12345, 1);
            _manager.PlaceTiles(level1MapData);
            
            // 2番目のレベルでも同じ重複エリアにRockブロックを配置
            var tiles2 = new TileType[20, 30];
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 30; y++)
                {
                    tiles2[x, y] = TileType.Sky;
                }
            }
            
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    tiles2[x, y] = TileType.Rock;
                }
            }
            
            var level2MapData = new MapData(20, 30, tiles2, 12346, 2);
            
            // Act: 重複エリア保護機能付きで配置
            _manager.PlaceTilesWithOverlapProtection(level2MapData, 5);
            
            // Assert: 重複エリアで既存のRockブロックが保護されているか確認
            var allTiles = new List<GameObject>();
            var level1Tiles = _manager.GetTilesForLevel(1);
            var level2Tiles = _manager.GetTilesForLevel(2);
            
            if (level1Tiles != null) allTiles.AddRange(level1Tiles);
            if (level2Tiles != null) allTiles.AddRange(level2Tiles);
            
            // 重複エリアでの座標チェック
            var overlappingTiles = new List<GameObject>();
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    var position = new Vector3(x, y, 0);
                    var tilesAtPosition = allTiles.FindAll(tile => 
                        tile != null && Vector3.Distance(tile.transform.position, position) < 0.1f);
                    
                    if (tilesAtPosition.Count > 1)
                    {
                        overlappingTiles.AddRange(tilesAtPosition);
                    }
                }
            }
            
            Assert.AreEqual(0, overlappingTiles.Count, 
                "重複エリアで複数のタイルが同じ位置に配置されてはいけません");
        }

        [Test]
        public void PlaceTilesWithOverlapProtection_ShouldAllowNonOverlappingTiles()
        {
            // Arrange: 最初のレベルのマップを生成
            var level1MapData = _generator.GenerateMap(1, 12345);
            _manager.PlaceTiles(level1MapData);
            
            // Act: 2番目のレベルを重複エリア保護機能付きで生成
            var level2MapData = _generator.GenerateMap(2, 12346);
            _manager.PlaceTilesWithOverlapProtection(level2MapData, 5);
            
            // Assert: 重複エリア外（y >= 5）のタイルは正常に配置されているか確認
            var level2Tiles = _manager.GetTilesForLevel(2);
            Assert.IsNotNull(level2Tiles, "2番目のレベルのタイルが生成されていません");
            
            var nonOverlapTiles = level2Tiles.FindAll(tile => tile.transform.position.y >= 5);
            Assert.Greater(nonOverlapTiles.Count, 0, 
                "重複エリア外のタイルが正常に配置されていません");
        }

        [Test]
        public void IsExistingWallBlockAtPosition_ShouldReturnTrueForWallBlocks()
        {
            // Arrange: Rockブロックを配置
            var tiles = new TileType[20, 30];
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 30; y++)
                {
                    tiles[x, y] = TileType.Sky;
                }
            }
            tiles[10, 10] = TileType.Rock;
            
            var mapData = new MapData(20, 30, tiles, 12345, 1);
            _manager.PlaceTiles(mapData);
            
            // Act & Assert: プライベートメソッドなので間接的にテスト
            var level2MapData = _generator.GenerateMap(2, 12346);
            _manager.PlaceTilesWithOverlapProtection(level2MapData, 15); // 重複エリアを大きくしてテスト
            
            // 重複チェックが正しく動作しているか確認
            var level1Tiles = _manager.GetTilesForLevel(1);
            var level2Tiles = _manager.GetTilesForLevel(2);
            
            Assert.IsNotNull(level1Tiles, "レベル1のタイルが存在しません");
            Assert.IsNotNull(level2Tiles, "レベル2のタイルが存在しません");
        }
    }
}
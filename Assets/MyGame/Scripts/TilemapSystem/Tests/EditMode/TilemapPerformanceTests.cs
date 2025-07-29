using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MyGame.TilemapSystem.Core;
using System.Diagnostics;

namespace MyGame.TilemapSystem.Tests.EditMode
{
    public class TilemapPerformanceTests
    {
        // パフォーマンス閾値定数
        private const int SMALL_MAP_THRESHOLD_MS = 100;
        private const int MEDIUM_MAP_THRESHOLD_MS = 500;
        private const int TILE_UPDATE_THRESHOLD_MS = 50;
        private const int MEMORY_OPTIMIZATION_THRESHOLD_MS = 100;
        private const int BLOCK_QUERY_THRESHOLD_MS = 10;
        private const int MOVEMENT_QUERY_THRESHOLD_MS = 5;
        
        private GameObject _parentObject;
        private GameObject _universalTilePrefab;
        private SpriteManager _spriteManager;
        private TilemapManager _tilemapManager;
        
        [SetUp]
        public void SetUp()
        {
            _parentObject = new GameObject("TestParent");
            
            // UniversalTile Prefabの作成
            _universalTilePrefab = new GameObject("UniversalTile");
            var spriteRenderer = _universalTilePrefab.AddComponent<SpriteRenderer>();
            var tileController = _universalTilePrefab.AddComponent<TileController>();
            
            // SpriteManagerの作成とテストスプライトの設定
            _spriteManager = ScriptableObject.CreateInstance<SpriteManager>();
            
            var testTexture = new Texture2D(1, 1);
            testTexture.SetPixel(0, 0, Color.white);
            testTexture.Apply();
            var testSprite = Sprite.Create(testTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
            
            _spriteManager.SetSpriteForBlockType(BlockType.Ground, testSprite);
            _spriteManager.SetSpriteForBlockType(BlockType.Rock, testSprite);
            _spriteManager.SetSpriteForBlockType(BlockType.Sky, testSprite);
            
            // TileControllerにSpriteManagerを設定
            tileController.SpriteManager = _spriteManager;
            
            _tilemapManager = new TilemapManager(_parentObject.transform, _universalTilePrefab);
        }
        
        [TearDown]
        public void TearDown()
        {
            if (_parentObject != null)
            {
                Object.DestroyImmediate(_parentObject);
            }
            
            if (_universalTilePrefab != null)
            {
                Object.DestroyImmediate(_universalTilePrefab);
            }
            
            if (_spriteManager != null)
            {
                Object.DestroyImmediate(_spriteManager);
            }
        }
        
        [Test]
        public void PlaceTiles_PerformanceTest_SmallMap()
        {
            // Arrange
            var mapData = CreateTestMapData(10, 10, 1);
            
            // Act & Measure
            var stopwatch = Stopwatch.StartNew();
            _tilemapManager.PlaceTiles(mapData);
            stopwatch.Stop();
            
            // Assert
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            Assert.Less(elapsedMs, SMALL_MAP_THRESHOLD_MS, $"Small map (10x10) placement took {elapsedMs}ms, expected < {SMALL_MAP_THRESHOLD_MS}ms");
            
            // Verify tiles were created
            var tiles = _tilemapManager.GetTilesForLevel(1);
            Assert.IsNotNull(tiles);
            Assert.AreEqual(100, tiles.Count); // 10x10 = 100 tiles
            
            UnityEngine.Debug.Log($"[Performance] Small map (10x10) placement: {elapsedMs}ms");
        }
        
        [Test]
        public void PlaceTiles_PerformanceTest_MediumMap()
        {
            // Arrange
            var mapData = CreateTestMapData(50, 50, 1);
            
            // Act & Measure
            var stopwatch = Stopwatch.StartNew();
            _tilemapManager.PlaceTiles(mapData);
            stopwatch.Stop();
            
            // Assert
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            Assert.Less(elapsedMs, MEDIUM_MAP_THRESHOLD_MS, $"Medium map (50x50) placement took {elapsedMs}ms, expected < {MEDIUM_MAP_THRESHOLD_MS}ms");
            
            // Verify tiles were created
            var tiles = _tilemapManager.GetTilesForLevel(1);
            Assert.IsNotNull(tiles);
            Assert.AreEqual(2500, tiles.Count); // 50x50 = 2500 tiles
            
            UnityEngine.Debug.Log($"[Performance] Medium map (50x50) placement: {elapsedMs}ms");
        }
        
        [Test]
        public void UpdateTileDisplay_PerformanceTest()
        {
            // Arrange
            var mapData = CreateTestMapData(20, 20, 1);
            _tilemapManager.PlaceTiles(mapData);
            
            var testPositions = new Vector2Int[]
            {
                new Vector2Int(5, 5),
                new Vector2Int(10, 10),
                new Vector2Int(15, 15),
                new Vector2Int(3, 7),
                new Vector2Int(12, 18)
            };
            
            // Act & Measure - タイル変更のパフォーマンステスト
            var stopwatch = Stopwatch.StartNew();
            
            foreach (var position in testPositions)
            {
                _tilemapManager.OnPlayerHitTile(position, 1);
            }
            
            stopwatch.Stop();
            
            // Assert
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            Assert.Less(elapsedMs, TILE_UPDATE_THRESHOLD_MS, $"Tile updates took {elapsedMs}ms, expected < {TILE_UPDATE_THRESHOLD_MS}ms");
            
            UnityEngine.Debug.Log($"[Performance] 5 tile updates: {elapsedMs}ms");
        }
        
        [Test]
        public void MemoryOptimization_PerformanceTest()
        {
            // Arrange - 複数レベルのマップを作成
            for (int level = 1; level <= 10; level++)
            {
                var mapData = CreateTestMapData(20, 20, level);
                _tilemapManager.PlaceTiles(mapData);
            }
            
            // Act & Measure - メモリ最適化のパフォーマンステスト
            var stopwatch = Stopwatch.StartNew();
            _tilemapManager.OptimizeMemory(5); // レベル5を基準として最適化
            stopwatch.Stop();
            
            // Assert
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            Assert.Less(elapsedMs, MEMORY_OPTIMIZATION_THRESHOLD_MS, $"Memory optimization took {elapsedMs}ms, expected < {MEMORY_OPTIMIZATION_THRESHOLD_MS}ms");
            
            // レベル3-7のみが残っているかチェック
            for (int level = 1; level <= 10; level++)
            {
                bool shouldBeLoaded = (level >= 3 && level <= 7);
                Assert.AreEqual(shouldBeLoaded, _tilemapManager.IsMapLoaded(level), 
                    $"Level {level} loaded state should be {shouldBeLoaded}");
            }
            
            UnityEngine.Debug.Log($"[Performance] Memory optimization: {elapsedMs}ms");
        }
        
        [Test]
        public void BlockTypeQuery_PerformanceTest()
        {
            // Arrange
            var mapData = CreateTestMapData(30, 30, 1);
            _tilemapManager.PlaceTiles(mapData);
            
            var queryPositions = new Vector2Int[100];
            for (int i = 0; i < 100; i++)
            {
                queryPositions[i] = new Vector2Int(
                    UnityEngine.Random.Range(0, 30),
                    UnityEngine.Random.Range(0, 30)
                );
            }
            
            // Act & Measure - ブロックタイプクエリのパフォーマンステスト
            var stopwatch = Stopwatch.StartNew();
            
            foreach (var position in queryPositions)
            {
                var blockType = _tilemapManager.GetBlockTypeAt(position, 1);
                Assert.IsTrue(System.Enum.IsDefined(typeof(BlockType), blockType));
            }
            
            stopwatch.Stop();
            
            // Assert
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            Assert.Less(elapsedMs, BLOCK_QUERY_THRESHOLD_MS, $"100 block type queries took {elapsedMs}ms, expected < {BLOCK_QUERY_THRESHOLD_MS}ms");
            
            UnityEngine.Debug.Log($"[Performance] 100 block type queries: {elapsedMs}ms");
        }
        
        [Test]
        public void MovementQuery_PerformanceTest()
        {
            // Arrange
            var mapData = CreateTestMapData(25, 25, 1);
            _tilemapManager.PlaceTiles(mapData);
            
            var queryPositions = new Vector2Int[50];
            for (int i = 0; i < 50; i++)
            {
                queryPositions[i] = new Vector2Int(
                    UnityEngine.Random.Range(0, 25),
                    UnityEngine.Random.Range(0, 25)
                );
            }
            
            // Act & Measure - 移動可能性クエリのパフォーマンステスト
            var stopwatch = Stopwatch.StartNew();
            
            foreach (var position in queryPositions)
            {
                var canPass = _tilemapManager.CanPlayerPassThrough(position, 1);
                // 結果は真偽値なので、型チェックのみ実施
                Assert.IsTrue(canPass == true || canPass == false);
            }
            
            stopwatch.Stop();
            
            // Assert
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            Assert.Less(elapsedMs, MOVEMENT_QUERY_THRESHOLD_MS, $"50 movement queries took {elapsedMs}ms, expected < {MOVEMENT_QUERY_THRESHOLD_MS}ms");
            
            UnityEngine.Debug.Log($"[Performance] 50 movement queries: {elapsedMs}ms");
        }
        
        private MapData CreateTestMapData(int width, int height, int level)
        {
            var tiles = new BlockType[width, height];
            var blockTypes = new[] { BlockType.Empty, BlockType.Ground, BlockType.Rock, BlockType.Sky };
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y] = blockTypes[(x + y) % blockTypes.Length];
                }
            }
            
            return new MapData(width, height, tiles, 12345, level);
        }
    }
}
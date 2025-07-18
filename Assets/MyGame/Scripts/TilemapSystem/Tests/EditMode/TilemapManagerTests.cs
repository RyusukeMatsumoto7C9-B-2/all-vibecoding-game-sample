using NUnit.Framework;
using UnityEngine;
using MyGame.TilemapSystem.Core;
using System.Collections.Generic;

namespace MyGame.TilemapSystem.Tests.EditMode
{
    [Description("2DSprite Prefabベースのタイルマップ管理システムのテスト")]
    public class TilemapManagerTests
    {
        private TilemapManager _manager;
        private Transform _parentTransform;
        private Dictionary<TileType, GameObject> _tilePrefabs;
        private GameObject _testGameObject;

        [SetUp]
        public void SetUp()
        {
            // テスト用GameObjectを作成
            _testGameObject = new GameObject("TestParent");
            _parentTransform = _testGameObject.transform;

            // テスト用プレハブを作成
            var skyPrefab = new GameObject("SkyTile");
            skyPrefab.AddComponent<SpriteRenderer>();
            
            var groundPrefab = new GameObject("GroundTile");
            groundPrefab.AddComponent<SpriteRenderer>();
            
            var rockPrefab = new GameObject("RockTile");
            rockPrefab.AddComponent<SpriteRenderer>();
            
            var treasurePrefab = new GameObject("TreasureTile");
            treasurePrefab.AddComponent<SpriteRenderer>();

            _tilePrefabs = new Dictionary<TileType, GameObject>
            {
                { TileType.Sky, skyPrefab },
                { TileType.Ground, groundPrefab },
                { TileType.Rock, rockPrefab },
                { TileType.Treasure, treasurePrefab }
            };

            _manager = new TilemapManager(_parentTransform, _tilePrefabs);
        }

        [TearDown]
        public void TearDown()
        {
            if (_testGameObject != null)
            {
                Object.DestroyImmediate(_testGameObject);
            }
            
            foreach (var prefab in _tilePrefabs.Values)
            {
                if (prefab != null)
                {
                    Object.DestroyImmediate(prefab);
                }
            }
        }

        [Test]
        [Description("タイル配置でGameObjectが正しく生成されることを検証")]
        public void PlaceTiles_WithValidMapData_CreatesGameObjects()
        {
            var tiles = new TileType[3, 3];
            tiles[0, 0] = TileType.Rock;
            tiles[1, 1] = TileType.Ground;
            tiles[2, 2] = TileType.Treasure;
            
            var mapData = new MapData(3, 3, tiles, 12345, 1);

            _manager.PlaceTiles(mapData);

            // 子オブジェクトが3個生成されることを確認
            Assert.AreEqual(3, _parentTransform.childCount, "期待される数のタイルGameObjectが生成されていない");
        }

        [Test]
        [Description("タイル配置で正しい座標にGameObjectが配置されることを検証")]
        public void PlaceTiles_WithValidMapData_PlacesGameObjectsAtCorrectPositions()
        {
            var tiles = new TileType[2, 2];
            tiles[0, 0] = TileType.Rock;
            tiles[1, 1] = TileType.Ground;
            
            var mapData = new MapData(2, 2, tiles, 12345, 1);

            _manager.PlaceTiles(mapData);

            var childCount = _parentTransform.childCount;
            Assert.AreEqual(2, childCount, "期待される数のタイルが配置されていない");

            // 配置されたGameObjectの座標を確認
            bool foundRockAt00 = false;
            bool foundGroundAt11 = false;

            for (int i = 0; i < childCount; i++)
            {
                var child = _parentTransform.GetChild(i);
                var position = child.position;

                if (Vector3.Distance(position, new Vector3(0, 0, 0)) < 0.1f)
                {
                    foundRockAt00 = true;
                }
                else if (Vector3.Distance(position, new Vector3(1, 1, 0)) < 0.1f)
                {
                    foundGroundAt11 = true;
                }
            }

            Assert.IsTrue(foundRockAt00, "座標(0,0)にRockタイルが配置されていない");
            Assert.IsTrue(foundGroundAt11, "座標(1,1)にGroundタイルが配置されていない");
        }

        [Test]
        [Description("同じレベルのマップを再配置時に既存タイルが削除されることを検証")]
        public void PlaceTiles_SameLevel_ReplacesExistingTiles()
        {
            var tiles1 = new TileType[2, 2];
            tiles1[0, 0] = TileType.Rock;
            var mapData1 = new MapData(2, 2, tiles1, 12345, 1);

            _manager.PlaceTiles(mapData1);
            Assert.AreEqual(1, _parentTransform.childCount, "初回配置でタイル数が正しくない");

            var tiles2 = new TileType[2, 2];
            tiles2[0, 0] = TileType.Rock;
            tiles2[1, 1] = TileType.Ground;
            var mapData2 = new MapData(2, 2, tiles2, 12345, 1);

            _manager.PlaceTiles(mapData2);
            Assert.AreEqual(2, _parentTransform.childCount, "再配置後のタイル数が正しくない");
        }

        [Test]
        [Description("タイル削除でGameObjectが正しく破棄されることを検証")]
        public void ClearTiles_WithValidLevel_DestroysGameObjects()
        {
            var tiles = new TileType[2, 2];
            tiles[0, 0] = TileType.Rock;
            tiles[1, 1] = TileType.Ground;
            var mapData = new MapData(2, 2, tiles, 12345, 1);

            _manager.PlaceTiles(mapData);
            Assert.AreEqual(2, _parentTransform.childCount, "配置前の子オブジェクト数が正しくない");

            _manager.ClearTiles(1);
            Assert.AreEqual(0, _parentTransform.childCount, "削除後に子オブジェクトが残っている");
        }

        [Test]
        [Description("存在しないレベルの削除要求で例外が発生しないことを検証")]
        public void ClearTiles_WithNonExistentLevel_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _manager.ClearTiles(999), 
                "存在しないレベルの削除で例外が発生した");
        }

        [Test]
        [Description("マップ生成イベントが正しく発火されることを検証")]
        public void PlaceTiles_Always_FiresMapGeneratedEvent()
        {
            bool eventFired = false;
            MapData receivedMapData = default;

            _manager.OnMapGenerated += (mapData) =>
            {
                eventFired = true;
                receivedMapData = mapData;
            };

            var tiles = new TileType[2, 2];
            tiles[0, 0] = TileType.Rock;
            var mapData = new MapData(2, 2, tiles, 12345, 1);

            _manager.PlaceTiles(mapData);

            Assert.IsTrue(eventFired, "OnMapGeneratedイベントが発火されていない");
            Assert.AreEqual(mapData.Level, receivedMapData.Level, "イベントで受信したMapDataが正しくない");
        }

        [Test]
        [Description("メモリ最適化で範囲外のレベルが削除されることを検証")]
        public void OptimizeMemory_WithMultipleLevels_RemovesOutOfRangeLevels()
        {
            // 複数レベルのマップを配置（各レベルで1個のRockタイル）
            for (int level = 1; level <= 5; level++)
            {
                var tiles = new TileType[2, 2];
                tiles[0, 0] = TileType.Rock;  // 1個だけRock、残り3個はEmpty（デフォルト）
                var mapData = new MapData(2, 2, tiles, 12345, level);
                _manager.PlaceTiles(mapData);
            }

            // 初期状態で5レベル分のタイルが配置されている（各レベル1個ずつ）
            Assert.AreEqual(5, _parentTransform.childCount, "複数レベル配置後のタイル数が正しくない");

            // レベル3で最適化実行（±2の範囲なので1,2,3,4,5全て残る）
            _manager.OptimizeMemory(3);
            Assert.AreEqual(5, _parentTransform.childCount, "最適化後のタイル数が正しくない（全て範囲内）");

            // レベル1で最適化実行（±2の範囲なので1,2,3のみ残る）
            _manager.OptimizeMemory(1);
            Assert.AreEqual(3, _parentTransform.childCount, "最適化後のタイル数が正しくない（レベル4,5が削除されるはず）");
        }

        [Test]
        [Description("ロードされたマップの状態確認が正しく動作することを検証")]
        public void IsMapLoaded_WithLoadedAndUnloadedMaps_ReturnsCorrectStatus()
        {
            var tiles = new TileType[2, 2];
            tiles[0, 0] = TileType.Rock;
            var mapData = new MapData(2, 2, tiles, 12345, 1);

            Assert.IsFalse(_manager.IsMapLoaded(1), "マップ配置前にロード済みとして判定されている");

            _manager.PlaceTiles(mapData);
            Assert.IsTrue(_manager.IsMapLoaded(1), "マップ配置後にロード済みとして判定されていない");

            _manager.ClearTiles(1);
            Assert.IsFalse(_manager.IsMapLoaded(1), "マップ削除後にロード済みとして判定されている");
        }

        [Test]
        [Description("Skyタイルが正しく配置されることを検証")]
        public void PlaceTiles_WithSkyTiles_PlacesCorrectly()
        {
            var tiles = new TileType[2, 2];
            tiles[0, 0] = TileType.Sky;
            tiles[1, 1] = TileType.Sky;
            
            var mapData = new MapData(2, 2, tiles, 12345, 1);

            _manager.PlaceTiles(mapData);

            Assert.AreEqual(2, _parentTransform.childCount, "Skyタイルが正しく配置されていない");
        }

        [Test]
        [Description("Treasureタイルが正しく配置されることを検証")]
        public void PlaceTiles_WithTreasureTiles_PlacesCorrectly()
        {
            var tiles = new TileType[2, 2];
            tiles[0, 0] = TileType.Treasure;
            tiles[1, 1] = TileType.Treasure;
            
            var mapData = new MapData(2, 2, tiles, 12345, 1);

            _manager.PlaceTiles(mapData);

            Assert.AreEqual(2, _parentTransform.childCount, "Treasureタイルが正しく配置されていない");
        }

        [Test]
        [Description("全てのタイルタイプが混在するマップが正しく配置されることを検証")]
        public void PlaceTiles_WithMixedTileTypes_PlacesCorrectly()
        {
            var tiles = new TileType[3, 3];
            tiles[0, 0] = TileType.Sky;
            tiles[1, 0] = TileType.Ground;
            tiles[2, 0] = TileType.Rock;
            tiles[0, 1] = TileType.Treasure;
            tiles[1, 1] = TileType.Empty;  // Emptyは配置されない
            
            var mapData = new MapData(3, 3, tiles, 12345, 1);

            _manager.PlaceTiles(mapData);

            // Emptyタイル以外の4つのタイルが配置される
            Assert.AreEqual(4, _parentTransform.childCount, "混在タイルタイプが正しく配置されていない");
        }

        [Test]
        [Description("Emptyタイルが配置されないことを検証")]
        public void PlaceTiles_WithEmptyTiles_DoesNotPlaceEmptyTiles()
        {
            var tiles = new TileType[2, 2];
            tiles[0, 0] = TileType.Empty;
            tiles[1, 1] = TileType.Ground;
            
            var mapData = new MapData(2, 2, tiles, 12345, 1);

            _manager.PlaceTiles(mapData);

            // Emptyタイル以外の1つのタイルのみが配置される
            Assert.AreEqual(1, _parentTransform.childCount, "Emptyタイルが配置されている");
        }

        [Test]
        [Description("プレイヤーの通過判定が正しく動作することを検証")]
        public void CanPlayerPassThrough_WithDifferentTileTypes_ReturnsCorrectResults()
        {
            var tiles = new TileType[3, 3];
            tiles[0, 0] = TileType.Sky;
            tiles[1, 0] = TileType.Ground;
            tiles[2, 0] = TileType.Rock;
            tiles[0, 1] = TileType.Treasure;
            tiles[1, 1] = TileType.Empty;
            
            var mapData = new MapData(3, 3, tiles, 12345, 1);
            _manager.PlaceTiles(mapData);

            // 通過可能なタイル
            Assert.IsTrue(_manager.CanPlayerPassThrough(new Vector2Int(0, 0), 1), "Skyタイルは通過可能であるべき");
            Assert.IsTrue(_manager.CanPlayerPassThrough(new Vector2Int(1, 0), 1), "Groundタイルは通過可能であるべき");
            Assert.IsTrue(_manager.CanPlayerPassThrough(new Vector2Int(0, 1), 1), "Treasureタイルは通過可能であるべき");
            Assert.IsTrue(_manager.CanPlayerPassThrough(new Vector2Int(1, 1), 1), "Emptyタイルは通過可能であるべき");
            
            // 通過不可能なタイル
            Assert.IsFalse(_manager.CanPlayerPassThrough(new Vector2Int(2, 0), 1), "Rockタイルは通過不可能であるべき");
        }

        [Test]
        [Description("プレイヤーのタイル衝突処理が正しく動作することを検証")]
        public void OnPlayerHitTile_WithDifferentTileTypes_UpdatesCorrectly()
        {
            var tiles = new TileType[3, 3];
            tiles[0, 0] = TileType.Ground;
            tiles[1, 0] = TileType.Treasure;
            tiles[2, 0] = TileType.Rock;
            
            var mapData = new MapData(3, 3, tiles, 12345, 1);
            _manager.PlaceTiles(mapData);

            int eventFiredCount = 0;
            _manager.OnTileHit += (position, oldType, scoreGained) =>
            {
                eventFiredCount++;
                
                if (position == new Vector2Int(0, 0))
                {
                    Assert.AreEqual(TileType.Ground, oldType, "Groundタイルの衝突イベントが正しくない");
                    Assert.AreEqual(0, scoreGained, "Groundタイルのスコアが正しくない");
                }
                else if (position == new Vector2Int(1, 0))
                {
                    Assert.AreEqual(TileType.Treasure, oldType, "Treasureタイルの衝突イベントが正しくない");
                    Assert.AreEqual(100, scoreGained, "Treasureタイルのスコアが正しくない");
                }
            };

            // Groundタイルへの衝突
            _manager.OnPlayerHitTile(new Vector2Int(0, 0), 1);
            
            // Treasureタイルへの衝突
            _manager.OnPlayerHitTile(new Vector2Int(1, 0), 1);
            
            // Rockタイルへの衝突（変化なし）
            _manager.OnPlayerHitTile(new Vector2Int(2, 0), 1);

            Assert.AreEqual(2, eventFiredCount, "タイル衝突イベントが正しく発火されていない");
        }

        [Test]
        [Description("マップ境界外での操作が安全に処理されることを検証")]
        public void TileBehaviorMethods_WithOutOfBoundsCoordinates_HandleSafely()
        {
            var tiles = new TileType[2, 2];
            tiles[0, 0] = TileType.Ground;
            
            var mapData = new MapData(2, 2, tiles, 12345, 1);
            _manager.PlaceTiles(mapData);

            // 境界外座標での操作
            Assert.IsTrue(_manager.CanPlayerPassThrough(new Vector2Int(-1, -1), 1), "境界外座標では通過可能であるべき");
            Assert.IsTrue(_manager.CanPlayerPassThrough(new Vector2Int(10, 10), 1), "境界外座標では通過可能であるべき");
            
            Assert.DoesNotThrow(() => _manager.OnPlayerHitTile(new Vector2Int(-1, -1), 1), "境界外座標での衝突処理で例外が発生した");
            Assert.DoesNotThrow(() => _manager.OnPlayerHitTile(new Vector2Int(10, 10), 1), "境界外座標での衝突処理で例外が発生した");
        }

        [Test]
        [Description("存在しないレベルでの操作が安全に処理されることを検証")]
        public void TileBehaviorMethods_WithNonExistentLevel_HandleSafely()
        {
            // 存在しないレベルでの操作
            Assert.IsTrue(_manager.CanPlayerPassThrough(new Vector2Int(0, 0), 999), "存在しないレベルでは通過可能であるべき");
            Assert.DoesNotThrow(() => _manager.OnPlayerHitTile(new Vector2Int(0, 0), 999), "存在しないレベルでの衝突処理で例外が発生した");
        }
    }
}
using NUnit.Framework;
using UnityEngine;
using MyGame.TilemapSystem.Core;
using System.Collections.Generic;

namespace MyGame.Player.Tests
{
    [TestFixture]
    [Description("プレイヤー移動制約の統合テスト：実際のブロックタイプとTilemapManagerを使用した動作検証")]
    public class PlayerMovementConstraintIntegrationTests
    {
        private PlayerMoveService _playerMoveService;
        private TilemapManager _tilemapManager;
        private MockTileBehavior _mockTileBehavior;

        // テスト用のTileBehavior実装
        private class MockTileBehavior : ITileBehavior
        {
            public bool CanPlayerPassThrough(BlockType blockType)
            {
                return blockType switch
                {
                    BlockType.Sky => false,
                    BlockType.Empty => true,
                    BlockType.Ground => true,
                    BlockType.Rock => false,
                    BlockType.Treasure => true,
                    _ => true
                };
            }

            public BlockType OnPlayerHit(BlockType blockType, Vector2Int position, out int scoreGained)
            {
                scoreGained = 0;
                return blockType;
            }

            public void OnTimeUpdate(Vector2Int position, BlockType[,] tiles, float deltaTime)
            {
                // テスト用のため何もしない
            }
        }

        [SetUp]
        public void SetUp()
        {
            // テスト用のオブジェクトを作成
            var parentTransform = new GameObject("TestParent").transform;
            var tilePrefabs = new Dictionary<BlockType, GameObject>();
            _mockTileBehavior = new MockTileBehavior();
            
            _tilemapManager = new TilemapManager(parentTransform, tilePrefabs, _mockTileBehavior);
            _playerMoveService = new PlayerMoveService();
            _playerMoveService.SetTilemapManager(_tilemapManager, 0);

            // テスト用のマップデータを作成（5x5の小さなマップ）
            var tiles = new BlockType[5, 5];
            
            // マップの配置
            // 0,0: Empty, 1,0: Ground, 2,0: Rock, 3,0: Empty, 4,0: Treasure
            // 0,1: Ground, 1,1: Empty, 2,1: Ground, 3,1: Rock, 4,1: Empty
            tiles[0, 0] = BlockType.Empty;
            tiles[1, 0] = BlockType.Ground;
            tiles[2, 0] = BlockType.Rock;
            tiles[3, 0] = BlockType.Empty;
            tiles[4, 0] = BlockType.Treasure;
            
            tiles[0, 1] = BlockType.Ground;
            tiles[1, 1] = BlockType.Empty;
            tiles[2, 1] = BlockType.Ground;
            tiles[3, 1] = BlockType.Rock;
            tiles[4, 1] = BlockType.Empty;

            // 他の座標はEmptyで埋める
            for (int x = 0; x < 5; x++)
            {
                for (int y = 2; y < 5; y++)
                {
                    tiles[x, y] = BlockType.Empty;
                }
            }

            var mapData = new MapData(5, 5, tiles, 12345, 0);
            _tilemapManager.PlaceTiles(mapData);
        }

        [TearDown]
        public void TearDown()
        {
            // テスト用に作成されたGameObjectを削除
            var testObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (var obj in testObjects)
            {
                if (obj.name == "TestParent")
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }

        [Test]
        [Description("Emptyブロック上への移動が成功することを検証")]
        public void Move_ToEmptyBlock_ShouldSucceed()
        {
            // Arrange
            _playerMoveService.SetPosition(new Vector2Int(0, 0)); // Empty位置に配置

            // Act - 右方向（Ground位置）への移動
            var result = _playerMoveService.Move(Direction.Right);

            // Assert
            Assert.IsTrue(result, "Emptyブロックから隣接するGroundブロックへの移動は成功すべき");
            Assert.AreEqual(new Vector2Int(1, 0), _playerMoveService.CurrentPosition);
        }

        [Test]
        [Description("Groundブロック上への移動が成功することを検証")]
        public void Move_ToGroundBlock_ShouldSucceed()
        {
            // Arrange
            _playerMoveService.SetPosition(new Vector2Int(1, 0)); // Ground位置に配置

            // Act - 左方向（Empty位置）への移動
            var result = _playerMoveService.Move(Direction.Left);

            // Assert
            Assert.IsTrue(result, "GroundブロックからEmptyブロックへの移動は成功すべき");
            Assert.AreEqual(new Vector2Int(0, 0), _playerMoveService.CurrentPosition);
        }

        [Test]
        [Description("Rockブロック上への移動が失敗することを検証")]
        public void Move_ToRockBlock_ShouldFail()
        {
            // Arrange
            _playerMoveService.SetPosition(new Vector2Int(1, 0)); // Ground位置に配置

            // Act - 右方向（Rock位置）への移動を試行
            var result = _playerMoveService.Move(Direction.Right);

            // Assert
            Assert.IsFalse(result, "Rockブロックへの移動は失敗すべき");
            Assert.AreEqual(new Vector2Int(1, 0), _playerMoveService.CurrentPosition, "移動失敗時は位置が変更されないべき");
        }

        [Test]
        [Description("Treasureブロック上への移動が成功することを検証")]
        public void Move_ToTreasureBlock_ShouldSucceed()
        {
            // Arrange
            _playerMoveService.SetPosition(new Vector2Int(3, 0)); // Empty位置に配置

            // Act - 右方向（Treasure位置）への移動
            var result = _playerMoveService.Move(Direction.Right);

            // Assert
            Assert.IsTrue(result, "Treasureブロックへの移動は成功すべき");
            Assert.AreEqual(new Vector2Int(4, 0), _playerMoveService.CurrentPosition);
        }

        [Test]
        [Description("マップ範囲外への移動が成功することを検証（境界チェック）")]
        public void Move_OutOfMapBounds_ShouldSucceed()
        {
            // Arrange
            _playerMoveService.SetPosition(new Vector2Int(4, 4)); // マップの右上端に配置

            // Act - 右方向（範囲外）への移動
            var result = _playerMoveService.Move(Direction.Right);

            // Assert
            Assert.IsTrue(result, "マップ範囲外への移動は成功すべき（仕様通り）");
            Assert.AreEqual(new Vector2Int(5, 4), _playerMoveService.CurrentPosition);
        }

        [Test]
        [Description("CanMoveメソッドが各ブロックタイプに対して正しい結果を返すことを検証")]
        [TestCase(0, 0, Direction.Right, true, TestName = "Empty→Ground移動可能")]
        [TestCase(1, 0, Direction.Right, false, TestName = "Ground→Rock移動不可")]
        [TestCase(3, 0, Direction.Right, true, TestName = "Empty→Treasure移動可能")]
        [TestCase(0, 1, Direction.Right, true, TestName = "Ground→Empty移動可能")]
        [TestCase(2, 1, Direction.Right, false, TestName = "Ground→Rock移動不可")]
        public void CanMove_VariousBlockTypes_ShouldReturnCorrectResult(int startX, int startY, Direction direction, bool expectedResult)
        {
            // Arrange
            _playerMoveService.SetPosition(new Vector2Int(startX, startY));

            // Act
            var canMove = _playerMoveService.CanMove(direction);

            // Assert
            Assert.AreEqual(expectedResult, canMove, 
                $"座標({startX}, {startY})から{direction}方向への移動可否判定が間違っています");
        }

        [Test]
        [Description("連続した移動制約チェックが正常に動作することを検証")]
        public void ConsecutiveMovements_WithMixedConstraints_ShouldWorkCorrectly()
        {
            // Arrange
            _playerMoveService.SetPosition(new Vector2Int(0, 1)); // Ground位置に配置

            // Act & Assert - 段階的に移動してテスト
            
            // 1. 右へ移動（Ground→Empty）- 成功するはず
            var move1 = _playerMoveService.Move(Direction.Right);
            Assert.IsTrue(move1, "1回目の移動（Ground→Empty）は成功すべき");
            Assert.AreEqual(new Vector2Int(1, 1), _playerMoveService.CurrentPosition);

            // 2. 右へ移動（Empty→Ground）- 成功するはず
            var move2 = _playerMoveService.Move(Direction.Right);
            Assert.IsTrue(move2, "2回目の移動（Empty→Ground）は成功すべき");
            Assert.AreEqual(new Vector2Int(2, 1), _playerMoveService.CurrentPosition);

            // 3. 右へ移動（Ground→Rock）- 失敗するはず
            var move3 = _playerMoveService.Move(Direction.Right);
            Assert.IsFalse(move3, "3回目の移動（Ground→Rock）は失敗すべき");
            Assert.AreEqual(new Vector2Int(2, 1), _playerMoveService.CurrentPosition, "移動失敗時は位置が変更されないべき");

            // 4. 上へ移動（Ground→Empty）- 成功するはず
            var move4 = _playerMoveService.Move(Direction.Up);
            Assert.IsTrue(move4, "4回目の移動（Ground→Empty）は成功すべき");
            Assert.AreEqual(new Vector2Int(2, 2), _playerMoveService.CurrentPosition);
        }
    }
}
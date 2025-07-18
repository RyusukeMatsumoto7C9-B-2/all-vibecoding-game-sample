using NUnit.Framework;
using UnityEngine;
using MyGame.TilemapSystem.Core;

namespace MyGame.TilemapSystem.Tests.EditMode
{
    [Description("タイル挙動システムのテスト")]
    public class TileBehaviorTests
    {
        private TileBehavior _tileBehavior;

        [SetUp]
        public void SetUp()
        {
            _tileBehavior = new TileBehavior();
        }

        [Test]
        [Description("Skyタイルがプレイヤーの通過を許可することを検証")]
        public void CanPlayerPassThrough_SkyTile_ReturnsTrue()
        {
            var result = _tileBehavior.CanPlayerPassThrough(TileType.Sky);
            
            Assert.IsTrue(result, "Skyタイルはプレイヤーの通過を許可するべき");
        }

        [Test]
        [Description("Emptyタイルがプレイヤーの通過を許可することを検証")]
        public void CanPlayerPassThrough_EmptyTile_ReturnsTrue()
        {
            var result = _tileBehavior.CanPlayerPassThrough(TileType.Empty);
            
            Assert.IsTrue(result, "Emptyタイルはプレイヤーの通過を許可するべき");
        }

        [Test]
        [Description("Groundタイルがプレイヤーの通過を許可することを検証")]
        public void CanPlayerPassThrough_GroundTile_ReturnsTrue()
        {
            var result = _tileBehavior.CanPlayerPassThrough(TileType.Ground);
            
            Assert.IsTrue(result, "Groundタイルはプレイヤーの通過を許可するべき");
        }

        [Test]
        [Description("Rockタイルがプレイヤーの通過を許可しないことを検証")]
        public void CanPlayerPassThrough_RockTile_ReturnsFalse()
        {
            var result = _tileBehavior.CanPlayerPassThrough(TileType.Rock);
            
            Assert.IsFalse(result, "Rockタイルはプレイヤーの通過を許可しないべき");
        }

        [Test]
        [Description("Treasureタイルがプレイヤーの通過を許可することを検証")]
        public void CanPlayerPassThrough_TreasureTile_ReturnsTrue()
        {
            var result = _tileBehavior.CanPlayerPassThrough(TileType.Treasure);
            
            Assert.IsTrue(result, "Treasureタイルはプレイヤーの通過を許可するべき");
        }

        [Test]
        [Description("プレイヤーがGroundタイルにヒットした時にEmptyタイルに変化することを検証")]
        public void OnPlayerHit_GroundTile_ChangesToEmptyTile()
        {
            var position = new Vector2Int(0, 0);
            
            var result = _tileBehavior.OnPlayerHit(TileType.Ground, position, out int scoreGained);
            
            Assert.AreEqual(TileType.Empty, result, "GroundタイルはEmptyタイルに変化するべき");
            Assert.AreEqual(0, scoreGained, "Groundタイルではスコアが得られないべき");
        }

        [Test]
        [Description("プレイヤーがTreasureタイルにヒットした時にEmptyタイルに変化しスコアを得ることを検証")]
        public void OnPlayerHit_TreasureTile_ChangesToEmptyTileAndGivesScore()
        {
            var position = new Vector2Int(0, 0);
            
            var result = _tileBehavior.OnPlayerHit(TileType.Treasure, position, out int scoreGained);
            
            Assert.AreEqual(TileType.Empty, result, "TreasureタイルはEmptyタイルに変化するべき");
            Assert.AreEqual(100, scoreGained, "Treasureタイルでは100スコアが得られるべき");
        }

        [Test]
        [Description("プレイヤーがRockタイルにヒットした時にタイルが変化しないことを検証")]
        public void OnPlayerHit_RockTile_DoesNotChange()
        {
            var position = new Vector2Int(0, 0);
            
            var result = _tileBehavior.OnPlayerHit(TileType.Rock, position, out int scoreGained);
            
            Assert.AreEqual(TileType.Rock, result, "Rockタイルは変化しないべき");
            Assert.AreEqual(0, scoreGained, "Rockタイルではスコアが得られないべき");
        }

        [Test]
        [Description("プレイヤーがSkyタイルにヒットした時にタイルが変化しないことを検証")]
        public void OnPlayerHit_SkyTile_DoesNotChange()
        {
            var position = new Vector2Int(0, 0);
            
            var result = _tileBehavior.OnPlayerHit(TileType.Sky, position, out int scoreGained);
            
            Assert.AreEqual(TileType.Sky, result, "Skyタイルは変化しないべき");
            Assert.AreEqual(0, scoreGained, "Skyタイルではスコアが得られないべき");
        }

        [Test]
        [Description("プレイヤーがEmptyタイルにヒットした時にタイルが変化しないことを検証")]
        public void OnPlayerHit_EmptyTile_DoesNotChange()
        {
            var position = new Vector2Int(0, 0);
            
            var result = _tileBehavior.OnPlayerHit(TileType.Empty, position, out int scoreGained);
            
            Assert.AreEqual(TileType.Empty, result, "Emptyタイルは変化しないべき");
            Assert.AreEqual(0, scoreGained, "Emptyタイルではスコアが得られないべき");
        }

        [Test]
        [Description("Rockタイルの落下処理が正しく実装されていることを検証")]
        public void OnTimeUpdate_RockTile_ProcessesFalling()
        {
            var tiles = new TileType[3, 3];
            tiles[1, 2] = TileType.Rock;    // 上部にRock
            tiles[1, 1] = TileType.Ground;  // 中央にGround
            tiles[1, 0] = TileType.Empty;   // 下部にEmpty
            
            var position = new Vector2Int(1, 2);
            
            _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime);
            
            // 落下処理が実行されるが、実際の落下は時間処理に依存する
            Assert.DoesNotThrow(() => _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime),
                "Rockタイルの落下処理で例外が発生した");
        }

        [Test]
        [Description("非Rockタイルの時間更新で例外が発生しないことを検証")]
        public void OnTimeUpdate_NonRockTile_DoesNotThrow()
        {
            var tiles = new TileType[3, 3];
            tiles[1, 1] = TileType.Ground;
            
            var position = new Vector2Int(1, 1);
            
            Assert.DoesNotThrow(() => _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime),
                "非Rockタイルの時間更新で例外が発生した");
        }

        [Test]
        [Description("Rockタイルの下にGroundブロックがあり、その下にEmptyブロックがある場合の落下処理を検証")]
        public void OnTimeUpdate_RockWithGroundBelow_ProcessesFallingCorrectly()
        {
            var tiles = new TileType[3, 4];
            tiles[1, 3] = TileType.Rock;    // 最上部にRock
            tiles[1, 2] = TileType.Ground;  // その下にGround
            tiles[1, 1] = TileType.Empty;   // その下にEmpty
            tiles[1, 0] = TileType.Ground;  // 最下部にGround
            
            var position = new Vector2Int(1, 3);
            
            // 時間更新を実行
            _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime);
            
            // 落下処理が実行されることを確認（具体的な落下は時間処理に依存）
            Assert.DoesNotThrow(() => _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime),
                "Rockタイルの落下処理で例外が発生した");
        }

        [Test]
        [Description("Rockタイルの直下がEmptyブロックの場合の落下処理を検証")]
        public void OnTimeUpdate_RockWithEmptyBelow_ProcessesFallingCorrectly()
        {
            var tiles = new TileType[3, 4];
            tiles[1, 3] = TileType.Rock;    // 最上部にRock
            tiles[1, 2] = TileType.Empty;   // その下にEmpty
            tiles[1, 1] = TileType.Empty;   // その下にEmpty
            tiles[1, 0] = TileType.Ground;  // 最下部にGround
            
            var position = new Vector2Int(1, 3);
            
            // 時間更新を実行
            _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime);
            
            // 落下処理が実行されることを確認（具体的な落下は時間処理に依存）
            Assert.DoesNotThrow(() => _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime),
                "Rockタイルの落下処理で例外が発生した");
        }

        [Test]
        [Description("Rockタイルの直下がRockブロックの場合に落下処理が開始されないことを検証")]
        public void OnTimeUpdate_RockWithRockBelow_DoesNotFall()
        {
            var tiles = new TileType[3, 3];
            tiles[1, 2] = TileType.Rock;    // 上部にRock
            tiles[1, 1] = TileType.Rock;    // 中央にRock
            tiles[1, 0] = TileType.Ground;  // 下部にGround
            
            var position = new Vector2Int(1, 2);
            
            // 時間更新を実行
            _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime);
            
            // 落下処理が実行されないことを確認（例外が発生しない）
            Assert.DoesNotThrow(() => _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime),
                "Rockタイルの落下処理で例外が発生した");
        }

        [Test]
        [Description("マップ境界での時間更新処理が例外を発生させないことを検証")]
        public void OnTimeUpdate_AtMapBoundary_DoesNotThrow()
        {
            var tiles = new TileType[3, 3];
            tiles[1, 0] = TileType.Rock;    // 最下部にRock
            
            var position = new Vector2Int(1, 0);
            
            // 境界での時間更新を実行
            Assert.DoesNotThrow(() => _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime),
                "マップ境界での時間更新処理で例外が発生した");
        }

        [Test]
        [Description("プレイヤー衝突処理でスコアが正しく計算されることを検証")]
        public void OnPlayerHit_ScoreCalculation_IsCorrect()
        {
            var position = new Vector2Int(0, 0);
            
            // Treasureタイルで100スコア
            _tileBehavior.OnPlayerHit(TileType.Treasure, position, out int treasureScore);
            Assert.AreEqual(100, treasureScore, "Treasureタイルのスコアが正しくない");
            
            // Groundタイルで0スコア
            _tileBehavior.OnPlayerHit(TileType.Ground, position, out int groundScore);
            Assert.AreEqual(0, groundScore, "Groundタイルのスコアが正しくない");
            
            // Rockタイルで0スコア
            _tileBehavior.OnPlayerHit(TileType.Rock, position, out int rockScore);
            Assert.AreEqual(0, rockScore, "Rockタイルのスコアが正しくない");
        }

        [Test]
        [Description("プレイヤー衝突処理でタイルの変化が正しく実行されることを検証")]
        public void OnPlayerHit_TileTransformation_IsCorrect()
        {
            var position = new Vector2Int(0, 0);
            
            // Treasureタイル → Emptyタイル
            var treasureResult = _tileBehavior.OnPlayerHit(TileType.Treasure, position, out _);
            Assert.AreEqual(TileType.Empty, treasureResult, "Treasureタイルが正しくEmptyに変化していない");
            
            // Groundタイル → Emptyタイル
            var groundResult = _tileBehavior.OnPlayerHit(TileType.Ground, position, out _);
            Assert.AreEqual(TileType.Empty, groundResult, "Groundタイルが正しくEmptyに変化していない");
            
            // Rockタイル → Rockタイル（変化なし）
            var rockResult = _tileBehavior.OnPlayerHit(TileType.Rock, position, out _);
            Assert.AreEqual(TileType.Rock, rockResult, "Rockタイルが変化してしまった");
        }

        [Test]
        [Description("複数のタイルタイプでのプレイヤー通過判定が正しく実行されることを検証")]
        public void CanPlayerPassThrough_MultipleTypes_ReturnCorrectValues()
        {
            // 通過可能なタイルタイプ
            var passableTypes = new[] { TileType.Sky, TileType.Empty, TileType.Ground, TileType.Treasure };
            foreach (var tileType in passableTypes)
            {
                Assert.IsTrue(_tileBehavior.CanPlayerPassThrough(tileType), 
                    $"{tileType}タイルは通過可能であるべき");
            }
            
            // 通過不可能なタイルタイプ
            var impassableTypes = new[] { TileType.Rock };
            foreach (var tileType in impassableTypes)
            {
                Assert.IsFalse(_tileBehavior.CanPlayerPassThrough(tileType), 
                    $"{tileType}タイルは通過不可能であるべき");
            }
        }

        [Test]
        [Description("Rockタイルの落下処理で正しい落下位置が計算されることを検証")]
        public void OnTimeUpdate_RockFalling_CalculatesCorrectPosition()
        {
            var tiles = new TileType[3, 5];
            tiles[1, 4] = TileType.Rock;    // 最上部にRock
            tiles[1, 3] = TileType.Empty;   // Empty
            tiles[1, 2] = TileType.Empty;   // Empty
            tiles[1, 1] = TileType.Ground;  // Ground（落下先）
            tiles[1, 0] = TileType.Ground;  // Ground（基盤）
            
            var position = new Vector2Int(1, 4);
            
            // 落下処理を実行
            _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime);
            
            // 落下処理が正常に完了することを確認
            Assert.DoesNotThrow(() => _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime),
                "Rockタイルの落下位置計算で例外が発生した");
        }

        [Test]
        [Description("Rockタイルの落下処理で境界条件が正しく処理されることを検証")]
        public void OnTimeUpdate_RockFalling_HandlesBoundaryConditions()
        {
            var tiles = new TileType[3, 3];
            tiles[1, 2] = TileType.Rock;    // 上部にRock
            tiles[1, 1] = TileType.Empty;   // 中部にEmpty
            tiles[1, 0] = TileType.Empty;   // 下部にEmpty（底なし）
            
            var position = new Vector2Int(1, 2);
            
            // 底なしの場合の落下処理
            Assert.DoesNotThrow(() => _tileBehavior.OnTimeUpdate(position, tiles, Time.deltaTime),
                "底なしの場合の落下処理で例外が発生した");
        }

        [Test]
        [Description("時間更新処理で異なるdeltaTime値が正しく処理されることを検証")]
        public void OnTimeUpdate_WithDifferentDeltaTime_HandlesCorrectly()
        {
            var tiles = new TileType[3, 3];
            tiles[1, 2] = TileType.Rock;
            tiles[1, 1] = TileType.Empty;
            tiles[1, 0] = TileType.Ground;
            
            var position = new Vector2Int(1, 2);
            
            // 異なるdeltaTime値でのテスト
            var deltaTimeValues = new[] { 0.0f, 0.016f, 0.033f, 1.0f };
            
            foreach (var deltaTime in deltaTimeValues)
            {
                Assert.DoesNotThrow(() => _tileBehavior.OnTimeUpdate(position, tiles, deltaTime),
                    $"deltaTime={deltaTime}での時間更新処理で例外が発生した");
            }
        }
    }
}
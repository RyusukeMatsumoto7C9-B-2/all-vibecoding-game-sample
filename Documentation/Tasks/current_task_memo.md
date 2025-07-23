# 現在のタスクメモ

## Groundブロック破壊機能実装

### 概要
プレイヤーがGroundブロックに移動する際に、移動と同時にGroundブロックをEmptyブロックに変更する機能を実装する。

### 実装対象
- `Assets/MyGame/Scripts/Player/PlayerMoveService.cs`のMove()メソッド拡張
- 対応するテストケースの追加

### TODO

#### 高優先度
- [x] PlayerMoveService.Move()メソッドにGround破壊処理を追加
  - [x] ITilemapManagerにGetBlockTypeAt()メソッドを追加
  - [x] 移動先ブロックタイプの取得
  - [x] Ground判定と破壊処理の実行
  - [x] TilemapManager.OnPlayerHitTile()の呼び出し

#### 中優先度  
- [x] PlayerMoveServiceTestsにテストケース追加
  - [x] MockTilemapManagerの拡張（GetBlockTypeAt実装、スパイ機能追加）
  - [x] Groundブロック移動時の破壊テスト
  - [x] 他ブロックタイプでは破壊しないことのテスト
  - [x] TilemapManagerとの連携テスト

#### 低優先度
- [x] player_spec.mdの実装状況更新（❌→✅）

### 技術仕様
- 移動先がGroundブロックの場合のみ破壊処理を実行
- 移動成功後に即座に破壊処理を実行
- 既存のTileBehavior.OnPlayerHit()を活用
- TilemapManager.OnPlayerHitTile()でタイル表示更新

### 完了条件
1. ✅ プレイヤーがGroundブロックに移動するとEmptyブロックに変化する
2. ✅ 他のブロックタイプでは破壊処理が実行されない
3. ✅ 対応するテストが全て通過する
4. ✅ 仕様書の実装状況が更新される

## 実装完了

**実装日**: 2025-01-23

**実装内容**:
- ITilemapManagerインターフェースにGetBlockTypeAt()メソッドを追加
- TilemapManagerでGetBlockTypeAt()メソッドを実装
- PlayerMoveService.Move()でGroundブロック破壊処理を実装
- MockTilemapManagerを拡張してテスト用スパイ機能を追加
- PlayerMoveServiceTestsに3つの新しいテストケースを追加
- player_spec.mdの実装状況を✅に更新

**テストケース**:
1. `Move_ToGroundBlock_ShouldCallOnPlayerHitTile` - Groundブロック破壊処理の呼び出し確認
2. `Move_ToEmptyBlock_ShouldNotCallOnPlayerHitTile` - Emptyブロックでは破壊処理されないことの確認
3. `Move_ToTreasureBlock_ShouldNotCallOnPlayerHitTile` - Treasureブロックでは破壊処理されないことの確認
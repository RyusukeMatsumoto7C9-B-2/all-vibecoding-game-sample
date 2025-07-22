# テスト失敗修正タスク

## タスク概要
PlayerMoveServiceTests と TileBehaviorTests でテストが失敗している問題の修正を行います。

## 問題分析

### 1. TileBehaviorTests.cs の失敗原因
**Skyブロック仕様の矛盾**
- **実装**: `BlockType.Sky => false` (通過不可)
- **テスト期待値**: `Assert.IsTrue(result)` (通過可能を期待)
- **失敗箇所**:
  - `CanPlayerPassThrough_SkyTile_ReturnsTrue()` (行19-25)
  - `CanPlayerPassThrough_MultipleTypes_ReturnCorrectValues()` (行269-284)

### 2. PlayerMoveServiceTests.cs の失敗原因
**安全性修正による期待値の矛盾**
- TilemapManager未設定時の動作を移動不可に変更したが、テストが古い期待値のまま
- 具体的な失敗箇所を特定して修正が必要

## 修正計画

### タスク1: TileBehaviorテストの仕様矛盾修正
- [ ] Skyブロック仕様の確認・統一（実装に合わせて通過不可で統一）
- [ ] `CanPlayerPassThrough_SkyTile_ReturnsTrue()` の期待値を `Assert.IsFalse` に修正
- [ ] `CanPlayerPassThrough_MultipleTypes_ReturnCorrectValues()` でSkyを通過不可配列に移動

### タスク2: PlayerMoveServiceテストの安全性修正対応  
- [ ] TilemapManager未設定時の動作変更に伴うテスト期待値修正
- [ ] 失敗している具体的なテストケースの特定と修正
- [ ] 安全性修正後の仕様に合わせたテスト更新

### タスク3: 仕様書との整合性確認
- [ ] player_spec.mdとTileBehavior実装の整合性確認
- [ ] 必要に応じて仕様書の更新

## 期待される結果
- 全テストが成功し、実装とテストの整合性が確保される
- Skyブロックの仕様が統一され、混乱が解消される
- 安全性修正後の動作が適切にテストされる

## 技術的詳細
- `TileBehaviorTests.cs`: Skyブロック関連テストの期待値修正
- `PlayerMoveServiceTests.cs`: TilemapManager未設定時のテスト期待値修正
- 仕様書: 実装との整合性確認と必要に応じた更新
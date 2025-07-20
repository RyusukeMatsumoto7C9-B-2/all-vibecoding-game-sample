# 新しいタイルタイプ用Prefab作成ガイド

## 日付: 2025-07-16
## 作業者: Claude
## 関連Issue: #8 - 壁ブロック消失問題対応

## 概要

更新されたタイルマップシステム仕様に対応するため、新しいタイルタイプ用のPrefabを作成する必要があります。

## 必要なPrefab

### 1. 現在存在するPrefab
- `Assets/MyGame/Prefabs/Tiles/GroundTile.prefab` - 地面タイル（そのまま使用）
- `Assets/MyGame/Prefabs/Tiles/WallTile.prefab` - 壁タイル（RockTileとして使用）

### 2. 新規作成が必要なPrefab

#### A. SkyTile.prefab
- **目的**: 空タイル（レベル1の上5マス用）
- **推奨色**: 薄青色（#87CEEB）
- **サイズ**: 64x64ピクセル
- **設定**: PixelsPerUnit = 64, FilterMode = Point

#### B. RockTile.prefab
- **目的**: 岩タイル（既存のWallTileを使用可能）
- **推奨色**: 灰色（#696969）
- **サイズ**: 64x64ピクセル
- **設定**: PixelsPerUnit = 64, FilterMode = Point

#### C. TreasureTile.prefab
- **目的**: 宝物タイル
- **推奨色**: 黄色（#FFD700）
- **サイズ**: 64x64ピクセル
- **設定**: PixelsPerUnit = 64, FilterMode = Point

## 作成手順

### 1. スプライト画像の準備

#### A. 画像ファイルの作成
1. `Assets/MyGame/Sprites/` フォルダに以下の画像を作成：
   - `SkySprite.png` - 64x64ピクセル、薄青色
   - `RockSprite.png` - 64x64ピクセル、灰色（既存のWallSpriteを使用可能）
   - `TreasureSprite.png` - 64x64ピクセル、黄色

#### B. インポート設定
各画像ファイルのインポート設定を以下のように変更：
```
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
Pixels Per Unit: 64
Filter Mode: Point (no filter)
Format: RGBA32
```

### 2. Prefabの作成

#### A. SkyTile.prefab
1. 空のGameObjectを作成し、`SkyTile`と命名
2. SpriteRendererコンポーネントを追加
3. `SkySprite`をSpriteフィールドに設定
4. `Assets/MyGame/Prefabs/Tiles/SkyTile.prefab`として保存

#### B. RockTile.prefab
1. 既存の`WallTile.prefab`を複製
2. 名前を`RockTile`に変更
3. `RockSprite`をSpriteフィールドに設定（必要に応じて）
4. `Assets/MyGame/Prefabs/Tiles/RockTile.prefab`として保存

#### C. TreasureTile.prefab
1. 空のGameObjectを作成し、`TreasureTile`と命名
2. SpriteRendererコンポーネントを追加
3. `TreasureSprite`をSpriteフィールドに設定
4. `Assets/MyGame/Prefabs/Tiles/TreasureTile.prefab`として保存

### 3. TilemapSystemTesterの設定

TilemapSystemTesterコンポーネントのインスペクターで以下のPrefabを設定：

```
Sky Tile Prefab: SkyTile.prefab
Ground Tile Prefab: GroundTile.prefab
Rock Tile Prefab: RockTile.prefab
Treasure Tile Prefab: TreasureTile.prefab
```

## 推奨カラーパレット

```
Sky: #87CEEB (薄青色)
Ground: #8B4513 (茶色) - 既存
Rock: #696969 (灰色)
Treasure: #FFD700 (黄色)
Empty: 透明（表示されない）
```

## 動作確認

### 1. 基本動作テスト
1. シーンでPlay modeに入る
2. 自動的にタイルマップが生成される
3. レベル1で上5マスにSkyタイルが表示される
4. 地面にGroundタイル、障害物にRockタイルが表示される

### 2. タイル挙動テスト
1. プレイヤーがGroundタイルに衝突→Emptyタイルに変化
2. プレイヤーがTreasureタイルに衝突→Emptyタイルに変化＋スコア加算
3. プレイヤーがRockタイルに衝突→通過不可

### 3. 時間経過テスト
1. Rockタイルの落下処理が正しく動作する
2. 条件に応じてRockタイルが落下する

## トラブルシューティング

### Q1: タイルが表示されない
- SpriteRendererコンポーネントが追加されているか確認
- Spriteフィールドが正しく設定されているか確認
- PrefabがTilemapSystemTesterに正しく設定されているか確認

### Q2: タイルの色が正しくない
- スプライト画像の色が正しいか確認
- インポート設定でFormatがRGBA32になっているか確認

### Q3: タイルのサイズが正しくない
- PixelsPerUnitが64に設定されているか確認
- スプライト画像のサイズが64x64ピクセルになっているか確認

## 関連ファイル

- `Assets/MyGame/Scripts/TilemapSystem/TilemapSystemTester.cs`
- `Assets/MyGame/Scripts/TilemapSystem/Core/TileDefinition.cs`
- `Assets/MyGame/Scripts/TilemapSystem/Core/TileBehavior.cs`
- `Assets/MyGame/Scripts/TilemapSystem/Core/TilemapManager.cs`

## 次のステップ

1. 作成したPrefabを使用してタイルマップシステムをテスト
2. プレイヤーシステムとの連携テスト
3. Issue #8の壁ブロック消失問題の検証
4. 必要に応じてタイル挙動の調整

## 注意事項

- 既存のWallTile.prefabをRockTileとして使用可能
- Emptyタイルは表示されないため、Prefabは不要
- 各Prefabは再利用可能な設計にする
- タイル間の一貫性を保つため、同じピクセル数とフィルタ設定を使用

## 作業完了チェックリスト

- [ ] SkySprite.png作成
- [ ] RockSprite.png作成（または既存WallSprite使用）
- [ ] TreasureSprite.png作成
- [ ] SkyTile.prefab作成
- [ ] RockTile.prefab作成
- [ ] TreasureTile.prefab作成
- [ ] TilemapSystemTester設定更新
- [ ] 動作確認テスト実行
- [ ] タイル挙動テスト実行
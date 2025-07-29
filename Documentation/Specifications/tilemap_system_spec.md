# タイルマップシステム 仕様書

## 概要
2D縦スクロールゲームにおけるタイルマップベースの地形生成・管理システム。
プロシージャル生成によるマップ作成とスクロールシステムとの連携を実現する。

## 目的
- プロシージャル生成による無限の地形提供
- メモリ効率的なタイルマップ管理
- スムーズなスクロール体験の実現
- 再現可能なマップ生成（シード機能）

## 機能要件

- ✅ **タイルマップの動的生成**（正方形の2Dスプライトを隙間なく並べる）
- ✅ **プロシージャル地形生成**（基本版：3~5個のWallランダム配置）
- ✅ **スクロールに応じたタイル管理**（メモリ最適化・スクロール連携実装済み）
- ✅ **タイルマップのスクロール機能**
- ✅ **メモリ効率的なタイル破棄**
- ⚠️ **60FPSでの安定動作**（測定・検証未実装）
- ✅ **スプライトの切り替え** BlockTypeを切り替えたら適切なスプライトに切り替わる
  - ブロック用のプレハブは一つだけとなり、属性の値により表示するスプライトを切り替える

## 動作仕様

### マップ設計
- **マップサイズ**: 横15マス × 縦20マス
- **タイル単位**: Unity 2DSprite GameObjectとして配置
- **スプライトサイズ**: 64x64ピクセル
- **座標系**: Unity標準2D座標系（整数座標でタイル配置）

### レベル定義
- **レベル単位**: 1スクロール = 1レベル
- **スクロール量**: 15ブロック分（縦方向）

### 地形生成仕様
- **生成方式**: シードベースのランダム生成
- **配置パターン**: Groundベース + 限定的なWall配置
- **WallTile配置ルール**: 1レベル当たり3~5個のWallをランダム配置
- **隙間なし配置**: 15×20の全300マスにタイルを配置

### タイル属性仕様
- **Sky**: レベル1の上5マス用ブロック
- **Empty**: 何もない空間を表現するブロック
- **Ground**: 基本的な地面ブロック、Playerがヒットした場合Emptyタイルに変化
- **Rock**: 岩ブロック、Playerは通過できない、落下処理あり
- **Treasure**: お宝ブロック、PlayerとヒットしたらEmptyブロックに変わりスコア加算

### メモリ管理
- **アクティブ範囲**: 現在地 ± 2レベル分のタイル保持
- **非アクティブ化**: 範囲外タイルの破棄

### スクロール連携
- **トリガー検知**: スクロールシステムからの通知受信
- **新レベル生成**: スクロール開始時の次レベル準備
- **座標変換**: スクロール後の座標系調整

## テスト仕様
テスト実装の方針とルールについては、以下のドキュメントを参照してください：
- `Documentation/Rules/TestRule.md`

## 注意事項
- プロシージャル生成の乱数シードは保存・復元可能に設計
- スクロールシステムとの密な連携が必要

## ブロック作成手順

### 新方式（単一プレハブ + スプライト切り替え）

#### 1. 基本構成
- **UniversalTile.prefab**: 共通タイルプレハブ
- **TileController**: BlockType管理とスプライト切り替え
- **SpriteManager**: BlockType→Spriteマッピング管理

#### 2. ブロック設定手順

**Step 1: SpriteManagerの初期化**
```csharp
// スプライトマップを作成
var spriteMap = new Dictionary<BlockType, Sprite>
{
    { BlockType.Sky, skySprite },
    { BlockType.Ground, groundSprite },
    { BlockType.Rock, rockSprite },
    { BlockType.Treasure, treasureSprite }
};

// SpriteManagerを初期化
SpriteManager.Initialize(spriteMap);
```

**Step 2: UniversalTileの使用**
```csharp
// UniversalTileプレハブをインスタンス化
var tileInstance = Instantiate(universalTilePrefab, position, Quaternion.identity);

// TileControllerでBlockTypeを設定（自動的にスプライトが切り替わる）
var tileController = tileInstance.GetComponent<TileController>();
tileController.Initialize(BlockType.Ground);

// または、プロパティで直接設定
tileController.BlockType = BlockType.Rock;
```

**Step 3: 動的なスプライト変更**
```csharp
// ブロックタイプを変更（スプライトも自動更新）
tileController.BlockType = BlockType.Empty; // 非表示になる
tileController.BlockType = BlockType.Treasure; // 宝物スプライトに切り替わる
```

#### 3. 従来方式からの移行

**従来方式の問題点:**
- 各BlockTypeごとに個別プレハブが必要
- Dictionary<BlockType, GameObject>でプレハブ管理
- タイル変更時のDestroy→Instantiateオーバーヘッド

**新方式の利点:**
- 単一プレハブの再利用によるメモリ効率向上
- スプライト切り替えによるパフォーマンス向上
- 新BlockType追加時の作業軽減
- プレハブ管理の簡素化

#### 4. ファイル構成
```
Assets/MyGame/
├── Scripts/TilemapSystem/Core/
│   ├── TileController.cs          # タイル個別管理
│   ├── SpriteManager.cs           # スプライトマッピング
│   └── BlockType.cs               # ブロック種別定義
└── Prefabs/Tiles/
    └── UniversalTile.prefab       # 共通タイルプレハブ
```

#### 5. 注意事項
- SpriteManagerは使用前に必ず Initialize() で初期化する
- EmptyブロックはSpriteRendererが非アクティブになる
- 既存のITileBehavior、TileBehaviorとの互換性を維持

## 更新履歴
| 日付 | 変更内容 | 担当者 |
|------|----------|--------|
| 2025-07-13 | 初版作成（game_overview_spec.mdから分離） | Claude |
| 2025-07-13 | 基本機能実装完了、実装状況マーキング追加 | Claude |
| 2025-07-13 | Unityシーンでの実装方法を追記 | Claude |
| 2025-07-13 | TileBaseアセット作成方法を正しい手順に修正 | Claude |
| 2025-07-13 | TileBaseアセット作成をTile Palette使用手順に変更 | Claude |
| 2025-07-13 | Unity TilemapSystemから2DSprite Prefabシステムに大幅変更 | Claude |
| 2025-07-13 | ProceduralGenerator実装、セルラーオートマトン地形生成追加 | Claude |
| 2025-07-13 | 現在の実装状況に合わせて仕様書全体を更新・検証 | Claude |
| 2025-07-14 | ProceduralGeneratorをシンプルな実装に変更 | Claude |
| 2025-07-14 | 仕様書を現在の実装（全マスWall配置）に合わせて更新 | Claude |
| 2025-07-14 | 実装状況マーキングを正確な状態に修正、将来拡張項目を明記 | Claude |
| 2025-07-14 | チェック柄からGroundベース+ランダムWall配置に変更 | Claude |
| 2025-07-14 | WallTile配置ルールを3~5個制限に変更、仕様書を最新実装に更新 | Claude |
| 2025-07-14 | 実装状況分析、機能要件に実装状況マーキング（✅/⚠️/❌）追加 | Claude |
| 2025-07-22 | 仕様書をシンプル化、技術詳細・実装詳細を削除 | Claude |
| 2025-07-24 | 単一プレハブ+スプライト切り替え方式のブロック作成手順を追加 | Claude |
| 2025-07-29 | スプライト切り替え機能実装完了、包括的テストケース追加、パフォーマンステスト実装 | Claude |
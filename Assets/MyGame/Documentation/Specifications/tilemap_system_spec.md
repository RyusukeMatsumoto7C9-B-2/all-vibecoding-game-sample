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

### 基本機能
- タイルマップの動的生成( タイルマップは正方形の2Dスプライトを隙間なく並べる)
- プロシージャル地形生成
- スクロールに応じたタイル管理
- メモリ効率的なタイル破棄

### パフォーマンス要件
- 60FPSでの安定動作
- メモリ使用量の最適化
- スクロール時の遅延なし

## 技術仕様

### マップ設計
- **マップサイズ**: 横20マス × 縦30マス
- **タイル単位**: Unity Tilemap System準拠
- **座標系**: Unity標準座標系（原点左下）

### レベル定義
- **レベル単位**: 1スクロール = 1レベル
- **スクロール量**: 25マス分（縦方向）
- **重複エリア**: 5マス分（次レベルとの継続性確保）

### 地上エリア仕様
- **地上範囲**: 上端から5マス分
- **地面タイル**: 配置なし（空間として扱う）
- **目的**: プレイヤーの初期スポーン領域確保

### プロシージャル生成仕様
- **生成方式**: ランダムシード基盤
- **地形パターン**: 通路、壁、空間の組み合わせ
- **生成制約**: 
  - 最低限の通路確保
  - 行き止まりの制限
  - プレイヤー進行可能性の保証

## システム設計

### クラス構成
```
TilemapSystem/
├── Core/
│   ├── TilemapGenerator      # プロシージャル生成ロジック ✅実装済み
│   ├── TilemapManager        # タイル管理・操作 ✅実装済み
│   └── TileDefinition        # タイル種別定義 ✅実装済み
├── Generation/
│   ├── ProceduralGenerator   # 生成アルゴリズム ❌未実装
│   ├── TerrainPattern        # 地形パターン定義 ❌未実装
│   └── SeedManager           # シード管理 ✅実装済み
└── Management/
    ├── TileMemoryManager     # メモリ最適化 ❌未実装
    └── TilePooling           # オブジェクトプーリング ❌未実装
```

### データ構造
```csharp
// タイル種別定義
public enum TileType
{
    Empty,      // 空間
    Ground,     // 地面
    Wall,       // 壁
    Treasure,   // お宝
    Enemy       // エネミー配置位置
}

// マップデータ構造
public struct MapData
{
    public int Width;           // 横幅
    public int Height;          // 縦幅
    public TileType[,] Tiles;   // タイル配置
    public int Seed;            // 生成シード
    public int Level;           // レベル番号
}
```

## 実装仕様

### 生成処理フロー
1. **シード設定**: レベル番号から生成シード決定 ✅実装済み
2. **基本地形生成**: ベースとなる地形パターン作成 ✅実装済み（基本版）
3. **通路確保**: プレイヤー進行ルート保証 ✅実装済み（中央通路）
4. **詳細配置**: お宝・エネミー配置位置決定 ❌未実装
5. **タイル配置**: Unity Tilemapへの実際の配置 ✅実装済み

### メモリ管理
- **アクティブ範囲**: 現在地 ± 2レベル分のタイル保持 ✅実装済み
- **非アクティブ化**: 範囲外タイルの破棄 ✅実装済み
- **プリロード**: 次レベルの事前生成 ❌未実装

### スクロール連携
- **トリガー検知**: スクロールシステムからの通知受信 ❌未実装
- **新レベル生成**: スクロール開始時の次レベル準備 ❌未実装
- **座標変換**: スクロール後の座標系調整 ❌未実装

## インターフェース設計

### 主要メソッド
```csharp
// タイルマップ生成
MapData GenerateMap(int level, int seed);

// タイル配置
void PlaceTiles(MapData mapData);

// メモリ最適化
void OptimizeMemory(int currentLevel);

// シード管理
int GetSeedForLevel(int level);
void SetSeed(int baseSeed);
```

### イベント通知
```csharp
// マップ生成完了通知
event Action<MapData> OnMapGenerated;

// メモリ最適化完了通知
event Action<int> OnMemoryOptimized;
```

## テスト仕様

### テスト方針
- **TDD適用**: t-wada流によるテスト駆動開発
- **ユニットテスト**: PureC#クラスのロジックテスト
- **統合テスト**: Unity Tilemap連携テスト

### テスト項目

#### 1. プロシージャル生成テスト
- **生成一貫性**: 同一シードでの同一マップ生成確認
- **サイズ検証**: 指定サイズでのマップ生成確認
- **地上エリア**: 上5マスの空間確保確認

#### 2. メモリ管理テスト
- **メモリ使用量**: 指定範囲内でのメモリ使用確認
- **破棄処理**: 不要タイルの正常破棄確認
- **リーク検出**: メモリリークの有無確認

#### 3. 座標変換テスト
- **スクロール対応**: スクロール前後の座標正確性確認
- **境界値**: マップ端での座標処理確認

#### 4. パフォーマンステスト
- **生成速度**: マップ生成時間の測定
- **フレームレート**: 生成処理中のFPS維持確認

### テストクラス例
```csharp
[Description("タイルマップ生成システムのテスト")]
public class TilemapGeneratorTests
{
    [Test]
    [Description("指定サイズでマップが正しく生成されることを検証")]
    public void GenerateMap_WithSpecifiedSize_CreatesCorrectSizeMap()
    {
        // テスト実装
    }
}
```

## パフォーマンス最適化

### メモリ最適化戦略
- **オブジェクトプーリング**: 頻繁に生成・破棄されるタイルの再利用 ❌未実装
- **遅延生成**: 必要時のみのタイル生成 ✅実装済み（基本版）
- **バッチ処理**: 複数タイルの一括処理 ✅実装済み

### 処理速度最適化
- **非同期処理**: 重い生成処理の分散実行 ❌未実装
- **キャッシュ活用**: 生成済みパターンの再利用 ❌未実装
- **LOD適用**: 遠距離タイルの簡略化 ❌未実装

## セキュリティ考慮事項
- **シード検証**: 不正なシード値の排除
- **メモリ保護**: バッファオーバーフロー対策
- **入力検証**: パラメータの妥当性確認

## エラーハンドリング
- **生成失敗**: 代替パターンでの再生成
- **メモリ不足**: 緊急時のタイル強制破棄
- **座標エラー**: 範囲外アクセスの防止

## 将来拡張性
- **タイル種別追加**: 新しいタイルタイプの対応
- **生成アルゴリズム**: 複数生成方式の切り替え
- **動的サイズ**: 可変マップサイズの対応

## 依存関係
- **Unity Tilemap System**: 基盤システム
- **Unity Mathematics**: 数学計算ライブラリ
- **UniTask**: 非同期処理ライブラリ
- **VContainer**: 依存性注入フレームワーク

## Unityシーンでの実装方法

### 必要なGameObject構成
シーンに以下のGameObjectを配置してタイルマップシステムを動作させます：

```
TilemapSystem (空のGameObject)
├── Grid (GridコンポーネントをアタッチしたGameObject)
│   └── Tilemap (Tilemap + TilemapRenderer コンポーネントをアタッチしたGameObject)
└── TilemapSystemTester (TilemapSystemTesterコンポーネントをアタッチしたGameObject)
```

### セットアップ手順

#### 1. Grid と Tilemap の作成
1. Hierarchy で右クリック → 2D Object → Tilemap → Rectangular を選択
2. 自動的に Grid と Tilemap GameObjectが作成される

#### 2. TilemapSystemTester の設定
1. 空のGameObjectを作成し「TilemapSystemTester」と命名
2. TilemapSystemTesterコンポーネントをアタッチ
3. インスペクターで以下を設定：
   - **Tilemap**: 作成した Tilemap を参照
   - **Wall Tile**: 壁用のTileBaseアセット（例：Sprite-Default）
   - **Ground Tile**: 地面用のTileBaseアセット（例：別のSprite）
   - **Test Level**: テスト用レベル番号（初期値：1）
   - **Test Seed**: テスト用シード値（初期値：12345）

#### 3. TileBase アセットの準備

**方法1: Tileアセットを直接作成（推奨）**
1. Project ウィンドウで右クリック → Create → 2D → Tile
2. 作成されたTileアセットを選択し、Inspector で Sprite フィールドに使用したいSpriteを設定
3. 壁用と地面用のTileアセットをそれぞれ作成

**方法2: Spriteから自動生成**
1. Project ウィンドウで右クリック → Create → 2D → Sprites → Square（Spriteアセット作成）
2. 作成されたSpriteを選択し、Inspector で以下を設定：
   - Sprite Mode: Single
   - Pixels Per Unit: 1（タイルサイズに応じて調整）
3. Sprite選択状態でInspector下部の「Create Tile」ボタンをクリック
4. 壁用と地面用の異なる色のSpriteとTileをそれぞれ用意

**注意**: TilemapSystemで使用するのはTileBase（Tileアセット）であり、Spriteアセットではありません。

### 動作確認方法

#### 基本動作テスト
1. Play モードに入る
2. 自動的にタイルマップが生成される
3. Sceneビューでタイルの配置を確認

#### コンテキストメニューでのテスト
TilemapSystemTesterコンポーネントの右上メニュー（⋮）から：
- **新しいマップを生成**: 次のレベルのマップを生成
- **メモリ最適化テスト**: 不要なマップの削除をテスト

### 設定可能なパラメータ

#### TilemapSystemTester
- `tilemap`: タイル配置対象のTilemap
- `wallTile`: 壁タイプのTileBase
- `groundTile`: 地面タイプのTileBase
- `testLevel`: 生成するレベル番号
- `testSeed`: プロシージャル生成用シード値

### トラブルシューティング

#### タイルが表示されない場合
1. TilemapRendererコンポーネントが有効か確認
2. 使用するTileBaseが正しく設定されているか確認
3. Grid の Cell Size が適切か確認（デフォルト：1, 1, 1）

#### マップ生成に失敗する場合
1. Console ウィンドウでエラーメッセージを確認
2. TilemapSystemTester の必須フィールドが設定されているか確認
3. TileBase アセットが null でないか確認

### カメラ設定の推奨事項
- Projection: Orthographic
- Size: 10-15（マップサイズに応じて調整）
- Position: (10, 15, -10)（マップ中央が見える位置）

## 注意事項
- Unity TestRunnerはEditModeのみで実施
- PureC#での実装を優先し、テスト容易性を確保
- プロシージャル生成の乱数シードは保存・復元可能に設計
- スクロールシステムとの密な連携が必要

## 更新履歴
| 日付 | 変更内容 | 担当者 |
|------|----------|--------|
| 2025-07-13 | 初版作成（game_overview_spec.mdから分離） | Claude |
| 2025-07-13 | 基本機能実装完了、実装状況マーキング追加 | Claude |
| 2025-07-13 | Unityシーンでの実装方法を追記 | Claude |
| 2025-07-13 | TileBaseアセット作成方法を正しい手順に修正 | Claude |
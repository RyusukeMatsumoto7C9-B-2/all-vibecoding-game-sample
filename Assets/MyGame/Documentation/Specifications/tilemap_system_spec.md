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
- **タイル単位**: Unity 2DSprite GameObjectとして配置
  - **スプライトサイズ**: 64x64ピクセル
  - **スケール設定**: spritePixelsPerUnit: 64
  - **タイルの並べ方**: 2DSpriteを1Unityユニット間隔で並べる
- **座標系**: Unity標準2D座標系（整数座標でタイル配置）

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
│   ├── TilemapManager        # 2DSprite Prefabタイル管理・操作 ✅実装済み
│   └── TileDefinition        # タイル種別定義 ✅実装済み
├── Generation/
│   ├── ProceduralGenerator   # セルラーオートマトン生成アルゴリズム ✅実装済み
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
2. **プロシージャル地形生成**: ProceduralGeneratorによる高度な地形作成 ✅実装済み
   - セルラーオートマトンによる基本地形生成
   - メイン通路・横通路の確保
   - 接続性チェックと孤立エリアの解消
   - 地形の滑らか化処理
3. **詳細配置**: お宝・エネミー配置位置決定 ❌未実装
4. **タイル配置**: 2DSprite Prefabの動的生成による実際の配置 ✅実装済み
   - GameObject.Instantiate()による動的生成
   - Transform階層での親子関係管理
   - レベル別のタイル管理

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
- **統合テスト**: 2DSprite Prefabシステム連携テスト

### 実装済みテストクラス

#### 1. ProceduralGeneratorTests
- セルラーオートマトン地形生成の検証
- 通路確保と接続性テスト
- 同一シードでの一貫性確認
- カスタムサイズ対応テスト

#### 2. SpriteTilemapManagerTests  
- GameObject動的生成の検証
- タイル座標配置の正確性テスト
- メモリ管理（生成・削除）テスト
- レベル別管理機能テスト

#### 3. TilemapGeneratorTests
- 統合的なマップ生成テスト
- ProceduralGenerator連携テスト
- 地上エリア空間確保テスト

#### 4. SeedManagerTests
- シード管理機能テスト
- レベル別ランダム生成テスト

### テスト項目カバレッジ

#### ✅ 実装済み
- **生成一貫性**: 同一シードでの同一マップ生成確認
- **サイズ検証**: 指定サイズでのマップ生成確認
- **地上エリア**: 上5マスの空間確保確認
- **GameObject管理**: 動的生成・削除・座標配置テスト
- **メモリ最適化**: レベル別タイル管理テスト

#### ❌ 未実装
- **パフォーマンステスト**: 生成速度・FPS測定
- **スクロール連携テスト**: 座標変換・境界値処理
- **大規模テスト**: 長時間実行・メモリリーク検出

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

## プロジェクト構造

### アセット構成
```
Assets/MyGame/
├── Sprites/                    # 2DSprite画像アセット
│   ├── WallSprite.png         # 壁用32x32グレースプライト
│   └── GroundSprite.png       # 地面用32x32茶色スプライト
├── Prefabs/Tiles/             # GameObject Prefabアセット
│   ├── WallTile.prefab        # 壁タイル用Prefab (SpriteRenderer付き)
│   └── GroundTile.prefab      # 地面タイル用Prefab (SpriteRenderer付き)
├── Scripts/TilemapSystem/     # システム実装
│   ├── Core/                  # コア機能
│   ├── Generation/            # 生成ロジック
│   ├── Management/            # 管理機能（未実装）
│   ├── Tests/EditMode/        # テストコード
│   └── TilemapSystemTester.cs # シーンテスト用コンポーネント
└── Scripts/Editor/            # エディタ拡張
    ├── TilemapSetupEditor.cs  # セットアップ自動化ツール
    └── MyGame.Editor.asmdef   # エディタ用Assembly Definition
```

### 主要クラス詳細
- **TilemapGenerator**: ProceduralGeneratorを使用した高度な地形生成
- **TilemapManager**: GameObject Instantiate/Destroyによるタイル管理
- **ProceduralGenerator**: セルラーオートマトン + 通路確保 + 滑らか化
- **TilemapSystemTester**: GameObject Prefab参照でのシーンテスト

## 依存関係
- **Unity 2D Sprite System**: 基盤システム
- **Unity Mathematics**: 数学計算ライブラリ
- **UniTask**: 非同期処理ライブラリ
- **VContainer**: 依存性注入フレームワーク

## Unityシーンでの実装方法

### 必要なGameObject構成
シーンに以下のGameObjectを配置してタイルマップシステムを動作させます：

```
TilemapSystem (空のGameObject)
└── TilemapSystemTester (TilemapSystemTesterコンポーネントをアタッチしたGameObject)
    ├── (動的生成される2DSpriteタイル群)
    └── (レベル毎に生成・削除される)
```

### セットアップ手順

#### 1. TilemapSystemTester の作成
1. 空のGameObjectを作成し「TilemapSystemTester」と命名
2. TilemapSystemTesterコンポーネントをアタッチ

#### 2. タイル用Prefabの準備

**壁タイル用Prefab**
1. 空のGameObjectを作成し「WallTilePrefab」と命名
2. SpriteRendererコンポーネントをアタッチ
3. Spriteフィールドに壁用のSpriteを設定
4. Prefab化：ProjectウィンドウのPrefabsフォルダにドラッグ&ドロップ

**地面タイル用Prefab**
1. 空のGameObjectを作成し「GroundTilePrefab」と命名
2. SpriteRendererコンポーネントをアタッチ
3. Spriteフィールドに地面用のSpriteを設定
4. Prefab化：ProjectウィンドウのPrefabsフォルダにドラッグ&ドロップ

#### 3. TilemapSystemTester の設定
インスペクターで以下を設定：
- **Wall Tile Prefab**: 作成した壁用Prefabを参照
- **Ground Tile Prefab**: 作成した地面用Prefabを参照
- **Test Level**: テスト用レベル番号（初期値：1）
- **Test Seed**: テスト用シード値（初期値：12345）

#### 4. Sprite アセットの準備

**現在の実装状況**
1. **作成済みSprite**
   - Assets/MyGame/Sprites/WallSprite.png (32x32 グレー)
   - Assets/MyGame/Sprites/GroundSprite.png (32x32 茶色)
   - Pixels Per Unit: 100 (Unity標準設定)
   - Filter Mode: Point (ピクセルアート用)

2. **作成済みPrefab**
   - Assets/MyGame/Prefabs/Tiles/WallTile.prefab
   - Assets/MyGame/Prefabs/Tiles/GroundTile.prefab
   - 各Prefabは対応するSpriteを参照するSpriteRendererを持つ

**注意**: 新システムでは2DSprite Prefabを使用します。Unity Tilemapシステムは使用しません。

### 動作確認方法

#### 基本動作テスト
1. Play モードに入る
2. 自動的に2DSpriteタイルマップが生成される
3. Sceneビューでタイルの配置を確認
4. Hierarchyウィンドウで動的生成されたタイルGameObjectを確認

#### コンテキストメニューでのテスト
TilemapSystemTesterコンポーネントの右上メニュー（⋮）から：
- **新しいマップを生成**: 次のレベルのマップを生成
- **メモリ最適化テスト**: 不要なマップの削除をテスト

### 設定可能なパラメータ

#### TilemapSystemTester
- `wallTilePrefab`: 壁タイプのGameObject Prefab
- `groundTilePrefab`: 地面タイプのGameObject Prefab
- `testLevel`: 生成するレベル番号
- `testSeed`: プロシージャル生成用シード値

### トラブルシューティング

#### タイルが表示されない場合
1. SpriteRendererコンポーネントが有効か確認
2. 使用するPrefabが正しく設定されているか確認
3. Spriteアセットが正しくアサインされているか確認
4. PrefabのSpriteRendererにSpriteが設定されているか確認

#### マップ生成に失敗する場合
1. Console ウィンドウでエラーメッセージを確認
2. TilemapSystemTester の必須フィールドが設定されているか確認
3. Prefab アセットが null でないか確認
4. Prefabに必要なコンポーネント（SpriteRenderer）があるか確認

#### パフォーマンスの問題
- 大量のGameObjectが生成されるため、適切なメモリ最適化が重要
- 不要なレベルのタイルは適時削除される設計

### カメラ設定の推奨事項
- Projection: Orthographic
- Size: 10-15（マップサイズに応じて調整）
- Position: (10, 15, -10)（マップ中央が見える位置）

### システムの特徴
- **動的タイル生成**: 必要時にGameObjectとしてタイルを生成
- **レベル別管理**: レベル毎にタイルを管理し、メモリ効率を向上
- **プレハブベース**: 再利用可能なPrefabシステムを採用
- **座標系**: Unity標準の2D座標系を使用（原点スプライト中心）

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
| 2025-07-13 | TileBaseアセット作成をTile Palette使用手順に変更 | Claude |
| 2025-07-13 | Unity TilemapSystemから2DSprite Prefabシステムに大幅変更 | Claude |
| 2025-07-13 | ProceduralGenerator実装、セルラーオートマトン地形生成追加 | Claude |
| 2025-07-13 | 現在の実装状況に合わせて仕様書全体を更新・検証 | Claude |
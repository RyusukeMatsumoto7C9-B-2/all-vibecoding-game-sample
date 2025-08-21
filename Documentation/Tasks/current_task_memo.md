# 現在のタスク

（現在進行中のタスクはありません）

# アーカイブ済みタスク

## TilemapManager・TilemapSystemController責務分離リファクタリング（2025-01-21完了）

### 概要
TilemapSystemController（MonoBehaviour）とTilemapService（非MonoBehaviour）の責務が混在している問題を解決し、TilemapServiceを中心とした適切な依存性注入構造に統合する。

### 実装内容

#### Phase 1: ITilemapManagerインターフェース拡張（完了）
- OptimizeMemory、PlaceTilesWithOverlapProtection、GetTilesForLevelメソッドを追加
- System.Collections.Genericのusing追加

#### Phase 2: TilemapScrollControllerのインターフェース対応（完了）
- TilemapManager具象クラス依存をITilemapManagerインターフェース依存に変更
- コンストラクタとフィールドを修正

#### Phase 3: TilemapSystemControllerの責務分離（完了）
- ITilemapManager実装を削除し、純粋なMonoBehaviourに変更
- VContainer [Inject]によるITilemapManager依存性注入を追加
- プロキシメソッドを削除
- プロパティをITilemapManager型に変更

#### Phase 4: VContainer設定修正（完了）
- GameLifetimeScope.csでTilemapServiceをITilemapManagerとしてSingleton登録
- TilemapSystemControllerをコンポーネントとして登録

#### Phase 5: 命名規則違反の是正（完了）
- TilemapManagerクラスをTilemapServiceに改名（*Manager禁止ルール準拠）
- MockTilemapManagerをMockTilemapServiceに改名
- TilemapManagerCoordinateTestsをTilemapServiceCoordinateTestsに改名
- 全参照箇所の更新（テストコード含む）

### 解決した問題
1. **責務混在**: TilemapSystemController が MonoBehaviour でありながら ITilemapManager を実装
2. **プロキシ構造**: TilemapSystemController の全ITilemapManagerメソッドが _manager に委譲
3. **VContainer構成不適切**: GameLifetimeScope で TilemapSystemController を ITilemapManager として登録
4. **依存関係の複雑化**: TilemapScrollController が具象クラス TilemapManager に依存

### 達成した構造
1. **TilemapService**: 非MonoBehaviourとして全タイルマップビジネスロジックを担当
2. **TilemapSystemController**: Unity固有機能（自動スクロール、ContextMenu）のみ担当
3. **VContainer注入**: TilemapService を ITilemapManager として注入
4. **適切な依存分離**: インターフェース経由での依存関係

### 変更ファイル一覧
- `Assets/MyGame/Scripts/TilemapSystem/Core/ITilemapManager.cs` - インターフェース拡張
- `Assets/MyGame/Scripts/TilemapSystem/Core/TilemapService.cs` - TilemapManagerから改名
- `Assets/MyGame/Scripts/TilemapSystem/Core/TilemapScrollController.cs` - インターフェース依存
- `Assets/MyGame/Scripts/TilemapSystem/TilemapSystemController.cs` - 責務分離
- `Assets/MyGame/Scripts/TilemapSystem/MockTilemapService.cs` - Mockクラス改名
- `Assets/MyGame/Scripts/DI/GameLifetimeScope.cs` - DI設定更新
- `Assets/MyGame/Scripts/Enemy/EnemyController.cs` - インターフェース型使用
- 全テストファイル - MockTilemapService参照に更新

### 達成した効果
- **責務の明確化**: Unity固有機能とビジネスロジックの完全分離
- **テスタビリティ向上**: TilemapService の単体テストが容易
- **VContainer活用**: 適切な依存性注入による疎結合設計
- **保守性向上**: インターフェース経由による変更影響の局所化
- **命名規則準拠**: *Manager禁止ルールに従いTilemapServiceに改名

## Player-TilemapSystem連携改善タスク（2025-01-11完了）

### 概要
PlayerがTilemapSystemの新機能（座標変換・移動判定）を利用するよう変更し、VContainerでの依存性注入を実装する。

### 実装要件

#### ✅ 現在のPlayer実装の調査と分析（完了）
- [x] PlayerControllerの現在の実装を確認
- [x] PlayerMoveServiceの現在の実装を確認  
- [x] 現在のTilemapManagerとの依存関係を確認
- [x] VContainerの使用状況を確認（v1.16.9導入済み）

#### ✅ VContainer依存性注入の設計・実装（完了）
- [x] ITilemapManagerインターフェースの依存性注入設計
- [x] GameLifetimeScopeでのBinding設定 (GameLifetimeScope.cs)
- [x] PlayerController/PlayerMoveServiceでのITilemapManager注入
- [x] 既存のSetTilemapManager呼び出しをDI化

#### ✅ Player側での新機能利用への変更（完了）
- [x] 座標変換: GetPosition(int x, int y)メソッドの利用
- [x] 移動判定: CanPassThrough(Vector2Int position, int level)メソッドの利用
- [x] 既存のCanPlayerPassThroughからCanPassThroughへの移行
- [x] ハードコーディングされた座標計算の置き換え

#### ✅ テスト・検証（完了）
- [x] 既存のPlayerテストの更新（コンストラクタインジェクション対応）
- [x] 新機能利用のテストケース追加（CanPassThroughメソッドテスト）
- [x] MockTilemapManagerの新機能対応

### 技術詳細

#### VContainer依存性注入方針（達成済み）
1. **インターフェース注入**: ITilemapManagerをコンストラクタ注入（[Inject] Constructメソッド）
2. **ライフサイクル管理**: Singletonでの登録完了
3. **初期化順序**: TilemapSystemController -> PlayerController（GameLifetimeScope経由）

#### 移行対象メソッド（達成済み）
- `GetPosition(int x, int y): Vector3` - PlayerController座標変換で利用開始
- `CanPassThrough(Vector2Int position, int level)` - PlayerMoveService移動制約で利用開始

### 実装完了ファイル
- `Assets/MyGame/Scripts/DI/GameLifetimeScope.cs` - VContainer設定（新規作成）
- `Assets/MyGame/Scripts/Player/PlayerController.cs` - DI対応・新機能利用
- `Assets/MyGame/Scripts/Player/PlayerMoveService.cs` - コンストラクタDI・新機能利用
- `Assets/MyGame/Scripts/Player/Tests/EditMode/PlayerMoveServiceTests.cs` - テスト更新
- `Assets/MyGame/Scripts/TilemapSystem/TilemapSystemController.cs` - 不要メソッド削除

### 成果
- **依存性注入**: 手動設定からVContainer DIへ完全移行
- **新機能利用**: GetPositionとCanPassThroughメソッドをPlayerで利用開始
- **ブロック通過仕様統一**: Sky(不可)、Empty(可)、Ground(可)、Rock(不可)、Treasure(可)
- **テスト整理**: TilemapSystemとPlayerテスト間の重複を削除し、責任を整理
- **テスト対応**: 21個の既存テスト+2個の新機能テストケース追加

## エネミーシステム開発（完了）

### ✅ フェーズ1: 基本移動システム（2025-01-22完了）
- EnemyController、EnemyMoveService、EnemyMovementConstraint実装
- Player/Enemy間でDirection.cs共通化
- 岩ブロック通過不可、マップ境界制限実装
- 単体テスト・統合テスト完了

### ✅ フェーズ2: レベルベース出現管理（2025-01-09完了）
- EnemySpawner、EnemySpawnConfig実装
- レベル別出現数計算機能（レベル1:5体、5レベル毎+1、上限10体）
- 画面境界外出現位置計算
- PlayModeテスト完了

## 参考資料
- `Documentation/Specifications/enemy_spec.md`
- `Documentation/Rules/CSharpCodingRule.md`
- `Documentation/Rules/TestRule.md`
# エネミー上下左右移動システム実装タスク

## 概要
enemy_spec.mdの機能要件「上下左右移動システム」を実装する。
プレイヤー移動システムと同様の構造でエネミーの基本移動機能を作成。

## 実装タスク

### 1. Enemyディレクトリ構造作成
- [x] `Assets/MyGame/Scripts/Enemy/` ディレクトリ作成
- [x] `Assets/MyGame/Scripts/Enemy/Tests/EditMode/` ディレクトリ作成

### 2. コアクラス実装
- [x] `Direction.cs` をPlayerから共通場所に移動・共通化
- [x] `EnemyController.cs` 基本制御クラス実装
- [x] `EnemyMoveService.cs` 移動ロジック実装
- [x] `EnemyMovementConstraint.cs` 移動制約実装

### 3. グリッドベース移動システム
- [x] 4方向移動（上下左右）対応
- [x] 1マス単位のグリッド移動実装
- [x] 岩ブロック通過不可制限
- [x] マップ境界での移動制限

### 4. テスト実装
- [x] `EnemyMoveServiceTests.cs` 単体テスト
- [x] `EnemyMovementConstraintTests.cs` 制約テスト
- [x] 移動制約の統合テスト

### 4.5. コンパイルエラー修正
- [x] PlayerMovementConstraintIntegrationTests.csにMyGame.Common.Directionのusing追加
- [x] EnemyテストクラスでMockTilemapManagerの参照エラー修正
- [x] PlayerController.csのFindObjectsOfType非推奨警告修正
- [x] MockTilemapManagerのusing修正

### 5. Prefab作成
- [x] 64x64スプライト.png作成ツールをClaudeCodeToolsディレクトリに実装
- [x] Enemy.prefab 作成
- [x] 基本スプライト設定（Unity側で自動調整）
- [x] コンポーネント設定（EnemyController、BoxCollider2D、Rigidbody2D）
- [x] EnemyControllerスクリプト参照修正

## 仕様詳細
- **移動方向**: 上下左右の4方向のみ（斜め移動は不可）
- **移動方式**: グリッドベース移動（1マス単位）
- **移動制限**: 岩ブロック通過不可、マップ外への移動不可

## 参考資料
- `Documentation/Specifications/enemy_spec.md`
- 既存のPlayerシステム実装
- `Documentation/Rules/CSharpCodingRule.md`
- `Documentation/Rules/TestRule.md`

## 実装状況

### ✅ フェーズ1: エネミー上下左右移動システム実装完了 (2025-01-22)

#### 基本移動システム実装完了
- EnemyController, EnemyMoveService, EnemyMovementConstraint実装完了
- Direction.csをCommonディレクトリに共通化完了
- 単体テスト実装完了
- コミット完了: 07c8b66

#### スプライト作成ツール実装完了
- 64x64スプライト.png作成ツール実装完了
- ClaudeCodeToolsディレクトリに配置
- コミット完了: b52b475

#### コンパイルエラー修正完了
- using文の修正完了
- FindObjectsOfType非推奨警告の修正完了
- MockTilemapManagerの参照エラー修正完了
- コミット完了: 41c8b8f

#### Enemy.prefab作成完了
- Enemy.prefab作成完了
- EnemyControllerスクリプト参照修正完了
- BoxCollider2D、Rigidbody2D設定完了
- スプライト設定完了（Unity側で自動設定）

## 次のステップ
- [x] Enemy.prefab作成完了（2025-01-22）
- [x] Enemy.prefabのスクリプト参照修正完了
- [ ] AI追跡システム実装
- [ ] ランダム移動AI実装  
- [ ] レベルベース出現管理実装
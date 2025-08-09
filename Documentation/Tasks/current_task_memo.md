# エネミーシステム開発タスク

## 概要
enemy_spec.mdの機能要件を段階的に実装する。現在は基本移動システムが完了し、次段階としてレベルベース出現管理システムの実装を進める。

## 実装完了項目（アーカイブ）

### ✅ フェーズ1: エネミー基本移動システム（2025-01-22完了）
- **基本移動機能**: EnemyController、EnemyMoveService、EnemyMovementConstraint実装
- **共通化対応**: Direction.csをPlayer/Enemy間で共通化
- **移動制約**: 岩ブロック通過不可、マップ境界制限
- **テスト**: 単体テスト・統合テスト実装
- **Prefab**: Enemy.prefab作成・コンポーネント設定
- **コミット**: 07c8b66, b52b475, 41c8b8f

## 現在の実装対象

### 🔄 フェーズ2: レベルベース出現管理システム

メモ : 次作業開始時、PlanModeで各コアシステムに必要なふるまいについてPlanModeで壁打ちしながら肉付け

### 2.1 テストケース作成 ( ユーザ側作業 )
**進捗**: EnemySpawnConfig・EnemySpawnerテスト実装完了（2025-01-09完了）
- [x] **EnemySpawnerTests.cs** - エネミー出現統合管理クラステスト
  - [X] クラス基本構造
    - [X] MonoBehaviourベースクラス
    - [X] 名前空間: MyGame.Enemy.PlayModeTests（命名規約準拠）
  - [X] プロパティ
    - [X] `ActiveEnemyCount` - 現在生きているエネミー数
      - [X] 初期値が0であることを確認
      - [X] エネミー生成後にカウントが増加することを確認
      - [X] エネミー削除後にカウントが減少することを確認
      - [X] 破棄されたエネミーが自動的にカウントから除外されることを確認
  - [X] 出現管理メソッド
    - [X] `SpawnEnemiesForLevel(int level)` - レベル別エネミー一括生成
      - [X] レベル1で5体生成されることを確認
      - [X] レベル5で5体生成されることを確認
      - [X] レベル6で6体生成されることを確認
      - [X] レベル26以上で10体（上限）生成されることを確認
      - [X] 負のレベル値でエラーを発出し、エネミーの生成を行わないことを確認する
    - [X] `ClearAllEnemies()` - 全エネミー削除
      - [X] エネミーが存在しない状態での実行が安全であることを確認
  - [X] テスト品質改善
    - [X] TestCase互換性問題修正（UnityTest+TestCaseの組み合わせエラー解決）
    - [X] テスト命名規則適用（意味のあるメソッド名に変更）
    - [X] Description属性追加（日本語説明による可読性向上）
    - [X] テストシーン設定（EnemySpawnerコンポーネント・EnemyContainer設定）
- [x] **EnemySpawnConfigTest.cs** - 出現設定データクラステスト（2025-01-09完了）
  - [x] ScriptableObject実装
    - [x] 名前空間: MyGame.Enemy.Spawn
    - [x] CreateAssetMenu属性の追加
      - [x] エディタメニューから作成可能であることを確認
      - [x] 作成されたアセットが正しく保存されることを確認
  - [x] パブリックメソッドの実装
    - [x] `GetEnemyCountForLevel(int level)` - レベル別出現数計算
      - [x] レベル1で5を返すことを確認
      - [x] レベル5で5を返すことを確認
      - [x] レベル6で6を返すことを確認
      - [x] レベル10で6を返すことを確認
      - [x] レベル11で7を返すことを確認
      - [x] レベル26以上で10（上限）を返すことを確認
      - [x] 0以下のレベルで最小値（BaseEnemyCount）を返すことを確認
      - [x] カスタム設定値での計算が正しいことを確認
  - [x] **コミット**: 1c78090, e7ed479, 34014bf, f67cdec, 909554a

#### 2.2 コアシステム設計・実装
**進捗**: EnemySpawnConfig・EnemySpawner実装完了（2025-01-09完了）
- [x] **EnemySpawner.cs** - エネミー出現統合管理クラス
  - [x] クラス基本構造
    - [x] MonoBehaviourベースクラス
    - [x] 名前空間: MyGame.Enemy.Spawn
  - [x] プロパティ
    - [x] `ActiveEnemyCount` - 現在生きているエネミー数（nullチェック対応）
  - [x] コンポーネント参照
    - [x] `[SerializeField] private GameObject enemyPrefab` - エネミーPrefab
    - [x] `[SerializeField] private EnemySpawnConfig spawnConfig` - 出現設定
    - [x] `[SerializeField] private Transform enemyContainer` - エネミー格納親オブジェクト
  - [x] 出現管理メソッド
    - [x] `SpawnEnemiesForLevel(int level)` - レベル別エネミー一括生成（エラーハンドリング対応）
    - [x] `ClearAllEnemies()` - 全エネミー削除
  - [x] 追加実装機能
    - [x] `CalculateSpawnPosition()` - 画面左右境界外への出現位置計算
    - [x] `activeEnemies` - アクティブエネミーリスト管理
    - [x] エラーハンドリング（負のレベル値、null参照チェック）
- [x] **EnemySpawnConfig.cs** - 出現設定データクラス作成（2025-01-09完了）
  - [x] ScriptableObject実装
    - [x] 名前空間: MyGame.Enemy.Spawn
    - [x] CreateAssetMenu属性の追加
  - [x] 設定パラメータ定義
    - [x] `BaseEnemyCount` - レベル1基本出現数（5匹）
    - [x] `EnemyIncreaseInterval` - 増加間隔（5レベル毎）
    - [x] `EnemyIncreaseAmount` - 増加数（+1匹）
    - [x] `MaxEnemyCount` - 最大出現数（10匹）
    - [x] `SpawnAreaMargin` - 画面境界外余白距離
    - [x] `MinSpawnDistanceFromPlayer` - 生成時プレイヤーとの最小距離
  - [x] パブリックメソッドの実装
    - [x] `GetEnemyCountForLevel(int level)` - レベル別出現数計算
  - [x] インターフェース実装（IEnemySpawnConfig）

#### 2.3 出現位置・タイミング制御
- [x] **出現位置計算システム** - 画面境界外座標生成機能実装（EnemySpawner.CalculateSpawnPositionで実装済み）
- [ ] **出現タイミング制御** - レベル開始時の出現トリガー実装
- [x] **エネミー生成処理** - Prefabインスタンス化・初期化機能実装（EnemySpawner.SpawnEnemiesForLevelで実装済み）

#### 2.4 画面外処理・テスト
- [ ] **画面外デス処理** - 強制スクロール連携・削除処理実装
- [x] **統合テスト** - PlayModeテストによる全体動作確認完了（EnemySpawnerTests.cs）
- [ ] **パフォーマンス最適化** - オブジェクトプール・メモリ管理改善

## 今後の実装予定

### フェーズ3: AI動作システム（未着手）
- [ ] プレイヤー追跡AI（距離3マス以下で発動）
- [ ] ランダム移動AI（追跡条件外での基本行動）
- [ ] マンハッタン距離計算

## 技術仕様
- **出現位置**: 画面左右境界外
- **出現数**: レベル1で5匹、5レベル毎に+1（上限10匹）
- **デス条件**: 強制スクロールによる画面外退場
- **移動方式**: グリッドベース移動（1マス単位、4方向）
- **移動制限**: 岩ブロック通過不可、マップ境界制限

## 参考資料
- `Documentation/Specifications/enemy_spec.md`
- `Documentation/Rules/CSharpCodingRule.md`
- `Documentation/Rules/TestRule.md`
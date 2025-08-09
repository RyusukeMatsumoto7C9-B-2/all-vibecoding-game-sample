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
- [ ] **EnemySpawnerTest.cs** - エネミー出現統合管理クラステスト
  - [ ] クラス基本構造
    - [ ] MonoBehaviourベースクラス
    - [ ] 名前空間: MyGame.Enemy.Spawn
  - [ ] プロパティ
    - [ ] `ActiveEnemyCount` - 現在生きているエネミー数
      - [ ] 初期値が0であることを確認
      - [ ] エネミー生成後にカウントが増加することを確認
      - [ ] エネミー削除後にカウントが減少することを確認
      - [ ] 破棄されたエネミーが自動的にカウントから除外されることを確認
  - [ ] 出現管理メソッド
    - [ ] `SpawnEnemiesForLevel(int level)` - レベル別エネミー一括生成
      - [ ] レベル1で5体生成されることを確認
      - [ ] レベル6で6体生成されることを確認
      - [ ] レベル11で7体生成されることを確認
      - [ ] レベル26以上で10体（上限）生成されることを確認
      - [ ] 負のレベル値でエラーが発生しないことを確認
      - [ ] 既存エネミーがある状態での生成動作を確認
    - [ ] `SpawnEnemy(Vector3 position)` - 個別エネミー生成
      - [ ] 指定位置にエネミーが生成されることを確認
      - [ ] 生成されたエネミーがActiveEnemyCountに反映されることを確認
      - [ ] enemyPrefabがnullの場合のエラーハンドリングを確認
      - [ ] OnEnemySpawnedイベントが発火することを確認
    - [ ] `ClearAllEnemies()` - 全エネミー削除
      - [ ] 全てのエネミーが削除されることを確認
      - [ ] ActiveEnemyCountが0になることを確認
      - [ ] エネミーが存在しない状態での実行が安全であることを確認
  - [ ] イベント
    - [ ] `UnityEvent<GameObject> OnEnemySpawned` - エネミー生成時
      - [ ] エネミー生成時に正しく発火することを確認
      - [ ] イベント引数に生成されたGameObjectが渡されることを確認
      - [ ] 複数のリスナーが正しく呼ばれることを確認
  - [ ] ヘルパーメソッド
    - [ ] `RemoveDestroyedEnemies()` - 破棄済みエネミーをリストから削除
      - [ ] null参照が正しく削除されることを確認
      - [ ] 有効なエネミーは削除されないことを確認
      - [ ] ActiveEnemyCountが正しく更新されることを確認
    - [ ] `ValidateSpawnPosition(Vector3 position)` - 出現位置の妥当性検証
      - [ ] 有効な位置でtrueを返すことを確認
      - [ ] プレイヤーに近すぎる位置でfalseを返すことを確認
      - [ ] マップ外の位置でfalseを返すことを確認
- [ ] **EnemySpawnConfigTest.cs** - 出現設定データクラステスト
  - [] ScriptableObject実装
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

#### 2.2 コアシステム設計・実装
- [ ] **EnemySpawner.cs** - エネミー出現統合管理クラス
  - [ ] クラス基本構造
    - [ ] MonoBehaviourベースクラス
    - [ ] 名前空間: MyGame.Enemy.Spawn
  - [ ] プロパティ
    - [ ] `ActiveEnemyCount` - 現在生きているエネミー数
  - [ ] コンポーネント参照
    - [ ] `[SerializeField] GameObject enemyPrefab` - エネミーPrefab
    - [ ] `[SerializeField] EnemySpawnConfig spawnConfig` - 出現設定
    - [ ] `[SerializeField] Transform enemyContainer` - エネミー格納親オブジェクト ( 将来的にEnemyManager クラスにに引き渡す形にする )
  - [ ] 出現管理メソッド
    - [ ] `SpawnEnemiesForLevel(int level)` - レベル別エネミー一括生成
    - [ ] `SpawnEnemy(Vector3 position)` - 個別エネミー生成
    - [ ] `ClearAllEnemies()` - 全エネミー削除
  - [ ] 出現計算メソッド : private 要素
    - [ ] `CalculateEnemyCount(int level)` - 出現数計算（L1:5匹、5L毎+1、上限10匹）
    - [ ] `GetSpawnPositions(int count)` - 出現位置座標リスト生成
    - [ ] `GetRandomSpawnPosition()` - ランダム出現位置取得
  - [ ] イベント
    - [ ] `UnityEvent<GameObject> OnEnemySpawned` - エネミー生成時
  - [ ] ヘルパーメソッド
    - [ ] `RemoveDestroyedEnemies()` - 破棄済みエネミーをリストから削除
    - [ ] `ValidateSpawnPosition(Vector3 position)` - 出現位置の妥当性検証
- [ ] **EnemySpawnConfig.cs** - 出現設定データクラス作成
  - [ ] ScriptableObject実装
    - [ ] 名前空間: MyGame.Enemy.Spawn
    - [ ] CreateAssetMenu属性の追加
  - [ ] 設定パラメータ定義
    - [ ] `BaseEnemyCount` - レベル1基本出現数（5匹）
    - [ ] `EnemyIncreaseInterval` - 増加間隔（5レベル毎）
    - [ ] `EnemyIncreaseAmount` - 増加数（+1匹）
    - [ ] `MaxEnemyCount` - 最大出現数（10匹）
    - [ ] `SpawnAreaMargin` - 画面境界外余白距離
    - [ ] `MinSpawnDistanceFromPlayer` - 生成時プレイヤーとの最小距離
  - [ ] パブリックメソッドの実装
    - [ ] `GetEnemyCountForLevel(int level)` - レベル別出現数計算

#### 2.3 出現位置・タイミング制御
- [ ] **出現位置計算システム** - 画面境界外座標生成機能実装
- [ ] **出現タイミング制御** - レベル開始時の出現トリガー実装
- [ ] **エネミー生成処理** - Prefabインスタンス化・初期化機能実装

#### 2.4 画面外処理・テスト
- [ ] **画面外デス処理** - 強制スクロール連携・削除処理実装
- [ ] **統合テスト** - 全体動作確認・エッジケーステスト実装
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
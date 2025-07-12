# エネミー 仕様書

## 概要
地下ダンジョンに生息するモンスターで、プレイヤーを追跡・妨害する存在。レベルの進行とともに数が増え、ゲームの難易度を調整する重要な要素。

## 目的
- プレイヤーに適度な緊張感と挑戦を提供する
- レベル進行による難易度上昇システムを実現する
- 戦略的な移動を促すゲームプレイを創出する

## 要件

### 機能要件
- 上下左右移動システム
- プレイヤー追跡AI
- ランダム移動AI
- レベルベース出現管理
- 画面外デス処理

### 非機能要件
- 効率的なAI処理
- 大量エネミー管理
- メモリ効率的な生成・破棄

## 技術仕様

### 移動システム
- **移動方向**: 上下左右の4方向のみ（斜め移動は不可）
- **移動方式**: グリッドベース移動（1マス単位）
- **移動制限**: 岩ブロック通過不可、マップ外への移動不可
- **移動速度**: [要調整] ミリ秒/マス

### 出現システム
- **出現位置**: 画面左右の境界外
- **出現数管理**: 
  - レベル1: 5匹
  - レベル6: 6匹（5レベル毎に+1）
  - 上限: 10匹（レベル26以降）
- **デス条件**: 強制スクロールによる画面外退場（スコア加算なし）

### AI動作システム
- **追跡モード**: プレイヤーとの距離が3マス以下で発動
- **ランダムモード**: 追跡条件外での基本行動
- **距離計算**: マンハッタン距離（|x1-x2| + |y1-y2|）

## 実装詳細

### 実装手順
1. エネミー基底クラスの作成
2. AI状態管理システムの実装
3. 出現管理システムの実装
4. プレイヤー追跡ロジックの実装
5. ランダム移動ロジックの実装
6. 画面外デス処理の実装

### 依存関係
- Unity Tilemap System
- VContainer（依存性注入）
- R3（リアクティブシステム）

### クラス設計
```
EnemyManager (MonoBehaviour)
├── EnemySpawnService (PureC#)
├── EnemyController (MonoBehaviour)
│   ├── EnemyAIService (PureC#)
│   ├── EnemyMoveService (PureC#)
│   └── EnemyAnimationController (MonoBehaviour)
└── EnemyCountManager (PureC#)
```

### 主要メソッド
- `SpawnEnemy(Vector2Int position)`: エネミー生成
- `CalculateEnemyCount(int level)`: レベル別出現数計算
- `UpdateAI()`: AI状態更新
- `MoveTowardsPlayer(Vector2Int playerPos)`: プレイヤー追跡移動
- `MoveRandomly()`: ランダム移動
- `CheckScreenBounds()`: 画面外判定

### AI状態遷移
```
ランダム移動モード
    ↓ （プレイヤー距離 ≤ 3マス）
追跡モード
    ↓ （プレイヤー距離 > 3マス）
ランダム移動モード
```

## テスト仕様

### テスト実装方針
- t-wada流TDDを適用
- Unity TestRunnerを使用
- PureC#クラスの重点的なテスト

### テスト項目
1. **出現システムテスト**
   - レベル別出現数の正確性確認
   - 上限値の制御確認
   - 出現位置の妥当性確認

2. **AI動作テスト**
   - 距離計算の正確性確認
   - 追跡モード切り替えの確認
   - ランダム移動の動作確認

3. **移動システムテスト**
   - 4方向移動の正確性確認
   - 岩ブロック通過制限の確認
   - マップ境界制限の確認

4. **画面外処理テスト**
   - デス判定の正確性確認
   - スコア加算なしの確認

### テストクラス例
```csharp
[TestFixture]
public class EnemyAIServiceTests
{
    [Test]
    public void CalculateDistance_WithValidPositions_ShouldReturnManhattanDistance()
    
    [Test]
    public void ShouldChasePlayer_WithinRange_ShouldReturnTrue()
    
    [Test]
    public void MoveTowardsPlayer_ValidPath_ShouldMoveCloser()
}

[TestFixture]
public class EnemySpawnServiceTests
{
    [Test]
    public void CalculateEnemyCount_Level1_ShouldReturn5()
    
    [Test]
    public void CalculateEnemyCount_Level26_ShouldReturn10()
}
```

## 注意事項
- TestRunner は EditMode のみを実施する（PlayMode のテストはユーザが手動で実施する）
- 可能な限り PureC# で実装し、TestRunnerでのテストを充実させる
- シングルトンパターンは使用禁止、VContainerを使用する
- AI処理は毎フレーム実行せず、適切な間隔で更新する
- エネミー同士の衝突・重複は考慮しない（同一マス占有可能）
- 追跡時の経路探索は単純な方向移動とし、複雑なパスファインディングは実装しない

## 更新履歴
| 日付 | 変更内容 | 担当者 |
|------|----------|--------|
| 2025-07-12 | 初版作成 | Claude |
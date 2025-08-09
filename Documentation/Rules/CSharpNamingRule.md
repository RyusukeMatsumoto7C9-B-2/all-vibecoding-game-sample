# C#命名規則（コーディング実装時用）

このドキュメントは、Unityゲームプロジェクトにおけるコーディング実装時の命名規則を定義します。

## 共通ルール
- Hoge, Fuga, a, b 等の意味のない名前は禁止
- int_value 等のハンガリアン記法は禁止
- 英語で命名し、日本語（ローマ字）は使用しない

## 名前空間
- MyGame をルートとしてPascalケースで記述
- 英語の単数形として命名する
  - ~Systems ではなく ~System となる
- 機能群としての名前を記載
  - 例: MyGame.Player, MyGame.ScoreSystem, MyGame.TilemapSystem

## 型の命名規則

### クラス名
- PascalCaseで記述
- 端的にそのクラスの責務が特定できる名前とする
- *Manager等のクラス名は単一責任の原則を破りやすい命名のため禁止
- 例: PlayerController, ScoreCalculator, EnemySpawner

### 構造体名
- PascalCaseで記述
- 端的にその構造体が管理するデータが特定できる名前とする
- 例: PlayerData, ScoreInfo, Vector2Int

### インターフェース名
- PascalCaseで記述
- 先頭に I を付ける
- 例: IMovable, IScoreProvider, IDamageable

### 列挙型
- PascalCaseで記述
- 端的にその列挙型が何を表すかを特定できる名前とする
- 各要素の名前もPascalCaseで記述
- 例:
```csharp
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}
```

## 定数の命名規則

### public定数
- PascalCaseで記述
- static readonly を使用
- 例: public static readonly int MaxPlayerHealth = 100;

### private定数
- static readonly を利用する場合は PascalCase で記述
- const を利用する場合は ALL_UPPER_CASE で記述
- 例:
  - private static readonly int DefaultSpeed = 5;
  - private const string DEFAULT_NAME = "Player";

## メンバーの命名規則

### プロパティ名
- PascalCaseで記述
- bool値のプロパティの場合は肯定系の名前とする
- 例: IsValid, HasData, CanMove, CurrentHealth

### フィールド名
- 先頭に_プリフィックスを付けたcamelCaseで記述
- bool値のフィールドの場合は肯定系の名前とする
- 例: _isValid, _hasData, _playerSpeed, _currentScore

### メソッド名
- PascalCaseで記述
- 動詞 + 名詞 or 動詞 + 具体的な処理内容 で記述
- 例: MovePlayer(), CalculateScore(), TransitionScene(), UpdateHealth()

## 変数の命名規則

### メソッド引数
- camelCaseで記述
- 例: playerName, damageAmount, isEnabled

### ローカル変数
- プリフィックスのないcamelCaseで記述
- ごく限られたスコープ内でならvalue, temp, resultなどのシンプルな名前を許可
- 例: playerPosition, totalScore, isGameOver

### ループ変数
- 単純なループではi, j, kを許可
- foreach文では意味のある名前を使用
- 例:
```csharp
for (int i = 0; i < count; i++) { }
foreach (var enemy in enemies) { }
```

## 命名例

```csharp
namespace MyGame.Player
{
    public interface IPlayerController
    {
        bool CanMove { get; }
        void MovePlayer(Vector2 direction);
    }

    public class PlayerController : IPlayerController
    {
        public static readonly int MaxHealth = 100;
        
        private const string DEFAULT_NAME = "Player";
        
        public bool CanMove { get; private set; }
        
        private float _moveSpeed;
        private bool _isGrounded;
        
        public void MovePlayer(Vector2 direction)
        {
            var normalizedDirection = direction.normalized;
            // 処理
        }
        
        private void UpdatePosition(float deltaTime)
        {
            // 処理
        }
    }
}   

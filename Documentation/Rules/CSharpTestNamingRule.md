# C#テスト命名規則（テスト構築時用）

このドキュメントは、Unityプロジェクトにおけるテスト構築時の命名規則を定義します。

## 名前空間規則

### EditModeテスト
- 名前空間は `MyGame.機能名.Tests` の形式で統一する
- 例: `MyGame.TilemapSystem.Tests`、`MyGame.Player.Tests`、`MyGame.Enemy.Tests`

### PlayModeテスト
- 名前空間は `MyGame.機能名.PlayModeTests` の形式で統一する
- 例: `MyGame.Player.PlayModeTests`、`MyGame.UI.PlayModeTests`

## テストクラス命名規則

### 基本ルール
- テスト対象クラス名 + `Tests` の形式で命名する
- PascalCaseで記述
- 例:
  - `PlayerController` → `PlayerControllerTests`
  - `ScoreCalculator` → `ScoreCalculatorTests`
  - `EnemySpawner` → `EnemySpawnerTests`

### 特定機能のテストクラス
- 機能が複雑な場合は、機能名を含めたクラス名にする
- 例:
  - `PlayerMovementTests`（プレイヤーの移動機能）
  - `PlayerCombatTests`（プレイヤーの戦闘機能）
  - `ScoreCalculationTests`（スコア計算機能）

## テストメソッド命名規則

### 基本フォーマット
```
Test_{テスト対象メソッド or プロパティ}_{任意:引き渡す引数}_{期待する結果}
```

### 命名パターン例

#### 通常のメソッドテスト
```csharp
[Test]
public void Test_CalculateScore_WhenEnemyDefeated_ReturnsCorrectPoints()
[Test]
public void Test_MovePlayer_WithNegativeSpeed_ThrowsArgumentException()
[Test]
public void Test_Initialize_WhenCalledTwice_DoesNotResetValues()
```

#### プロパティのテスト
```csharp
[Test]
public void Test_IsAlive_WhenHealthIsZero_ReturnsFalse()
[Test]
public void Test_MaxHealth_WhenSet_UpdatesCurrentHealth()
```

#### 条件付きテスト
```csharp
[Test]
public void Test_Attack_WhenTargetIsNull_ThrowsNullReferenceException()
[Test]
public void Test_Jump_WhenIsGrounded_IncreasesYVelocity()
[Test]
public void Test_Save_WhenFileExists_OverwritesData()
```

## パラメータ化テストの命名

### TestCaseを使用する場合
```csharp
[Test]
[TestCase(0, 0)]
[TestCase(5, 25)]
[TestCase(-3, 9)]
public void Test_Square_WithVariousInputs_ReturnsCorrectResult(int input, int expected)
```

### ValueSourceを使用する場合
```csharp
private static int[] TestValues = { 1, 2, 3, 4, 5 };

[Test]
public void Test_IsPositive_WithPositiveNumbers_ReturnsTrue([ValueSource(nameof(TestValues))] int value)
```

## ヘルパーメソッド・フィールドの命名

### SetUp/TearDownメソッド
```csharp
[SetUp]
public void SetUp()
[TearDown]
public void TearDown()
[OneTimeSetUp]
public void OneTimeSetUp()
[OneTimeTearDown]
public void OneTimeTearDown()
```

### テスト用フィールド
- 通常のフィールドと同様に`_`プリフィックスを付ける
```csharp
private PlayerController _playerController;
private IInputProvider _mockInputProvider;
private List<Enemy> _testEnemies;
```

### ヘルパーメソッド
- 通常のprivateメソッドと同様にPascalCaseで記述
- Create、Setup、Arrange、Build等のプレフィックスを使用
```csharp
private PlayerController CreatePlayerWithHealth(int health)
private void SetupMockInput(float horizontal, float vertical)
private List<Enemy> ArrangeEnemies(int count)
private ScoreData BuildTestScoreData()
```

## テストデータ・定数の命名

### テスト用定数
```csharp
private const int DEFAULT_TEST_HEALTH = 100;
private const string TEST_PLAYER_NAME = "TestPlayer";
private static readonly Vector2 TestStartPosition = new Vector2(0, 0);
```

### Expected値
```csharp
var expectedScore = 150;
var expectedPosition = new Vector2(10, 5);
var expectedMessage = "Game Over";
```

## 完全な命名例

```csharp
namespace MyGame.Player.Tests
{
    [Description("プレイヤーコントローラーの機能をテストする")]
    public class PlayerControllerTests
    {
        private const int DEFAULT_MAX_HEALTH = 100;
        private static readonly Vector2 DefaultStartPosition = Vector2.zero;
        
        private PlayerController _playerController;
        private IInputProvider _mockInputProvider;
        private IPhysicsEngine _mockPhysicsEngine;
        
        
        [SetUp]
        public void SetUp()
        {
            _mockInputProvider = Substitute.For<IInputProvider>();
            _mockPhysicsEngine = Substitute.For<IPhysicsEngine>();
            _playerController = CreateDefaultPlayerController();
        }
        
        
        [Test]
        [Description("ダメージを受けた時にHPが正しく減少することを検証")]
        public void Test_TakeDamage_WithPositiveAmount_DecreasesHealth()
        {
            // Arrange
            var initialHealth = 100;
            var damageAmount = 30;
            var expectedHealth = 70;
            
            // Act
            _playerController.TakeDamage(damageAmount);
            
            // Assert
            Assert.AreEqual(expectedHealth, _playerController.CurrentHealth);
        }
        
        
        [Test]
        [Description("HPが0の時に死亡状態になることを検証")]
        public void Test_IsAlive_WhenHealthIsZero_ReturnsFalse()
        {
            // Arrange
            _playerController.SetHealth(0);
            
            // Act
            var result = _playerController.IsAlive;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        
        private PlayerController CreateDefaultPlayerController()
        {
            return new PlayerController(
                _mockInputProvider,
                _mockPhysicsEngine,
                DEFAULT_MAX_HEALTH,
                DefaultStartPosition
            );
        }
        
        
        private PlayerController CreatePlayerWithHealth(int health)
        {
            var controller = CreateDefaultPlayerController();
            controller.SetHealth(health);
            return controller;
        }
    }
}
```

## 注意事項

- テストメソッド名は長くなっても、何をテストしているかが明確にわかるようにする
- 略語は避け、完全な単語を使用する
- 日本語は使用せず、英語で命名する（Description属性内は日本語可）
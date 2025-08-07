# C#テスト設計ルール（テスト構築時用）

このドキュメントは、Unityプロジェクトにおけるテスト構築時の設計規約を定義します。

## 基本方針

### テストの原則
- 全てのPureC#クラスに対してテストを作成する
- 1つのテストは1つの振る舞いのみを検証する
- テストは独立して実行可能であること（他のテストに依存しない）
- テストは高速に実行できること

### テストの種類
- EditModeTest: Unityエディタ内で実行されるテスト（主にロジックテスト）
- PlayModeTest: Unity実行環境で実行されるテスト（主に統合テスト）

## テストクラス構造

### 必須属性
- 全てのテストクラスには`[Description("")]`属性を付与し、日本語でテストの対象と目的を記述する
- 全てのテストメソッドには`[Description("")]`属性を付与し、日本語でテストの概要を記述する
- テストの意図と期待結果が日本語で明確に理解できるようにする

### テストパターン
- AAA（Arrange-Act-Assert）パターンを使用する
  - Arrange: テストの準備
  - Act: テスト対象の実行
  - Assert: 結果の検証

## クラス内宣言ルール

### フィールドの宣言
- フィールドは _ プリフィックスをつけ極力 private readonly として宣言する
- セットアップで初期化するフィールドは private のみとする

### コードフォーマット
- メソッド間の空行は2行開けるものとする

## クラス内宣言順序

テストクラス内の宣言は以下の優先度順に上から記述する：

1. public定数
2. private定数
3. enum定義
4. publicプロパティ
5. privateプロパティ
6. privateフィールド
7. SetUpメソッド
8. TearDownメソッド
9. publicテストメソッド
10. privateヘルパーメソッド

## 依存関係の管理

### Mockの使用
- 依存関係は可能な限りMockクラスを注入する
- NSubstituteを使用してMockを作成する
- インターフェースを通じてMockを注入する

### テストダブル
- Stub: 固定値を返すダミーオブジェクト
- Mock: 呼び出しを記録・検証するオブジェクト
- Fake: 簡易実装を持つオブジェクト

## テスト実装例

```csharp
namespace MyGame.Player.Tests
{
    [Description("プレイヤーの移動機能をテストする")]
    public class PlayerMovementTests
    {
        private PlayerController _playerController;
        private IInputProvider _mockInputProvider;
        private IPhysicsEngine _mockPhysicsEngine;
        
        
        [SetUp]
        public void SetUp()
        {
            _mockInputProvider = Substitute.For<IInputProvider>();
            _mockPhysicsEngine = Substitute.For<IPhysicsEngine>();
            _playerController = new PlayerController(_mockInputProvider, _mockPhysicsEngine);
        }
        
        
        [TearDown]
        public void TearDown()
        {
            _playerController = null;
            _mockInputProvider = null;
            _mockPhysicsEngine = null;
        }
        
        
        [Test]
        [Description("上方向への移動時に座標のY値が1増加することを検証")]
        public void Test_MoveUp_WhenCalled_IncreasesYCoordinateByOne()
        {
            // Arrange
            var initialPosition = new Vector2(0, 0);
            _playerController.SetPosition(initialPosition);
            _mockInputProvider.GetVerticalAxis().Returns(1f);
            
            // Act
            _playerController.ProcessInput();
            _playerController.UpdatePosition();
            
            // Assert
            Assert.AreEqual(1, _playerController.Position.y);
            _mockPhysicsEngine.Received(1).ApplyForce(Arg.Any<Vector2>());
        }
        
        
        [Test]
        [Description("移動速度が0の時に位置が変化しないことを検証")]
        public void Test_Move_WhenSpeedIsZero_DoesNotChangePosition()
        {
            // Arrange
            var initialPosition = new Vector2(5, 5);
            _playerController.SetPosition(initialPosition);
            _playerController.SetSpeed(0);
            
            // Act
            _playerController.MoveUp();
            
            // Assert
            Assert.AreEqual(initialPosition, _playerController.Position);
        }
        
        
        private PlayerController CreatePlayerControllerWithDefaults()
        {
            return new PlayerController(
                Substitute.For<IInputProvider>(),
                Substitute.For<IPhysicsEngine>()
            );
        }
    }
}
```

## テストの実行

### Unity Test Runner
- UnityNaturalMCPからRunEditModeTestsコマンドで実施
- EditModeのみ実施 ( PlayModeはユーザが実施 )
- 特定のテストクラス・メソッドのみを選択して実行可能

### コマンドライン実行
- CI/CDパイプラインでの自動実行に対応

## 注意事項

- テストコードもプロダクションコードと同等の品質を保つ
- テストの可読性を重視し、複雑なロジックは避ける
- テストが失敗した時に原因が明確にわかるようにする
- 外部リソース（ファイル、ネットワーク）に依存しない
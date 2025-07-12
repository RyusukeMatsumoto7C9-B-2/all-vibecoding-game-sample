# アーキテクチャ 仕様書

## 概要
このプロジェクトは、テスタビリティと保守性を重視したクリーンアーキテクチャを採用する。
UI層はMV(R)P設計、依存性注入はVContainer、リアクティブプログラミングはR3を使用し、
可能な限りPureC#でロジックを実装してUnity TestRunnerでのテストを充実させる。

## 目的
- 高いテスタビリティを持つコードベースの構築
- 疎結合で拡張性の高いアーキテクチャの実現
- Unity依存を最小限に抑えたロジック層の構築

## 要件

### 機能要件
- MV(R)P設計によるUI層の実装
- VContainerを使用した依存性注入
- PureC#によるロジック層の実装
- R3によるリアクティブプログラミング

### 非機能要件
- シングルトンパターンの使用禁止
- 高いテストカバレッジの維持
- Unity依存の最小化

## 技術仕様

### アーキテクチャパターン
- **UI設計**: MV(R)P（Model-View-(Reactive)Presenter）
- **依存性注入**: VContainer
- **リアクティブ**: R3（ReactiveX for Unity）
- **テスティング**: Unity TestRunner（EditModeのみ）

### レイヤー構成
```
┌─────────────────┐
│   View Layer    │ ← MonoBehaviour, uGUI
├─────────────────┤
│ Presenter Layer │ ← MonoBehaviour, R3
├─────────────────┤
│  Service Layer  │ ← PureC#, Business Logic
├─────────────────┤
│ Repository Layer│ ← PureC#, Data Access
└─────────────────┘
```

### 主要フレームワーク
- **uGUI**: UI実装
- **VContainer**: 依存性注入コンテナ
- **R3**: リアクティブプログラミング
- **Unity Input System**: 入力処理
- **Unity Tilemap**: マップシステム

## 実装詳細

### MV(R)P設計
```csharp
// View（MonoBehaviour）
public class GameUIView : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Text scoreText;
    
    public Observable<Unit> OnStartButtonClicked => 
        startButton.OnClickAsObservable();
}

// Presenter（MonoBehaviour）
public class GameUIPresenter : MonoBehaviour
{
    [Inject] private readonly IGameService gameService;
    private GameUIView view;
    
    private void Start()
    {
        view.OnStartButtonClicked
            .Subscribe(_ => gameService.StartGame());
    }
}

// Service（PureC#）
public class GameService : IGameService
{
    private readonly IScoreRepository scoreRepository;
    
    public GameService(IScoreRepository scoreRepository)
    {
        this.scoreRepository = scoreRepository;
    }
}
```

### 依存性注入設計
```csharp
public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // Services
        builder.Register<IGameService, GameService>(Lifetime.Singleton);
        builder.Register<IPlayerService, PlayerService>(Lifetime.Singleton);
        builder.Register<IEnemyService, EnemyService>(Lifetime.Singleton);
        
        // Repositories
        builder.Register<IScoreRepository, ScoreRepository>(Lifetime.Singleton);
        builder.Register<ITilemapRepository, TilemapRepository>(Lifetime.Singleton);
        
        // Factories
        builder.Register<IEnemyFactory, EnemyFactory>(Lifetime.Singleton);
    }
}
```

### フォルダ構成
```
Assets/MyGame/
├── Scripts/
│   ├── UI/                          # UI関連（MV(R)P設計）
│   │   ├── Views/                   # View層
│   │   ├── Presenters/              # Presenter層
│   │   ├── Components/              # 再利用可能UIコンポーネント
│   │   └── Tests/                   # UIテスト
│   │       ├── EditMode/
│   │       └── PlayMode/
│   │
│   ├── GameSystem/                  # ゲームロジック
│   │   ├── Player/                  # プレイヤー機能
│   │   │   └── Tests/
│   │   │       ├── EditMode/
│   │   │       └── PlayMode/
│   │   ├── Enemy/                   # 敵機能
│   │   │   └── Tests/
│   │   │       ├── EditMode/
│   │   │       └── PlayMode/
│   │   ├── Game/                    # ゲーム全体管理
│   │   │   └── Tests/
│   │   │       ├── EditMode/
│   │   │       └── PlayMode/
│   │   ├── Data/                    # データ管理
│   │   │   └── Tests/
│   │   │       └── EditMode/
│   │   └── Shared/                  # 共通ユーティリティ
│   │       └── Tests/
│   │           └── EditMode/
│   │
│   └── Infrastructure/              # 基盤設定
│       ├── DI/                      # VContainer設定
│       ├── Interfaces/              # 共通インターフェース
│       └── Tests/
│           └── EditMode/
```

### 実装手順
1. ドメインモデルの作成（PureC#）
2. リポジトリインターフェースの定義（PureC#）
3. サービス層の実装（PureC#）
4. VContainerの設定とDI構成
5. View層の実装（MonoBehaviour + uGUI）
6. Presenter層の実装（MonoBehaviour + R3）
7. ユニットテストの実装

### 依存関係
- VContainer（DI Container）
- R3（Reactive Extensions）
- Unity Input System
- Unity TestRunner
- UniTask（非同期処理）

## テスト仕様

### テスト実装方針
- t-wada流TDDを適用
- Unity TestRunnerを使用（EditModeのみ）
- PureC#クラスを重点的にテスト
- モックオブジェクトを使用した単体テスト

### テスト対象
1. **ドメインモデル**
   - ビジネスルールの検証
   - 状態変更の正確性確認

2. **サービス層**
   - ビジネスロジックの動作確認
   - 境界値テスト

3. **リポジトリ実装**
   - データアクセスの正確性確認
   - エラーハンドリングの確認

### テストクラス例
```csharp
[TestFixture]
public class PlayerServiceTests
{
    private IPlayerService playerService;
    private Mock<ITilemapRepository> tilemapRepositoryMock;
    
    [SetUp]
    public void Setup()
    {
        tilemapRepositoryMock = new Mock<ITilemapRepository>();
        playerService = new PlayerService(tilemapRepositoryMock.Object);
    }
    
    [Test]
    public void MovePlayer_ValidPosition_ShouldUpdatePosition()
    {
        // Arrange, Act, Assert
    }
}
```

## 注意事項
- TestRunner は EditMode のみを実施する（PlayMode のテストはユーザが手動で実施する）
- 可能な限り PureC# で実装し、TestRunnerでのテストを充実させる
- シングルトンパターンは使用禁止、VContainerを使用する
- MonoBehaviourには最小限のロジックのみを記述し、ほとんどのロジックはPureC#で実装する
- UIのSerializeFieldは必ずprivateとし、[SerializeField]属性を使用する
- R3のObservableを適切に管理し、メモリリークを防ぐ

## 設計原則
- **SOLID原則の遵守**
- **関心の分離**
- **依存関係逆転の原則**
- **テスタビリティファースト**
- **Unity依存の最小化**

## 更新履歴
| 日付 | 変更内容 | 担当者 |
|------|----------|--------|
| 2025-07-12 | 初版作成 | Claude |
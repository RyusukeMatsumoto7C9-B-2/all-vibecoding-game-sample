# コーディングルール

このドキュメントは、Unity 2Dゲームプロジェクトにおけるコーディング規約と実装方針を定義します。

## 基本方針

### PureC#クラスの原則
- 重要なロジックはPureC#のクラスで作成し、UnityTestRunnerでテストを行える実装とする
  - ロジック内でUnityのコンポーネントを利用する場合はコンストラクタまたはVContainerにて参照を取得する
- PureC#で構築されたクラスは必ずUnity TestRunner でテストが行える形にする

### 設計パターン
- シングルトンクラスを作成しない
  - VContainerを利用した置換可能な設計とする
- UI部分の設計は R3 を用いた MV(R)P の設計とする
  - UIは原則としPresenter,View以外はPureC#で構築し、UnityTestRunnerでテストが実施できるようにする
  - Viewで参照を紐づけるコンポーネントはpublicフィールドではなくprivateフィールドに[SerializeField]属性をつけることで実現する
- イベント購読系処理は R3 を利用し、UnityEvent や event を利用しない

## クラス内宣言ルール
1. public で公開する定数は public static readonly として定義する ( PascalCase )  
2. const 定数は private でのみ利用できる ( ALL_UPPER_CASE )  
3. フィールドは _ プリフィックスをつけ極力 private readonly として宣言する   
4. メソッド間の空行は2行開けるものとする


## クラス内宣言順序

クラス内の宣言は以下の優先度順に上から記述する：

1. public定数  
2. private定数
3. enum定義
4. publicプロパティ
5. privateプロパティ
6. privateフィールド
7. Unityイベント
8. publicメソッド
9. privateメソッド

## 実装例

```csharp
public class ExampleClass
{
    // public定数
    public readonly int MaxValue = 100;
    
    // private定数
    private const string DefaultName = "Default";
    
    // enum定義
    public enum StateType
    {
        Active,
        Inactive
    }
    
    // publicプロパティ
    public int Value { get; private set; }
    
    // privateプロパティ
    private bool IsInitialized { get; set; }
    
    // privateフィールド
    [SerializeField] private GameObject targetObject;
    
    // Unityイベント
    void Start()
    {
        Initialize();
    }
    
    // publicメソッド
    public void Initialize()
    {
        // 初期化処理
    }
    
    // privateメソッド
    private void InternalProcess()
    {
        // 内部処理
    }
}
```

## 注意事項

- コンストラクタインジェクションを優先し、フィールドインジェクションは避ける
- 依存関係は可能な限りインターフェースを通して注入する
- Unityのライフサイクルメソッドに直接ビジネスロジックを記述しない
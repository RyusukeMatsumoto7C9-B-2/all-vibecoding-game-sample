# C#設計ルール（コーディング実装時用）

このドキュメントは、Unityプロジェクトにおけるコーディング実装時の設計規約を定義します。

## 基本設計方針

### PureC#クラスの原則
- 重要なロジックはPureC#のクラスで作成し、UnityTestRunnerでテストを行える実装とする
  - ロジック内でUnityのコンポーネントを利用する場合はコンストラクタまたはVContainerにて参照を取得する
- PureC#で構築されたクラスは必ずUnity TestRunner でテストが行える形にする
- テストを実施しやすい形にするために適切にinterfaceを作成する

### 単一責任の原則
- 一つのクラスには一つの責務とし、複数の責務を持つクラスを作成しない
- *Manager等のクラス名は単一責任の原則を破りやすい命名のため禁止

## ファイル構造

### ファイル単位
ファイルの単位は1クラス、構造体、列挙型につき1ファイルとする

### 例外規則
- クラス内でしか利用しない列挙型はクラス内宣言可能
- エディタ拡張コードはクラス内クラスとして記述することは可能
  - その場合 #if UNITY_EDITOR で囲い、ビルド成果物に含まれないようにする
  - ただしクラス内クラス、クラス内クラスによるエディタ拡張は必要最小限のものとする

```csharp
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MyClass 
{
    // クラス実装

#if UNITY_EDITOR
    private class MyClassEditor : UnityEditor
    {
        // エディタ拡張実装
    }
#endif
}
```

## 設計パターン

### 依存性注入
- シングルトンクラスを作成しない
- VContainerを利用した置換可能な設計とする
- コンストラクタインジェクションを優先し、フィールドインジェクションは避ける
- 依存関係は可能な限りインターフェースを通して注入する

### UI設計
- UI部分の設計は R3 を用いた MV(R)P の設計とする
- UIは原則としPresenter,View以外はPureC#で構築し、UnityTestRunnerでテストが実施できるようにする
- Viewで参照を紐づけるコンポーネントはpublicフィールドではなくprivateフィールドに[SerializeField]属性をつけることで実現する

### イベント処理
- イベント購読系処理は R3 を利用し、UnityEvent や event を利用しない

## クラス内宣言ルール

### 定数の宣言
- public定数は必ず public static readonly を利用する
- private定数は private static readonly を利用する
- const定数は極力利用せず、コンパイル時定数が必要な場合にのみ利用する
  - const定数は private 以外のスコープでは利用しない

### フィールドの宣言
- フィールドは _ プリフィックスをつけ極力 private readonly として宣言する

### コードフォーマット
- メソッド間の空行は2行開けるものとする
- メソッド、フィールド、プロパティのアクセス指定子は必ず明記する

## クラス内宣言順序

クラス内の宣言は以下の優先度順に上から記述する：

1. public定数
2. private定数
3. クラス内enum定義
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
    public static readonly int MaxValue = 100;
    
    // private定数
    private static readonly int MaxCount = 100;
    private const string DEFAULT_NAME = "Default";
    
    // クラス内enum定義
    private enum StateType
    {
        Active,
        Inactive
    }
    
    // publicプロパティ
    public int Value { get; private set; }
    
    // privateプロパティ
    private bool IsInitialized { get; set; }
    
    // privateフィールド
    [SerializeField] private GameObject _targetObject;
    private readonly int _hp;
    
    // Unityイベント
    private void Start()
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

- Unityのライフサイクルメソッドに直接ビジネスロジックを記述しない
- regionによるグルーピングは行わない
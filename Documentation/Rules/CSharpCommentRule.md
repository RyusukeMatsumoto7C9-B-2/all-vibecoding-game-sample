# C#コメント規則

このドキュメントは、Unityプロジェクトにおけるコメント規則を定義します。

## 基本方針

- コメントは必要最小限とし、コードが自己説明的になるよう命名と構造を工夫する
- コメントを書く場合は、「なぜ」を説明し、「何を」は説明しない
- 日本語でコメントを記述する

## コメントが必要な場合

### 複雑なビジネスロジック
- 複雑な計算式や判定ロジックの意図を説明
```csharp
// ダメージ計算式: 基礎ダメージ × (1 + クリティカル倍率) × 属性相性
var damage = baseDamage * (1 + criticalMultiplier) * elementalAffinity;
```

### 回避策・特殊処理
- バグ回避や特殊な処理の理由を記載
```csharp
// Unity2022.3のバグ回避: Collider2Dの初期化タイミングを遅延
yield return null;
```

### TODOコメント
- 将来の改善点や未実装機能を記載
```csharp
// TODO: パフォーマンス改善 - オブジェクトプーリングの実装
```

## コメントが不要な場合

### 自明なコード
```csharp
// 悪い例
// プレイヤーのHPを100に設定
playerHealth = 100;

// 良い例（コメント不要）
playerHealth = MaxHealth;
```

### getter/setter
```csharp
// 悪い例
/// <summary>
/// プレイヤーの名前を取得または設定します
/// </summary>
public string PlayerName { get; set; }

// 良い例（コメント不要）
public string PlayerName { get; set; }
```

## XMLドキュメントコメント

### publicメソッド・プロパティ
- 外部から使用されるpublicメンバーには必要に応じてXMLドキュメントコメントを記載
```csharp
/// <summary>
/// 指定されたダメージをプレイヤーに与える
/// </summary>
/// <param name="amount">ダメージ量</param>
/// <param name="ignoreArmor">防御力を無視するか</param>
/// <returns>実際に与えたダメージ量</returns>
public int TakeDamage(int amount, bool ignoreArmor = false)
{
    // 実装
}
```

### インターフェース
- インターフェースのメンバーには必ずXMLドキュメントコメントを記載
```csharp
public interface IHealth
{
    /// <summary>
    /// 現在のHP
    /// </summary>
    int CurrentHealth { get; }
    
    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="amount">ダメージ量</param>
    void TakeDamage(int amount);
}
```

## コメントのフォーマット

### 単行コメント
```csharp
// コメント内容
```

### 複数行コメント
```csharp
// 長いコメントは
// 複数行に分けて
// 記載する
```

### インラインコメント
```csharp
var result = CalculateScore(); // 特別な理由がある場合のみ使用
```

## テストコードのコメント

### Description属性
- テストクラスとテストメソッドには必ず`[Description("")]`属性を付与
```csharp
[Description("プレイヤーの移動機能をテストする")]
public class PlayerMovementTests
{
    [Test]
    [Description("上方向への移動時に座標のY値が増加することを検証")]
    public void Test_MoveUp_IncreasesYCoordinate()
    {
        // テスト実装
    }
}
```

## 禁止事項

- コメントアウトしたコードを残さない
- 古いコメントを更新せずに放置しない
- 英語と日本語を混在させない（コード例は除く）
- 冗長な説明コメントを書かない
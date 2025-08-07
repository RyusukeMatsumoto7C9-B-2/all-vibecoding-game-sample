# テストコーディングルール

このドキュメントは、Unityプロジェクトにおけるテストコーディング規約と実装方針を定義します。

## 基本方針

## クラス内宣言ルール
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
7. publicメソッド
8. privateメソッド

## 必須属性
- 全てのテストメソッドには`[Description("")]`属性を付与し、日本語でテストの概要を記述する
- テストクラス自体にも`[Description("")]`属性を付与し、そのクラスがテストする対象と目的を記述する
- テストの意図と期待結果が日本語で明確に理解できるようにする

## 名前空間規則
- EditModeテストクラスの名前空間は `MyGame.機能名.Tests` の形式で統一する
- 例: `MyGame.TilemapSystem.Tests`、`MyGame.Player.Tests`
- PlayModeテストクラスの名前空間も同様の形式を使用する

## テストメソッド記述ルール
- Test_{テスト対象メソッド or プロパティ}**_{任意:引き渡す引数**}_{期待する結果} となる形で記述する


## 記述例
```csharp
namespace MyGame.Player.Tests
{
    [Description("プレイヤーの移動機能をテストする")]
    public class PlayerMovementTests
    {
    [Test]
    [Description("上方向への移動時に座標のY値が1増加することを検証")]
    public void Test_MoveUp_WhenCalled_IncreasesYCoordinateByOne()
    {
        // Arrange
        var player = new Player(new Vector2(0, 0));
        
        // Act
        player.MoveUp();
        
        // Assert
        Assert.AreEqual(1, player.Position.y);
    }
    }
}
```

## 注意事項
- 依存関係は可能な限りMockクラスを注入する

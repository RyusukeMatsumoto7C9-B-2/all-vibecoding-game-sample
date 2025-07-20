# テストルール

このドキュメントは、Unity 2Dゲームプロジェクトにおけるテスト実装の方針とルールを定義します。

## テスト実装の基本方針（t-wada流）

### TDDサイクル
1. **Red**: 失敗するテストを書く
2. **Green**: テストを通す最小限の実装
3. **Refactor**: リファクタリング

### 実装アプローチ
- 小さなステップで進める
- 仮実装（ベタ書き）から始める
- 三角測量で一般化する
- 明白な実装が分かる場合は直接実装してもOK
- テストリストを常に更新する
- 不安なところからテストを書く

## テスト記述ルール

### 必須属性
- 全てのテストメソッドには`[Description("")]`属性を付与し、日本語でテストの概要を記述する
- テストクラス自体にも`[Description("")]`属性を付与し、そのクラスがテストする対象と目的を記述する
- テストの意図と期待結果が日本語で明確に理解できるようにする

### 記述例
```csharp
[Description("プレイヤーの移動機能をテストするクラス")]
public class PlayerMovementTests
{
    [Test]
    [Description("上方向への移動時に座標のY値が1増加することを検証")]
    public void MoveUp_WhenCalled_IncreasesYCoordinateByOne()
    {
        // Arrange
        var player = new Player(new Vector2(0, 0));
        
        // Act
        player.MoveUp();
        
        // Assert
        Assert.AreEqual(1, player.Position.y);
    }
}
```

## テストファイル構成

### フォルダ構造
```
Assets/MyGame/Scripts/[機能名]/
├── Tests/
│   ├── EditMode/        # EditModeテスト
│   │   ├── [機能名]Tests.cs
│   │   └── [機能名]Tests.asmdef
│   └── PlayMode/        # PlayModeテスト
│       ├── [機能名]PlayTests.cs
│       └── [機能名]PlayTests.asmdef
```

### EditModeテスト
- 各機能フォルダ/Tests/EditModeフォルダに構築する
- PureC#ロジックのテストを実装する
- Unityのライフサイクルに依存しない処理のテスト

### PlayModeテスト
- 各機能フォルダ/Tests/PlayModeフォルダに構築する
- PlayModeテストはテスト仕様書をタスク報告書に記述し、ユーザが手動で構築するものとする
- Unityのライフサイクルや実際のゲームオブジェクトを必要とするテスト

## アセンブリ定義

### 配置原則
- .asmdefファイルは各機能ごとではなく、各機能のEditModeまたはPlayModeのフォルダに配置する
- アセンブリの過度な分割を避け、適切な粒度でまとめる

### 命名規則
- EditMode: `[ProjectName].[機能名].Tests.EditMode`
- PlayMode: `[ProjectName].[機能名].Tests.PlayMode`

## リファクタリングルール

### 基本方針
- リファクタリングはMartin Fowlerの提唱する手法で実施する
- リファクタリング後のテストコード検証はユーザが実行する

### リファクタリング手順
1. テストが全て通ることを確認
2. 小さな変更を加える
3. テストを再実行して確認
4. 必要に応じてテストケースを追加

## テスト実行

### 実行責任
- テストの実行はユーザが行う
- Unity Test Runnerを使用してテストを実行
- CI/CDパイプラインでの自動実行も可能

### 実行タイミング
- 機能実装後
- リファクタリング後
- プルリクエスト作成前
- 定期的な品質チェック時
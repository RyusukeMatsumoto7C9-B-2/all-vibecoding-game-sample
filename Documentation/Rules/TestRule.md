# テストルール

## テストのコーディングルール
`Documentation/Rules/CSharpTestCodingRule.md` を参照

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
- **重要**: EditModeテストではGameObject、Transform、その他UnityEngineオブジェクトの使用を禁止する
- GameObjectを利用するテストは不安定で実行タイミングによって成否にブレが生じるので禁止する
- GameObjectが必要なテストはPlayModeテストとして実装する

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
- テストの実行はMCPサーバを通じてRunEditModeTestsで実行する

### 実行タイミング
- 機能実装後
- リファクタリング後
- プルリクエスト作成前
- 定期的な品質チェック時

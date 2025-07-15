# CLAUDE.md

このファイルは、このリポジトリでコードを扱う際のClaude Code (claude.ai/code)向けのガイダンスを提供します。

## プロジェクト概要

これはUnity 6000.0.51f1で構築されたUnity 2Dゲームプロジェクトで、Model Context Protocol (MCP)サーバー機能を統合しています。このプロジェクトはUniversal Render Pipeline (URP)を使用し、Input System、非同期操作用のUniTask、NuGetパッケージ管理などの最新のUnityパッケージを含んでいます。

## 主要なアーキテクチャコンポーネント

### Unityプロジェクト構造
```
Assets/MyGame/
├── Scripts/           # コアゲームロジック
├── Scenes/           # ゲームシーン (Sample)
├── Documentation/     # プロジェクトドキュメント
```

# 開発ルール
- Assets/MyGame/Documentation フォルダ以下のタスク用文書を参照し、作業を開始する
- 機能ごとに適切にフォルダを分け、各機能フォルダ内にTestsフォルダを作成しテストを実装する
  - EditModeテストは各機能フォルダ/Tests/EditModeフォルダに構築する
  - PlayModeテストは各機能フォルダ/Tests/PlayModeフォルダに構築する
    - PlayModeテストはテスト仕様書をタスク報告書に記述し、ユーザが手動で構築するものとする
  - この構成により機能単位での.unitypackage化と再利用性を向上させる
  - .asmdefファイルは各機能ごとではなく、各機能のEditModeまたはPlayModeのフォルダに配置しアセンブリの過度な分割を避ける
- 作業を開始する際は適切なブランチを作成し、作業を行い作業完了後プルリクエストを作成する
- 日本語で受け答えをする
- 絵文字は使用禁止
- ユーザからの指示や仕様に疑問があれば作業を中断し、質問すること
- 強制追加など -f コマンドは禁止
- 適切なタイミングでコミットを実行すること
- コミットのコメントは日本語で行うこと

## コーディングルール
- 重要なロジックはPureC#のクラスで作成し、UnityTestRunnerでテストを行える実装とする
  - ロジック内でUnityのコンポーネントを利用する場合はコンストラクタまたはVContainerにて参照を取得する
- PureC#で構築されたクラスは必ずUnity TestRunner でテストが行える形にする
- シングルトンクラスを作成しない
  - VContainerを利用した置換可能な設計とする
- UI部分の設計は R3 を用いた MV(R)P の設計とする
  - UIは原則としPresenter,View以外はPureC#で構築し、UnityTestRunnerでテストが実施できるようにする
  - Viewで参照を紐づけるコンポーネントはpublicフィールドではなくprivateフィールドに[SerializeField]属性をつけることで実現する
- クラス内の宣言は以下の優先度順に上から記述する
  - public定数 ( public定数は必ず public readonly で宣言 )
  - private定数
  - enum定義
  - publicプロパティ
  - privateプロパティ
  - privateフィールド
  - Unityイベント
  - publicメソッド
  - privateメソッド

### テストルール テストコードの作成は以下の方針 ( t-wada流 ) で実施する
#### 基本方針
- Red: 失敗するテストを書く
- Green: テストを通す最小限の実装
- Refactor: リファクタリング
- 小さなステップで進める
- 仮実装（ベタ書き）から始める
- 三角測量で一般化する
- 明白な実装が分かる場合は直接実装してもOK
- テストリストを常に更新する
- 不安なところからテストを書く

#### テスト記述ルール
- 全てのテストメソッドには[Description("")]属性を付与し、日本語でテストの概要を記述する
- Descriptionの記述例: [Description("上方向への移動時に座標のY値が1増加することを検証")]
- テストクラス自体にも[Description("")]属性を付与し、そのクラスがテストする対象と目的を記述する
- テストの意図と期待結果が日本語で明確に理解できるようにする

### リファクタリングルール
- リファクタリングはMartin Fowlerの提唱する手法で実施する
- リファクタリング後は必ずテストコードを検証する

## ドキュメント管理

### ドキュメント構造
```
Assets/MyGame/Documentation/
├── Specifications/    # ゲーム仕様書、機能仕様書
├── Tasks/            # タスク管理用ドキュメント（進行中・完了含む）
├── Templates/        # ドキュメントテンプレート
```

### ドキュメント参照ルール
- タスク実行前に必ず Assets/MyGame/Documentation/Specifications/ 配下の関連ドキュメントを確認する
- 新機能開発時は該当する仕様書を参照し、仕様に従って実装する
- 仕様に不明な点があれば作業を中断し、ユーザに確認を求める
- タスク完了後は Assets/MyGame/Documentation/Tasks/ にタスクの実行結果を記録する

### ドキュメントファイル命名規則
- 仕様書: `{機能名}_spec.md`
- タスク記録: `{YYYY-MM-DD}_{タスク名}.md`
- テンプレート: `{種類}_template.md`

## ツール・コマンド実行ルール

### 設定ファイル確認の必須化
- **重要**: 外部ツールのコマンドを実行する前に、必ず以下の設定ファイルを確認する
  1. `.claude/settings.json` (共通設定)
  2. `.claude/settings.local.json` (ローカル環境設定)
- 設定ファイルにツールのパスが定義されている場合は、必ずそのパスを使用する
- パスが設定されていない場合のみ、標準コマンドを使用する

### 対象ツール例
- **GitHub CLI**: `.claude/settings.local.json` の `github_cli.path`
- **Unity Editor**: `.claude/settings.local.json` の `unity.editor_path`
- **その他外部ツール**: 各種開発ツール、ビルドツール等

### 実行手順
1. 設定ファイル確認: `Read .claude/settings.local.json` を実行
2. パス設定の存在確認: 該当ツールのパス設定を確認
3. パス使用: 設定されたフルパスでコマンド実行
4. フォールバック: パス未設定の場合のみ標準コマンドを使用

## Git操作ルール

### Git操作の参照先
- Git操作に関する全ての詳細ルールは `Assets/MyGame/Documentation/Rules/GitRule.md` を参照すること
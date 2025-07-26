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
├── Prefabs/          # プレハブ
├── Sprites/          # スプライト画像

Documentation/        # プロジェクトドキュメント（ルートディレクトリ）
```

# 開発ルール
- Documentation フォルダ以下のタスク用文書を参照し、作業を開始する
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
コーディング規約と実装方針については、以下のドキュメントを参照してください：
- `Documentation/Rules/CodingRule.md`

## テストルール
テスト実装の方針とルールについては、以下のドキュメントを参照してください：
- `Documentation/Rules/TestRule.md`

## ドキュメント管理

### ドキュメント構造
```
Documentation/
├── Specifications/    # ゲーム仕様書、機能仕様書
├── Tasks/            # タスク管理用ドキュメント（進行中・完了含む）
├── Templates/        # ドキュメントテンプレート
```

### ドキュメント参照ルール
- タスク実行前に必ず Documentation/Specifications/ 配下の関連ドキュメントを確認する
- 新機能開発時は該当する仕様書を参照し、仕様に従って実装する
- 仕様に不明な点があれば作業を中断し、ユーザに確認を求める
- タスク完了後は Documentation/Tasks/ にタスクの実行結果を記録する

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

### Unity Natural MCP Server コマンド利用ルール
- **RunEditModeTests**: Unity EditorのEditModeテストを実行
  - **正しい実行方法**: `run-editmode-tests` コマンドを使用
  - **禁止**: `mcp__ide__executeCode` による実行（Jupyter環境専用のため）
  - **代替方法**: HTTP経由でMCPサーバーに直接リクエスト送信
- Unity Natural MCPサーバーはポート56780で動作し、`.vscode/mcp.json`で設定されている

### 対象ツール例
- **GitHub CLI**: `.claude/settings.local.json` の `github_cli.path`
- **Unity Editor**: `.claude/settings.local.json` の `unity.editor_path`
- **Unity Test Runner**: `run-editmode-tests` コマンド（MCP経由）
- **その他外部ツール**: 各種開発ツール、ビルドツール等

### 実行手順
1. 設定ファイル確認: `Read .claude/settings.local.json` を実行
2. パス設定の存在確認: 該当ツールのパス設定を確認
3. パス使用: 設定されたフルパスでコマンド実行
4. フォールバック: パス未設定の場合のみ標準コマンドを使用

## Git操作ルール
- Git操作をする際は必ず`Documentation/Rules/GitRule.md`を参照してから操作をすること

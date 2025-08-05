# CLAUDE.md

このファイルは、このリポジトリでコードを扱う際のClaude Code (claude.ai/code)向けのガイダンスを提供します。


# Unityプロジェクト構造
```
Assets/MyGame/
├── Scripts/           # コアゲームロジック
├── Scenes/           # ゲームシーン (Sample)
├── Prefabs/          # プレハブ
├── Sprites/          # スプライト画像

Documentation/        # プロジェクトドキュメント（ルートディレクトリ）

ClaudeCodeTools/      # ClaudeCodeが利用するツール配置ディレクトリ
```

# コードレビューコマンド  
プログラムを編集した後に必ず実施する
1. レビュー対象コードの取得方法が支持されていない場合は、この時点で処理を終了する  
2. 自分でコードを取得せず、その代わりに次節に記載したサブエージェントを**全て必ず並列で**起動する  
3. サブエージェントが並列で起動されていることを確認し、もしなされていない場合は起動されていないものを起動する  
4. 起動したサブエージェントにコード取得方法を伝えて、レビューを依頼する  

## 呼び出し対象のサブエージェント   
* csharp-class-modeling-reviewer    
* csharp-naming-reviewer  
* csharp-comment-reviewer  
* file-structure-checker  

# 開発ルール
- `Documentation` ディレクトリ以下のタスク用文書を参照し、作業を開始する
- 作業を開始する際は適切なブランチを作成し、作業を行い作業完了後プルリクエストを作成する
- 日本語で受け答えをする
- 絵文字は使用禁止
- ユーザからの指示や仕様に疑問があれば作業を中断し、質問すること
- 強制追加など -f コマンドは禁止
- 適切なタイミングでコミットを実行すること
- コミットのコメントは日本語で行うこと

## コーディングルール
コーディングルールは以下のドキュメントを参照
- `Documentation/Rules/CSharpCodingRule.md`

## テストルール
テスト実装の方針とルールについては、以下のドキュメントを参照
- `Documentation/Rules/TestRule.md`

# ドキュメント管理

## ドキュメント構造
```
Documentation/
├── Rules/            # 各種コーディングルールなど
├── Specifications/    # ゲーム仕様書、機能仕様書
├── Tasks/            # タスク管理用ドキュメント（進行中・完了含む）
├── Templates/        # ドキュメントテンプレート
```

## ドキュメント参照ルール
- タスク実行前に必ず Documentation/Specifications/ 配下の関連ドキュメントを確認する
- 新機能開発時は該当する仕様書を参照し、仕様に従って実装する
- 仕様に不明な点があれば作業を中断し、ユーザに確認を求める
- タスク完了後は Documentation/Tasks/ にタスクの実行結果を記録する

## ドキュメントファイル命名規則
- 仕様書: `{機能名}_spec.md`
- タスク記録: `{YYYY-MM-DD}_{タスク名}.md`
- テンプレート: `{種類}_template.md`

# ツール・コマンド実行ルール

## 設定ファイル確認の必須化
- **重要**: 外部ツールのコマンドを実行する前に、必ず以下の設定ファイルを確認する
  1. `.claude/settings.json` (共通設定)
  2. `.claude/settings.local.json` (ローカル環境設定)
- 設定ファイルにツールのパスが定義されている場合は、必ずそのパスを使用する
- パスが設定されていない場合のみ、標準コマンドを使用する

## Unity Natural MCP Server コマンド利用ルール
- **RunEditModeTests**: Unity EditorのEditModeテストをUnityNaturalMCPサーバー経由で実行
- Unity Natural MCPサーバーはポート56780で動作し、`.vscode/mcp.json`で設定されている

## 対象ツール例
- **GitHub CLI**: `.claude/settings.local.json` の `github_cli.path`
- **Unity Test Runner**: `RunEditModeTests` コマンド（MCP経由）
- **その他外部ツール**: 各種開発ツール、ビルドツール等

## 実行手順
1. 設定ファイル確認: `Read .claude/settings.local.json` を実行
2. パス設定の存在確認: 該当ツールのパス設定を確認
3. パス使用: 設定されたフルパスでコマンド実行
4. フォールバック: パス未設定の場合のみ標準コマンドを使用

# Git操作ルール
- Git操作をする際は必ず`Documentation/Rules/GitRule.md`を参照してから操作をすること

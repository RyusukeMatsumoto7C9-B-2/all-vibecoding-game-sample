# Git操作ルール

## Claude用Git操作ルール

### アカウント切り替え必須化
- **重要**: ClaudeがGit操作を実行する際は、必ず以下の手順に従う
  1. Git操作前: `/switch-to-claude-git` コマンドでClaude用アカウントに切り替え
  2. Git操作実行: 必要なGit操作を実行

### Git操作の基本ルール
- ブランチ作成後は必ずリモートにプッシュする
- プルリクエスト作成は GitHub CLI を優先使用する
- 認証エラー時は `.claude/settings.local.json` の設定を確認する
- 適切なタイミングでコミットを実行すること
- コミットのコメントは日本語で行うこと
- 強制追加など -f コマンドは禁止

## GitHub CLI使用規則
- GitHub CLIのパスは `.claude/settings.local.json` の `github_cli.path` で設定されている
- プッシュやプルリクエスト作成時は設定されたパスを使用する
- GitHub CLI コマンド実行例: `"/c/Program Files/GitHub CLI/gh.exe" pr create`

## Git操作実行例

### 基本的なGit操作フロー（Windows環境）
1. Git操作前: `/switch-to-claude-git` コマンドでClaude用アカウントに切り替え
2. Git操作実行: 必要なGit操作を実行

### プルリクエスト作成フロー（Windows環境）
1. Git操作前: `/switch-to-claude-git` コマンドでClaude用アカウントに切り替え
2. Git操作実行: 必要なGit操作を実行

## 注意事項
- Git操作を忘れずに必ずアカウント切り替えを行うこと
- アカウント切り替え忘れによる権限エラーに注意すること

## 利用可能なカスタムコマンド
- `/switch-to-claude-git`: git設定をClaude Code用アカウントに切り替え
- `/switch-to-user-git`: git設定をユーザーアカウントに切り替え
- `/show-git-config`: 現在のgit設定を表示・確認
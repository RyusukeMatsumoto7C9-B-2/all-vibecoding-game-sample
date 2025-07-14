# Git操作ルール

## Claude用Git操作ルール

### アカウント切り替え必須化
- **重要**: ClaudeがGit操作を実行する際は、必ず以下の手順に従う
  1. Git操作前: 直接コマンドでClaude用アカウントに切り替え
  2. Git操作実行: 必要なGit操作を実行
  3. Git操作後: 直接コマンドでユーザーアカウントに戻す

### アカウント切り替えコマンド（Windows環境）
- **Claude用アカウント切り替え**:
  ```bash
  git config --global user.name "Claude"
  git config --global user.email "noreply@anthropic.com"
  ```
- **ユーザーアカウント切り替え**:
  ```bash
  git config --global user.name "RyusukeMatsumoto"
  git config --global user.email "matsumotokakadevelop1102@gmail.com"
  ```

### 補助スクリプト（Windows環境）
- **Claude用スクリプト**: `ClaudeCodeTools/use-claude-account.bat`
- **ユーザー用スクリプト**: `ClaudeCodeTools/use-user-account.bat`
- **実行方法**: Windows環境では個別のgitコマンドを順次実行
- **注意**: エラーハンドリング付きでスクリプトを改良済み
- **フォルダ説明**: ClaudeCodeToolsフォルダには今後追加されるPowerShellスクリプトなどのツール類を配置

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
```bash
# 1. Claude用アカウントに切り替え
git config --global user.name "Claude"
git config --global user.email "noreply@anthropic.com"

# 2. Git操作実行
git add .
git commit -m "機能追加: 新機能の実装"
git push origin feature-branch

# 3. ユーザーアカウントに戻す
git config --global user.name "RyusukeMatsumoto"
git config --global user.email "matsumotokakadevelop1102@gmail.com"
```

### プルリクエスト作成フロー（Windows環境）
```bash
# 1. Claude用アカウントに切り替え
git config --global user.name "Claude"
git config --global user.email "noreply@anthropic.com"

# 2. ブランチプッシュ
git push -u origin feature-branch

# 3. プルリクエスト作成
"/c/Program Files/GitHub CLI/gh.exe" pr create --title "タイトル" --body "説明"

# 4. ユーザーアカウントに戻す
git config --global user.name "RyusukeMatsumoto"
git config --global user.email "matsumotokakadevelop1102@gmail.com"
```

## 注意事項
- Git操作を忘れずに必ずアカウント切り替えを行うこと
- 操作完了後は必ずユーザーアカウントに戻すこと
- アカウント切り替え忘れによる権限エラーに注意すること
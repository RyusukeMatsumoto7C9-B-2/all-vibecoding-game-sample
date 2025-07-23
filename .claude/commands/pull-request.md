---
allowed-tools: Bash(git *), Bash(gh *)
description: 現在のブランチからmasterブランチへのプルリクエストを作成
---

## コンテキスト

- 現在のブランチ: !`git branch --show-current`
- 現在のgitステータス: !`git status --short`
- masterとの差分: !`git log --oneline origin/master..HEAD`
- 変更されたファイル: !`git diff --name-only origin/master..HEAD`

## タスク

現在のブランチからmasterブランチへのプルリクエストを作成してください。

### 実行手順

1. **事前チェック**
   - 現在のブランチがmasterでないことを確認
   - 未コミットの変更がないことを確認
   - masterブランチとの差分があることを確認

2. **プルリクエスト情報の準備**
   - タイトル: 最新コミットメッセージを基に自動生成、または引数で指定
   - 本文: コミット履歴と変更ファイル一覧を含む詳細な説明を自動生成

3. **GitHub CLIでプルリクエスト作成**
   ```bash
   # 最新のmasterブランチ情報を取得
   git fetch origin master
   
   # プルリクエストを作成（タイトルと本文は動的生成）
   gh pr create --title "$(git log -1 --pretty=format:'%s')" \
     --body "$(cat <<EOF
   ## 変更内容
   
   $(git log --pretty=format:'- %s' origin/master..HEAD)
   
   ## 変更されたファイル
   
   $(git diff --name-only origin/master..HEAD | sed 's/^/- /')
   
   ## テスト実行状況
   
   - [ ] EditModeテスト実行
   - [ ] 動作確認
   
   🤖 Generated with [Claude Code](https://claude.ai/code)
   EOF
   )" \
     --base master \
     --head $(git branch --show-current)
   ```

4. **結果の確認と次ステップの案内**
   - 作成されたプルリクエストURLを表示
   - レビューやマージに関する次ステップを案内

### 引数オプション

- `--title "タイトル"`: プルリクエストのタイトルを手動指定
- `--draft`: ドラフトプルリクエストとして作成
- `--body "本文"`: プルリクエストの本文を手動指定

### エラーハンドリング

- masterブランチからの実行を防止
- 未コミット変更がある場合の警告
- GitHub CLI未インストールやログイン未完了の場合のエラー表示

### 出力例

```
✅ プルリクエストが正常に作成されました！
プルリクエストURL: https://github.com/user/repo/pull/123

📝 次のステップ:
  1. プルリクエストページで詳細を確認
  2. レビュアーを追加  
  3. 必要に応じてラベルやマイルストーンを設定
  4. CIチェックの結果を確認
```
# Git設定表示コマンド

現在のgitグローバル設定を表示し、settings.local.jsonの設定と比較します。

## 実行手順

1. 設定ファイル確認: `.claude/settings.local.json`を読み込み
2. 現在のgitグローバル設定を取得
3. 設定ファイルの内容と現在の設定を比較表示

## 実装

### 設定ファイル参照
- `.claude/settings.local.json`の`git.user_account`と`git.claude_account`を参照
- 現在の設定がどちらのアカウントに対応しているかを判定

### 表示内容
- 現在のgit user.name
- 現在のgit user.email
- 設定ファイルで定義されているアカウント情報
- 現在の設定がどのアカウントに対応しているかの判定結果

### 実行コマンド
```bash
# 現在のgit設定を取得
git config --global user.name
git config --global user.email
```

### 比較ロジック
現在の設定と設定ファイルの各アカウント情報を比較し、一致するアカウントを特定します。
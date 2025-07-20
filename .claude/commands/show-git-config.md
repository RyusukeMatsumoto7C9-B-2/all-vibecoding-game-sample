# Git設定表示コマンド

現在のgitグローバル設定とローカル設定を表示

## 実行手順

1. 現在のgitグローバル設定およびローカル設定を取得
2. 設定ファイルの内容をそれぞれ以下の表示形式の書式にしたがい表示する

## 実装

### 表示内容
- 現在のglobal設定の git user.name
- 現在のglobal設定のgit user.email
- 現在のlocal設定の git user.name
- 現在のlocal設定のgit user.email

### 表示形式
global:
  "user.name": "現在のグローバルユーザ名",
  "user.email": "現在のグローバルメールアドレス"

local:
  "user.name": "現在のローカルユーザ名",
  "user.email": "現在のローカルメールアドレス"

### 実行コマンド
```bash
# 現在のgit設定を取得
git config --global user.name
git config --global user.email
git config --local user.name
git config --local user.email
```


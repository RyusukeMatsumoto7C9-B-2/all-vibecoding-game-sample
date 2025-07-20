# Git設定表示コマンド

現在のgitグローバル設定とローカル設定を表示します。

## 実行手順

1. 現在のgitグローバル設定を取得
2. 現在のgitローカル設定を取得
3. 取得した設定値を表示

## 実装

### 表示内容
- グローバル設定のgit user.name
- グローバル設定のgit user.email
- ローカル設定のgit user.name
- ローカル設定のgit user.email

### 実行コマンド
```bash
# グローバル設定を取得
git config --global user.name
git config --global user.email

# ローカル設定を取得
git config --local user.name
git config --local user.email
```
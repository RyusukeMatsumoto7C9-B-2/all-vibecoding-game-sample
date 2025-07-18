gitのグローバル設定をユーザーアカウントに切り替えます。

## 実行手順

1. 設定ファイル確認: `.claude/settings.local.json`を読み込み
2. `git.user_account`の設定値を取得  
3. gitグローバル設定を該当アカウントに更新
4. 切り替え結果を確認・表示

## 実装

### 設定ファイル参照と実行
```bash
# 設定ファイルを確認
Read .claude/settings.local.json

# 設定ファイルから取得した値でgit設定を更新
git config --global user.name "RyusukeMatsumoto"
git config --global user.email "matsumotokakadevelop1102@gmail.com"

# 設定変更を確認
git config --global user.name
git config --global user.email
```

### 確認・表示
- 更新後のgit設定を表示
- 切り替え成功メッセージを表示

このコマンドによりgitのグローバル設定がユーザーアカウント（RyusukeMatsumoto）に切り替わります。
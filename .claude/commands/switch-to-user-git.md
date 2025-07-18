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

# 設定ファイルから取得した値でgit設定を更新（ローカル設定で優先）
# 注意: 実際の値は settings.local.json の git.user_account から取得
git config --local user.name "{{ settings.local.json.git.user_account.name }}"
git config --local user.email "{{ settings.local.json.git.user_account.email }}"

# 設定変更を確認
git config --local user.name
git config --local user.email
```

### 実装方法
1. `.claude/settings.local.json` を読み込む
2. `git.user_account.name` と `git.user_account.email` の値を取得
3. 取得した値を使用してgitローカル設定を更新（プロジェクト固有の設定として優先適用）

### 確認・表示
- 更新後のgit設定を表示
- 切り替え成功メッセージを表示

このコマンドによりgitのローカル設定（プロジェクト固有）がsettings.local.jsonで定義されたユーザーアカウントに切り替わります。ローカル設定はグローバル設定よりも優先されるため、このプロジェクトでのコミットは確実にユーザーアカウントで実行されます。
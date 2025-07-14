@echo off
echo Claude用アカウントに切り替え中...
git config --global user.name "Claude"
if %errorlevel% neq 0 (
    echo エラー: ユーザー名の設定に失敗しました
    exit /b 1
)
git config --global user.email "noreply@anthropic.com"
if %errorlevel% neq 0 (
    echo エラー: メールアドレスの設定に失敗しました
    exit /b 1
)
echo 完了: Claude用アカウントに切り替えました
echo 名前: Claude
echo メール: noreply@anthropic.com
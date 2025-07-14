@echo off
echo ユーザーアカウントに切り替え中...
git config --global user.name "RyusukeMatsumoto"
if %errorlevel% neq 0 (
    echo エラー: ユーザー名の設定に失敗しました
    exit /b 1
)
git config --global user.email "matsumotokakadevelop1102@gmail.com"
if %errorlevel% neq 0 (
    echo エラー: メールアドレスの設定に失敗しました
    exit /b 1
)
echo 完了: ユーザーアカウントに切り替えました
echo 名前: RyusukeMatsumoto
echo メール: matsumotokakadevelop1102@gmail.com
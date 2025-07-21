# スプライト作成ツール 取扱説明書

## 概要

このツールは、指定した色の64x64ピクセルのPNG画像を生成するためのコマンドラインツールです。Unity プロジェクトで使用するスプライト画像を素早く作成できます。

## ツール構成

- **メインツール**: `ClaudeCodeTools/sprite-create-tool.bat`
- **PowerShellモジュール**: `ClaudeCodeTools/SpriteCreator.psm1`
- **設定ファイル**: `.claude/settings.json`

## インストール

ツールは既にプロジェクトの`ClaudeCodeTools`フォルダに配置されており、`.claude/settings.json`にカスタムコマンドとして登録されています。

## 使用方法

### 基本的な使い方

```bash
# PowerShellから直接実行
powershell -ExecutionPolicy Bypass -Command "Import-Module './ClaudeCodeTools/SpriteCreator.psm1'; Create-Sprite -ColorString '(R,G,B,A)'"

# batファイルから実行
ClaudeCodeTools/sprite-create-tool.bat (R,G,B,A)
```

### カラー指定

色は ARGB 形式で指定します：
- **A**: アルファ値（透明度）
- **R**: 赤色成分
- **G**: 緑色成分  
- **B**: 青色成分

#### 値の範囲

1. **0-255の整数値** （標準的なRGB値）
2. **0.0-1.0の小数値** （正規化されたRGB値）

### 実行例

```bash
# 赤色のスプライト (255, 0, 0, 255)
powershell -ExecutionPolicy Bypass -Command "Import-Module './ClaudeCodeTools/SpriteCreator.psm1'; Create-Sprite -ColorString '(255,0,0,255)'"

# 緑色のスプライト (0, 255, 0, 255)
powershell -ExecutionPolicy Bypass -Command "Import-Module './ClaudeCodeTools/SpriteCreator.psm1'; Create-Sprite -ColorString '(0,255,0,255)'"

# 青色のスプライト (0, 0, 255, 255)
powershell -ExecutionPolicy Bypass -Command "Import-Module './ClaudeCodeTools/SpriteCreator.psm1'; Create-Sprite -ColorString '(0,0,255,255)'"

# 半透明の白色 (255, 255, 255, 128)
powershell -ExecutionPolicy Bypass -Command "Import-Module './ClaudeCodeTools/SpriteCreator.psm1'; Create-Sprite -ColorString '(255,255,255,128)'"

# 正規化された値でグレー (0.5, 0.5, 0.5, 1.0)
powershell -ExecutionPolicy Bypass -Command "Import-Module './ClaudeCodeTools/SpriteCreator.psm1'; Create-Sprite -ColorString '(0.5,0.5,0.5,1.0)'"
```

## 出力

### 出力先

生成されたスプライト画像は `Assets/MyGame/Sprites/` フォルダに保存されます。

### ファイル名形式

```
sprite_{R}_{G}_{B}_{A}_{timestamp}.png
```

例：`sprite_255_0_0_255_20250717_030109.png`

### 出力サイズ

- **幅**: 64ピクセル
- **高さ**: 64ピクセル
- **形式**: PNG

## エラーハンドリング

### 一般的なエラー

1. **色指定エラー**
   ```
   Error: Invalid color format. Please use (R,G,B,A) format.
   ```
   - 解決方法: 正しい形式で色を指定してください

2. **PowerShell実行ポリシーエラー**
   - 解決方法: `-ExecutionPolicy Bypass`パラメータを使用してください

3. **モジュール読み込みエラー**
   - 解決方法: `SpriteCreator.psm1`ファイルの存在を確認してください

## 技術詳細

### 使用技術

- **PowerShell 5.x以上**
- **.NET System.Drawing** - 画像生成
- **Batch Script** - Windowsコマンドライン統合

### 機能

- ARGB色空間での色指定
- 0-1および0-255の値範囲サポート
- 自動的なタイムスタンプ付きファイル名生成
- 出力ディレクトリの自動作成
- エラーハンドリングとバリデーション

## トラブルシューティング

### 問題: PowerShellの実行ポリシーエラー

**症状**: PowerShellスクリプトが実行できない

**解決策**:
```powershell
# 実行ポリシーを一時的に変更
powershell -ExecutionPolicy Bypass -Command "..."

# または現在のユーザーの実行ポリシーを変更
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### 問題: 出力ディレクトリが存在しない

**症状**: ファイルが保存されない

**解決策**: ツールが自動的にディレクトリを作成しますが、手動で作成することも可能です
```bash
mkdir "Assets/MyGame/Sprites"
```

### 問題: 色の値が正しく認識されない

**症状**: 期待した色にならない

**解決策**:
- 括弧とカンマを正しく使用してください: `(R,G,B,A)`
- 値の範囲を確認してください: 0-255または0.0-1.0
- 引用符を適切に使用してください

## 更新履歴

- **v1.0.0** (2025-07-17): 初期リリース
  - 基本的なスプライト作成機能
  - ARGB色指定サポート
  - 自動ファイル名生成

## サポート

問題が発生した場合は、以下を確認してください：
1. PowerShellのバージョン
2. 実行ポリシーの設定
3. ファイルパスの正確性
4. 色指定の形式

追加のサポートが必要な場合は、プロジェクトの開発者にお問い合わせください。
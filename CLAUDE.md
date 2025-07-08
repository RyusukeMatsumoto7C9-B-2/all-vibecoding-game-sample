# CLAUDE.md

このファイルは、このリポジトリでコードを扱う際のClaude Code (claude.ai/code)向けのガイダンスを提供します。

## プロジェクト概要

これはUnity 6000.0.51f1で構築されたUnity 2Dゲームプロジェクトで、Model Context Protocol (MCP)サーバー機能を統合しています。このプロジェクトはUniversal Render Pipeline (URP)を使用し、Input System、非同期操作用のUniTask、NuGetパッケージ管理などの最新のUnityパッケージを含んでいます。

## 主要なアーキテクチャコンポーネント

### Unityプロジェクト構造
- **Assets/MyGame/**: メインゲームソースコードディレクトリ（現在空 - すべてのゲームスクリプトをここに配置する必要があります）
- **Assets/Scenes/**: メインゲームシーンを含む（SampleScene.unity）
- **Assets/Settings/**: UnityプロジェクトsetとURP設定
- **Assets/Packages/**: NuGetForUnityで管理されるNuGetパッケージ
- **ProjectSettings/**: Unityプロジェクト設定ファイル

### MCP統合
プロジェクトにはUnity Natural MCP Server (UnityNaturalMCP)統合が含まれています：
- サーバーはポート56780で設定（ProjectSettings/UnityNaturalMCPSetting.assetで設定可能）
- MCPサーバーログ表示が有効
- デフォルトMCPツールが有効

### パッケージ依存関係
主要パッケージとその目的：
- **Microsoft.Extensions.AI.Abstractions**: AIサービス抽象化
- **Microsoft.Extensions.DependencyInjection**: 依存性注入コンテナ
- **ModelContextProtocol**: MCPクライアント/サーバー機能
- **System.Text.Json**: MCP通信用JSON シリアライゼーション
- **UniTask**: Unity向けAsync/await サポート
- **Unity Input System**: 最新の入力処理

## 開発コマンド

### Unity エディタ
- Unity HubでUnity エディタでプロジェクトを開く
- ビルド: File → Build Settings → Build（またはCtrl+Shift+B）
- プレイモード: プレイボタンをクリックまたはCtrl+Pを押す

### IDE統合
- **Visual Studio**: all-vibecoding-game-sample.slnを使用
- **JetBrains Rider**: フルサポートが設定済み
- **Visual Studio Code**: Unity拡張機能でサポート

### NuGetパッケージ管理
- NuGetパッケージはNuGetForUnityで管理
- 設定: Assets/NuGet.config
- パッケージリスト: Assets/packages.config
- パッケージ管理にはUnity エディタのNuGet For Unityウィンドウを使用

## 技術設定

### Unity設定
- **Unity バージョン**: 6000.0.51f1
- **ターゲットフレームワーク**: .NET Framework 4.7.1
- **言語バージョン**: C# 9.0
- **レンダーパイプライン**: Universal Render Pipeline (URP)
- **2D機能**: Unity 2D機能パッケージで有効

### Input System
- 入力アクションはAssets/InputSystem_Actions.inputactionsで定義
- プレイヤーアクションには以下を含む: Move、Look、Attack、Interact、Crouch
- 移動とlookにはVector2、離散アクションにはButtonを使用

### MCPサーバー設定
- デフォルトIP: '*'（すべてのインターフェース）
- デフォルトポート: 56780
- デバッグ用ログ表示が有効
- デフォルトツールが有効

## コード整理ガイドライン

### スクリプト配置
- すべてのゲームスクリプトはAssets/MyGame/に配置する必要があります
- Unity名前空間の慣例に従う
- 大きなプロジェクトではアセンブリ定義ファイルを使用

### 依存関係
- サービス登録にはMicrosoft.Extensions.DependencyInjectionを使用
- Unity CoroutinesではなくUniTaskを非同期操作に活用
- JSON操作にはSystem.Text.Jsonを使用

### MCP統合パターン
- MCPツールはサービスとして実装する必要があります
- MCPサービス登録には依存性注入を使用
- ツール定義にはMCPプロトコル仕様に従う

## 開発ノート

### 空のプロジェクト状態
プロジェクトは現在、以下の初期状態にあります：
- ソースコード用の空のMyGameディレクトリ
- 基本的なUnityシーンセットアップ
- MCPサーバー統合準備完了
- すべてのパッケージ依存関係がインストール済み

### 開発の次のステップ
1. Assets/MyGame/にメインゲームスクリプトを作成
2. MCPツールとサービスを実装
3. シーンでゲームオブジェクトとコンポーネントを設定
4. ゲームプレイ用の入力システムアクションを設定

### テスト
- 単体テストにはUnity Test Frameworkを使用
- MCPサーバー機能はMCPクライアント接続でテスト可能
- 必要に応じてターゲットプラットフォーム全体でビルドとテストを実行
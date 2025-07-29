# アーキテクチャ 仕様書

## 概要
このプロジェクトは、テスタビリティと保守性を重視したクリーンアーキテクチャを採用する。
UI層はMV(R)P設計、依存性注入はVContainer、リアクティブプログラミングはR3を使用し、
可能な限りPureC#でロジックを実装してUnity TestRunnerでのテストを充実させる。

## 目的
- 高いテスタビリティを持つコードベースの構築
- 疎結合で拡張性の高いアーキテクチャの実現
- Unity依存を最小限に抑えたロジック層の構築

## 機能要件

- MV(R)P設計によるUI層の実装
- VContainerを使用した依存性注入
- PureC#によるロジック層の実装
- R3によるリアクティブプログラミング
- シングルトンパターンの使用禁止

## 動作仕様

### レイヤー構成
- **View Layer**: MonoBehaviour, uGUI
- **Presenter Layer**: MonoBehaviour, R3
- **Service Layer**: PureC#, Business Logic
- **Repository Layer**: PureC#, Data Access

### 主要フレームワーク
- **uGUI**: UI実装
- **VContainer**: 依存性注入コンテナ
- **R3**: リアクティブプログラミング
- **Unity Input System**: 入力処理
- **Unity 2D Sprite System**: タイルマップシステム（UniversalTileプレハブベース）

## テスト仕様
テスト実装の方針とルールについては、以下のドキュメントを参照してください：
- `Documentation/Rules/TestRule.md`

## 注意事項
- MonoBehaviourには最小限のロジックのみを記述し、ほとんどのロジックはPureC#で実装する
- UIのSerializeFieldは必ずprivateとし、[SerializeField]属性を使用する
- R3のObservableを適切に管理し、メモリリークを防ぐ

## 更新履歴
| 日付 | 変更内容 | 担当者 |
|------|----------|--------|
| 2025-07-12 | 初版作成 | Claude |
| 2025-07-22 | 仕様書をシンプル化、技術詳細・実装詳細を削除 | Claude |
| 2025-07-29 | タイルマップシステムの単一プレハブ化アーキテクチャ反映 | Claude |
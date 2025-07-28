# Player フォルダコード修正タスクメモ

## 実施日
2025-07-28

## 対象
Assets\MyGame\Scripts\Player\ フォルダ以下のC#コード

## レビュー結果概要

### クラス設計レビュー結果
- **PlayerController.cs**: Updateメソッドに移動処理ビジネスロジックが直接記述（ルール違反）
- **PlayerInputHandler.cs**: R3未使用でeventを使用（ルール違反）
- **PlayerMoveService.cs**: 良好な設計、PureC#で実装済み
- **Direction.cs**: 問題なし

### 命名規則レビュー結果
- 全てのコードが命名規則に完全準拠
- 優秀な命名が実施されている

## 修正計画

### 高優先度修正項目

1. **PlayerController.cs のUpdate メソッド修正**
   - Updateメソッドから移動処理ビジネスロジックを分離
   - 移動処理を専用のハンドラーメソッドに移動
   - 対象: PlayerController.cs:42-54

2. **PlayerInputHandler.cs のR3移行**
   - `public event Action<Direction> OnMoveInput` をR3のReactivePropertyまたはSubjectに変更
   - イベント購読系処理をR3ベースに書き換え
   - 対象: PlayerInputHandler.cs:9

3. **VContainer依存関係注入システム実装**
   - PlayerControllerのnew演算子による直接インスタンス化を修正
   - VContainerを使用した依存関係注入に変更
   - 対象: PlayerController.cs:18

4. **修正後のテスト実行**
   - EditModeテストを実行して動作確認

### 中優先度修正項目

5. **PlayerController.cs のクラス内宣言順序修正**
   - privateフィールドとUnityイベントメソッドの順序を適切に調整

6. **AutoDetectTilemapManager() の責務分離**
   - リフレクションを使用した複雑な検索処理を別クラスに分離
   - 対象: PlayerController.cs:62-77

7. **PlayerMoveService.cs のログ出力抽象化**
   - Debug.LogWarning/LogをILoggerインターフェース経由に変更

### 低優先度修正項目

8. **PlayerInputHandler.cs の例外処理改善**
   - throw new ArgumentExceptionの処理をより適切な形に修正
   - 対象: PlayerInputHandler.cs:44

## 詳細な設計ルール違反箇所

### PlayerController.cs
- **L43-54**: Updateメソッドに移動処理のビジネスロジックが直接記述
- **L65-77**: AutoDetectTilemapManager()がリフレクションを使用した複雑な検索処理
- **L18**: new演算子による直接インスタンス化
- **L33, L39**: UnityEventではなくR3を使用すべき

### PlayerInputHandler.cs
- **L9**: `public event Action<Direction> OnMoveInput;` - R3を使用すべき
- **L44**: throw new ArgumentExceptionでの例外処理が不適切

## 期待される効果
- Unityライフサイクルメソッドからのビジネスロジック分離
- R3を使用したリアクティブプログラミングの実現
- VContainerによるテスタブルな依存関係注入
- 設計ルール完全準拠の実現

## 注意事項
- 修正後は必ずEditModeテストを実行して動作確認すること
- 各修正は段階的に実施し、都度テストで確認すること
- コミットは適切なタイミングで日本語コメントで実施すること
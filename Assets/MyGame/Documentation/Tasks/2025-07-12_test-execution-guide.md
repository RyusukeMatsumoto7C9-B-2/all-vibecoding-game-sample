# Unity Test Runner 実行ガイド

## 実行日時
2025-07-12

## EditModeテスト実行手順

### Unity Editor内でのテスト実行方法

1. **Test Runnerウィンドウを開く**
   - Unity Editor メニューバー: `Window > General > Test Runner`

2. **EditModeタブを選択**
   - Test Runner ウィンドウで "EditMode" タブをクリック

3. **テスト一覧確認**
   - `MyGame.Player.Tests` アセンブリが表示されることを確認
   - `PlayerMoveServiceTests` クラスが表示されることを確認
   - 以下の5つのテストメソッドが表示されることを確認:
     - `Move_WithUpDirection_ShouldUpdatePositionUpward`
     - `Move_WithDownDirection_ShouldUpdatePositionDownward`
     - `Move_WithLeftDirection_ShouldUpdatePositionLeft`
     - `Move_WithRightDirection_ShouldUpdatePositionRight`
     - `SetPosition_ShouldUpdateCurrentPosition`

4. **テスト実行**
   - `PlayerMoveServiceTests` を右クリック → `Run Selected` をクリック
   - または "Run All" ボタンをクリックして全テストを実行

### 期待される結果

- **全5つのテストが成功（緑色）** で表示されること
- 各テストの詳細:
  - **上移動テスト**: (0,0) → (0,1) への移動が正しく実行される
  - **下移動テスト**: (0,1) → (0,0) への移動が正しく実行される
  - **左移動テスト**: (1,0) → (0,0) への移動が正しく実行される
  - **右移動テスト**: (0,0) → (1,0) への移動が正しく実行される
  - **位置設定テスト**: SetPosition(5,3) で位置が正しく設定される

### テスト対象コード

- **PlayerMoveService.cs**: PureC#実装のコアロジック
  - `SetPosition(Vector2Int position)`: 位置設定メソッド
  - `Move(Direction direction)`: 移動処理メソッド
  - `CurrentPosition`: 現在位置プロパティ

- **Direction.cs**: 移動方向を定義するenum
  - Up, Down, Left, Right の4方向

### テスト設計

- **TDD (Test-Driven Development)** 手法で実装
- **Red-Green-Refactor** サイクルに従った開発
- **PureC#** 実装により Unity TestRunner での実行が可能
- **AAA パターン** (Arrange-Act-Assert) でテストを構造化

### 注意事項

- Unity Editor が起動している状態で実行する
- コマンドライン実行は Unity Editor との競合により不可
- テスト失敗時は実装ロジックの見直しが必要
- 全テストが成功することで基本移動機能の正確性が保証される

### トラブルシューティング

1. **テストが表示されない場合**
   - プロジェクトを再インポート: `Assets > Reimport All`
   - Assembly Definition の設定確認

2. **コンパイルエラーが発生する場合**
   - `MyGame.asmdef` の参照設定確認
   - namespace の一致確認

3. **テストが失敗する場合**
   - PlayerMoveService の実装ロジック確認
   - Vector2Int の座標系確認（Y軸が上向き）

## 実行推奨タイミング

- コード変更後の動作確認
- プルリクエスト作成前の品質保証
- リファクタリング後の機能確認
- 新機能追加時の既存機能への影響確認
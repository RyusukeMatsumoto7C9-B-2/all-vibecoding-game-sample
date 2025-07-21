# プレイヤー 仕様書

## 概要
プレイヤーは地下ダンジョンを探索するキャラクターで、タイルマップ上を移動しながら土ブロックを破壊して進路を開拓する。

## 目的
- 直感的で制約のある移動システムを提供する
- ブロック破壊による戦略的な経路選択を実現する
- 岩ブロックによる移動制限で難易度を調整する

## 要件

### 機能要件
- ✅ **上下左右の4方向移動システム** (完全実装済み - WASD入力、アニメーション対応)
- ❌ **土ブロック破壊機能** (未実装 - タイルマップとの相互作用なし)
- ❌ **岩ブロックに対する移動制限** (未実装 - 移動制限機能なし)
- ✅ **初期配置システム** (実装済み - X10, Y3座標での固定配置)

### 非機能要件
- レスポンシブな入力処理
- スムーズな移動アニメーション
- 一定フレームレートでの動作保証

## 技術仕様

### 描画仕様
- **スプライト**: 正方形の2Dスプライト
- **レンダリング**: Unity 2D Renderer使用

### 移動システム
- **移動方向**: 上下左右の4方向のみ（斜め移動は不可）
- **移動方式**: グリッドベース移動（1マス単位）
- **入力方式**: Unity Input System使用（WASD キー）
- **移動速度**: [要調整] ミリ秒/マス

### 初期配置
- **初期位置**: タイルマップ上のX10, Y3
- **座標系**: タイルマップ座標に基づく配置

### ブロック相互作用システム
- **土ブロック**: 移動時に自動破壊
- **岩ブロック**: 破壊不可、通過不可
- **空中**: 通過可能

## 実装詳細

### 実装手順
1. プレイヤー移動コントローラーの作成
2. Input Systemとの連携実装
3. タイルマップとの衝突判定実装
4. ブロック破壊ロジックの実装
5. アニメーションシステムの実装

### 依存関係
- Unity Input System
- Unity Tilemap System
- Unity Animation System
- VContainer（依存性注入）

### クラス設計
```
PlayerController (MonoBehaviour)
├── PlayerMoveService (PureC#)
├── PlayerBlockInteractionService (PureC#)
├── PlayerInputHandler (MonoBehaviour)
└── PlayerAnimationController (MonoBehaviour)
```

### 主要メソッド
- `Move(Direction direction)`: プレイヤー移動処理
- `CanMoveTo(Vector2Int position)`: 移動可能判定
- `DestroyBlock(Vector2Int position)`: ブロック破壊処理
- `SetInitialPosition()`: 初期位置設定

## テスト仕様

### テスト実装方針
- t-wada流TDDを適用
- Unity TestRunnerを使用
- PureC#クラスの重点的なテスト

### テスト項目
1. **移動システムテスト**
   - 4方向移動の正確性確認
   - 斜め移動の制限確認
   - グリッド境界での動作確認

2. **ブロック相互作用テスト**
   - 土ブロック破壊の確認
   - 岩ブロック通過制限の確認
   - 空中移動の確認

3. **初期配置テスト**
   - 正確な初期位置の確認
   - 初期状態の検証

4. **衝突判定テスト**
   - タイルマップとの正確な衝突判定
   - マップ境界での動作確認

### テストクラス例
```csharp
[TestFixture]
public class PlayerMoveServiceTests
{
    [Test]
    public void Move_WithValidDirection_ShouldUpdatePosition()
    
    [Test]
    public void CanMoveTo_WithRockBlock_ShouldReturnFalse()
    
    [Test]
    public void DestroyBlock_WithDirtBlock_ShouldRemoveBlock()
}
```

## 注意事項
- TestRunner は EditMode のみを実施する（PlayMode のテストはユーザが手動で実施する）
- 可能な限り PureC# で実装し、TestRunnerでのテストを充実させる
- シングルトンパターンは使用禁止、VContainerを使用する
- 移動処理はフレームレートに依存しない実装とする
- ブロック破壊時のエフェクト・サウンド再生を考慮した設計とする

## 更新履歴
| 日付 | 変更内容 | 担当者 |
|------|----------|--------|
| 2025-07-12 | 初版作成 | Claude |
| 2025-07-12 | 描画仕様追加（正方形2Dスプライト）、入力方式詳細化（WASD） | Claude |
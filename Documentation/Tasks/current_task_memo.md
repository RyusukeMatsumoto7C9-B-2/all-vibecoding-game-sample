# 現在のタスク

## TilemapSystem改善タスク

### 概要
Enemy、Player等のゲームオブジェクトがTilemapSystemと連携しやすくするための機能拡張を実装する。

### 実装要件

#### 🔵 座標変換システム
- [ ] **GetPosition(int x, int y): Vector3** - グリッド座標からワールド座標への変換
  - [ ] ITilemapManagerインターフェースへのメソッド追加
  - [ ] TilemapManagerでの実装
  - [ ] TilemapSystemControllerへの公開メソッド追加
  - [ ] 単体テストの作成

#### 🔵 タイル状態取得システム  
- [ ] **GetTileStatus(int x, int y)** - タイル属性の取得（移動可否判定用）
  - [ ] 現在のGetBlockTypeAtメソッドの拡張検討
  - [ ] Player/Enemy向けの統一インターフェース設計
  - [ ] 移動可否の判定ロジック統合
  - [ ] 単体テストの作成

### 技術詳細

#### 現在の実装状況
- **既存メソッド**:
  - `GetBlockTypeAt(Vector2Int position, int level)` - BlockType取得
  - `CanPlayerPassThrough(Vector2Int position, int level)` - プレイヤー通過可否判定
  - タイル配置時の座標計算: `new Vector3(x, y, 0)` (TilemapManager.cs:57)

#### 実装方針
1. **座標変換**: グリッド座標(int x, int y)を受け取り、タイルの中心座標をVector3で返す
2. **統一インターフェース**: Player/Enemyが共通で使用できる移動判定インターフェース
3. **Rockタイル判定**: Enemy/Player両方でRockタイルへの移動を制限

### 関連ファイル
- `Assets/MyGame/Scripts/TilemapSystem/Core/ITilemapManager.cs`
- `Assets/MyGame/Scripts/TilemapSystem/Core/TilemapManager.cs`
- `Assets/MyGame/Scripts/TilemapSystem/TilemapSystemController.cs`

---

# アーカイブ済みタスク

## エネミーシステム開発（完了）

### ✅ フェーズ1: 基本移動システム（2025-01-22完了）
- EnemyController、EnemyMoveService、EnemyMovementConstraint実装
- Player/Enemy間でDirection.cs共通化
- 岩ブロック通過不可、マップ境界制限実装
- 単体テスト・統合テスト完了

### ✅ フェーズ2: レベルベース出現管理（2025-01-09完了）
- EnemySpawner、EnemySpawnConfig実装
- レベル別出現数計算機能（レベル1:5体、5レベル毎+1、上限10体）
- 画面境界外出現位置計算
- PlayModeテスト完了

## 参考資料
- `Documentation/Specifications/enemy_spec.md`
- `Documentation/Rules/CSharpCodingRule.md`
- `Documentation/Rules/TestRule.md`
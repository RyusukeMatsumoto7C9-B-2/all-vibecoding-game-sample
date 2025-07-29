# 現在のタスクメモ

## スプライト切り替え機能実装

### 概要
tilemap_system_specに追加された「スプライトの切り替え」機能を実装する。
単一のプレハブを再利用し、設定された属性（BlockType）によって表示するスプライトを変更する基盤システムを構築する。

### 実装目標
- 各BlockTypeごとに個別プレハブを使用する現在の方式から、単一プレハブでスプライト切り替えを行う方式に変更
- メモリ効率とパフォーマンスの向上
- 新しいBlockType追加時の運用負荷軽減

### 現状分析

#### 現在のシステム
- **プレハブ管理**: `Dictionary<BlockType, GameObject> _tilePrefabs`
- **タイル更新**: プレハブ完全置き換え（Destroy→Instantiate）
- **プレハブ数**: 5種類（Sky/Empty/Ground/Rock/Treasure）
- **構成**: 各プレハブは同一構造（Transform + SpriteRenderer）

#### 問題点
- BlockType数に比例してプレハブが増加
- タイル変更時のオーバーヘッド（完全置き換え）
- メモリ使用量の増大
- 新規BlockType追加時のプレハブ作成作業

### TODO

#### 高優先度
- [x] **単一タイルプレハブの設計と作成**
  - [x] 共通Tileプレハブの作成（Transform + SpriteRenderer）
  - [x] TileControllerコンポーネントの設計
  - [x] BlockType管理機能の実装
  - [x] SpriteManagerをScriptableObjectで実装
    - [x] 各属性のスプライトをインスペクタ上で編集できるようにする
    - [x] SpriteManagerのScriptableObjectのインスタンスをAssets/MyGame/Data/に作成
    - [x] TileControllerはSpriteManagerを参照し、自身の属性に併せて表示するスプライトを切り替える

- [x] **TileControllerコンポーネントの実装**
  - [x] BlockTypeプロパティの実装
  - [x] スプライト切り替えメソッドの実装
  - [x] 初期化処理の実装
  - [x] デバッグ情報表示機能

- [x] **TilemapManagerの変更**
  - [x] プレハブ辞書から単一プレハブ方式への変更
  - [x] PlaceTiles()メソッドの修正（UniversalTile.prefab使用）
  - [x] PlaceTilesWithOverlapProtection()メソッドの修正
  - [x] TileController経由でのBlockType設定処理の実装
  - [x] 従来のプレハブInstantiate処理の置き換え

- [x] **TilemapSystemControllerの作成**
  - [x] TilemapSystemTesterに代わる本実装用コンポーネント作成
  - [x] UniversalTile.prefab対応の初期化処理
  - [x] 公開メソッドによる外部システム連携機能
  - [x] TilemapSystemTesterの廃止予定マーキング

#### 中優先度
- [x] **UpdateTileDisplayメソッドの最適化**
  - [x] Destroy→Instantiate から TileController.SetBlockType()への変更
  - [x] パフォーマンス最適化
  - [x] エラーハンドリング強化

- [x] **複数プレハブの統合・削除**
  - [x] 従来の複数プレハブ（EmptyBlock/GroundBlock等）の段階的削除
  - [x] UniversalTile.prefabへの統一
  - [x] プレハブ参照の整合性確認

- [ ] **テストケースの更新と追加**
  - [ ] MockTilemapManagerの新方式対応
  - [ ] TileControllerのテストケース作成
  - [ ] スプライト切り替え機能のテスト
  - [ ] パフォーマンステストの追加

#### 低優先度
- [ ] **ドキュメント更新**
  - [ ] tilemap_system_spec.mdの実装状況更新
  - [ ] アーキテクチャドキュメントの更新
  - [ ] 使用方法の記録

### 技術仕様

#### アーキテクチャ変更
```
【現在】
TilemapManager → Dictionary<BlockType, GameObject> → 各専用プレハブ

【変更後】
TilemapManager → 単一Tileプレハブ + TileController → スプライト切り替え
```

#### 主要コンポーネント
1. **TileController**: タイル個別の状態とスプライト管理
2. **SpriteManager**: BlockType→Spriteマッピング管理
3. **Modified TilemapManager**: 単一プレハブベースの管理

#### 実装方針
- 既存のBlockType enumはそのまま使用
- ITileBehavior、TileBehaviorインターフェースとの互換性維持
- R3イベントシステムとの連携維持
- 段階的な移行（既存機能を壊さない）

### 完了条件
1. 単一プレハブでのタイル生成が正常に動作する
2. BlockType変更時のスプライト切り替えが正常に動作する
3. 既存のタイル破壊・生成機能が正常に動作する
4. パフォーマンスが現状より向上する
5. 全テストケースが通過する
6. 仕様書の実装状況が更新される

### 想定効果
- **メモリ効率**: プレハブインスタンス数の削減
- **パフォーマンス**: スプライト切り替えの軽量化
- **保守性**: 単一プレハブによる管理簡素化
- **拡張性**: 新BlockType追加時の作業軽減

### 注意事項
- 既存の保存データとの互換性を維持
- PlayerMoveServiceなど他システムとの連携を確認
- Unity Editorでのプレハブ参照設定変更が必要
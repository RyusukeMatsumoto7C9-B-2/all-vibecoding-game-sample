# 使用方法ガイド

## 概要
このドキュメントでは、実装されたスプライト切り替え機能の使用方法について説明します。

## UniversalTileシステムの使用方法

### 1. 基本的な使用方法

#### TilemapSystemControllerの設定
```csharp
// シーン内のTilemapSystemControllerにUniversalTile.prefabを設定
[SerializeField] private GameObject universalTilePrefab;

// TilemapManagerが自動的に初期化される
private void Start()
{
    // 初期マップの生成
    GenerateInitialMap();
}
```

#### SpriteManagerの設定
```csharp
// SpriteManagerのScriptableObjectを作成し、各BlockTypeのスプライトを設定
// Assets/MyGame/Data/SpriteManagerData.asset に保存
```

### 2. 動的なタイル操作

#### BlockTypeの変更
```csharp
// TileControllerを取得してBlockTypeを変更
var tileController = tileGameObject.GetComponent<TileController>();
tileController.BlockType = BlockType.Ground; // スプライトが自動更新される
```

#### 新しいタイルの生成
```csharp
// TilemapManagerを使用してタイルを配置
var mapData = tilemapGenerator.GenerateMap(level, seed);
tilemapManager.PlaceTiles(mapData);
```

### 3. デバッグ機能

#### デバッグモードの有効化
```csharp
// TileControllerのデバッグ機能を有効化
tileController.SetDebugMode(true, true);

// デバッグ情報の取得
string debugInfo = tileController.GetDebugInfo();
Debug.Log(debugInfo);
```

### 4. パフォーマンス最適化

#### メモリ最適化
```csharp
// 現在レベルから離れたマップデータを削除
tilemapManager.OptimizeMemory(currentLevel);
```

#### 効率的なタイル更新
```csharp
// 従来: Destroy + Instantiate（非推奨）
// 新方式: TileController.BlockType変更（推奨）
tileController.BlockType = newBlockType;
```

## テストコードの実行

### EditModeテストの実行
```bash
# Unity Natural MCP Server経由
curl -X POST http://localhost:56780/mcp -H "Content-Type: application/json" -d '{"jsonrpc": "2.0", "method": "tools/call", "params": {"name": "RunEditModeTests", "arguments": {}}, "id": 3}'
```

### パフォーマンステストの確認
```csharp
// TilemapPerformanceTestsクラスでパフォーマンス閾値を確認
// 小さなマップ (10x10): 100ms以下
// 中サイズマップ (50x50): 500ms以下
// タイル更新 (5個): 50ms以下
```

## トラブルシューティング

### よくある問題と解決方法

#### 1. スプライトが表示されない
- SpriteManagerが正しく設定されているか確認
- TileController.SpriteManagerプロパティが設定されているか確認
- BlockTypeに対応するSpriteがSpriteManagerに登録されているか確認

#### 2. パフォーマンスが低下している
- パフォーマンステストを実行して閾値を確認
- メモリ最適化が適切に動作しているか確認
- 不要なDebugLogが有効になっていないか確認

#### 3. テストが失敗する
- 全EditModeテストを実行して問題箇所を特定
- モックオブジェクトが正しく設定されているか確認
- テスト環境でのUnity設定を確認

## 更新履歴
| 日付 | 変更内容 | 担当者 |
|------|----------|--------|
| 2025-07-29 | 初版作成 | Claude |
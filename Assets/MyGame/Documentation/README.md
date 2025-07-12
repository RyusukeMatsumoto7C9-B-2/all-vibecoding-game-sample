# ドキュメント管理

このディレクトリは、プロジェクトのドキュメント管理用です。

## ディレクトリ構造

```
Documentation/
├── README.md           # このファイル
├── Specifications/     # ゲーム仕様書、機能仕様書
├── Tasks/             # タスク実行記録
└── Templates/         # ドキュメントテンプレート
    ├── spec_template.md    # 仕様書テンプレート
    └── task_template.md    # タスク記録テンプレート
```

## 使用方法

### 仕様書の作成
1. `Templates/spec_template.md` をコピー
2. `Specifications/{機能名}_spec.md` として保存
3. テンプレートに従って記入

### タスク記録の作成
1. `Templates/task_template.md` をコピー
2. `Tasks/{YYYY-MM-DD}_{タスク名}.md` として保存
3. テンプレートに従って記入

## 命名規則

### 仕様書
- ファイル名: `{機能名}_spec.md`
- 例: `player_movement_spec.md`

### タスク記録
- ファイル名: `{YYYY-MM-DD}_{タスク名}.md`
- 例: `2025-01-15_player_implementation.md`

## 注意事項

- Claude Codeは自動的にこのディレクトリの仕様書を参照します
- 新機能開発前に必ず関連仕様書を確認してください
- 仕様に不明な点があれば作業を中断し、ユーザに確認を求めてください
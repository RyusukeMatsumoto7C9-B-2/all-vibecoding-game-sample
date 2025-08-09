# ClaudeCodeTools

Unity プロジェクト開発用のツール集

## sprite_generator.py

64x64 スプライト.png作成ツール

### セットアップ

```bash
pip install -r requirements.txt
```

### 使用方法

#### 基本的な使用方法
```bash
python sprite_generator.py --name enemy --color red
```

#### オプション指定
```bash
# エネミー専用パターンで作成
python sprite_generator.py --name enemy --color dark_red --pattern enemy

# 宝物用に金色の星形
python sprite_generator.py --name treasure --color gold --pattern star

# 青いポーション（円形）
python sprite_generator.py --name potion --color blue --pattern circle

# カスタムサイズ（128x128）
python sprite_generator.py --name big_enemy --color purple --pattern enemy --size 128

# 出力先を指定
python sprite_generator.py --name test --color green --output-dir "custom/path"
```

### 対応色
- red, green, blue, yellow, orange, purple, pink, cyan
- gold, silver, brown, black, white, gray
- dark_red, dark_green, dark_blue

### 対応パターン
- square: 正方形
- circle: 円形  
- diamond: ダイヤモンド形
- triangle: 三角形
- star: 星形
- enemy: エネミー専用（目と口付き）

### 出力先
デフォルト: `Assets/MyGame/Sprites/`

### 実行例

エネミー用スプライトを作成:
```bash
python sprite_generator.py --name Enemy --color dark_red --pattern enemy
```

これにより、`Assets/MyGame/Sprites/Enemy.png` が作成されます。
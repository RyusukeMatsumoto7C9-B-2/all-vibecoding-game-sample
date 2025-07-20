# タスク概要
- ユーザによるリファクタリング


# 問題点   
- Unity や System のイベントは利用せずに R3 のリアクティブプロパティないしはサブジェクトで実装する  

# 変更点  
- TileType を BlockType へと変更した(システム名自体はTilemapSystemのままとしている)
  - タイルマップの一要素 : ブロック
  - 全体の構成 : タイルマップ
- MapData 構造体を TilemapDefinition から独立した一つのスクリプトとして定義
- 
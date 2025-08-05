#!/usr/bin/env python3
"""
64x64スプライト.png作成ツール

使用方法:
    python sprite_generator.py --name enemy --color red
    python sprite_generator.py --name treasure --color gold
    python sprite_generator.py --name potion --color blue --pattern circle
"""

import argparse
from PIL import Image, ImageDraw
import os
import sys

# 色の定義
COLORS = {
    'red': (255, 0, 0),
    'green': (0, 255, 0),
    'blue': (0, 0, 255),
    'yellow': (255, 255, 0),
    'orange': (255, 165, 0),
    'purple': (128, 0, 128),
    'pink': (255, 192, 203),
    'cyan': (0, 255, 255),
    'gold': (255, 215, 0),
    'silver': (192, 192, 192),
    'brown': (165, 42, 42),
    'black': (0, 0, 0),
    'white': (255, 255, 255),
    'gray': (128, 128, 128),
    'dark_red': (139, 0, 0),
    'dark_green': (0, 100, 0),
    'dark_blue': (0, 0, 139)
}

def create_basic_sprite(size=64, color=(255, 0, 0), pattern='square'):
    """基本的なスプライトを作成"""
    image = Image.new('RGBA', (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(image)
    
    center = size // 2
    
    if pattern == 'square':
        # 正方形
        margin = size // 8
        draw.rectangle([margin, margin, size - margin, size - margin], 
                      fill=color, outline=(0, 0, 0, 255), width=2)
    
    elif pattern == 'circle':
        # 円形
        margin = size // 8
        draw.ellipse([margin, margin, size - margin, size - margin], 
                    fill=color, outline=(0, 0, 0, 255), width=2)
    
    elif pattern == 'diamond':
        # ダイヤモンド形
        margin = size // 6
        points = [
            (center, margin),              # 上
            (size - margin, center),       # 右
            (center, size - margin),       # 下
            (margin, center)               # 左
        ]
        draw.polygon(points, fill=color, outline=(0, 0, 0, 255))
    
    elif pattern == 'triangle':
        # 三角形
        margin = size // 8
        points = [
            (center, margin),                    # 上
            (size - margin, size - margin),      # 右下
            (margin, size - margin)              # 左下
        ]
        draw.polygon(points, fill=color, outline=(0, 0, 0, 255))
    
    elif pattern == 'star':
        # 星形
        outer_radius = size // 2 - 4
        inner_radius = outer_radius // 2
        points = []
        
        for i in range(10):
            angle = i * 36  # 36度ずつ
            if i % 2 == 0:
                # 外側の頂点
                radius = outer_radius
            else:
                # 内側の頂点
                radius = inner_radius
            
            import math
            x = center + radius * math.cos(math.radians(angle - 90))
            y = center + radius * math.sin(math.radians(angle - 90))
            points.append((x, y))
        
        draw.polygon(points, fill=color, outline=(0, 0, 0, 255))
    
    elif pattern == 'enemy':
        # エネミー専用パターン
        create_enemy_sprite(draw, size, color)
    
    else:
        # デフォルトは正方形
        margin = size // 8
        draw.rectangle([margin, margin, size - margin, size - margin], 
                      fill=color, outline=(0, 0, 0, 255), width=2)
    
    return image

def create_enemy_sprite(draw, size, color):
    """エネミー専用スプライト"""
    center = size // 2
    
    # 体（楕円）
    body_margin = size // 6
    draw.ellipse([body_margin, body_margin + 8, size - body_margin, size - body_margin], 
                fill=color, outline=(0, 0, 0, 255), width=2)
    
    # 目（小さな白い円）
    eye_size = 6
    eye_y = center - 8
    # 左目
    draw.ellipse([center - 12, eye_y, center - 12 + eye_size, eye_y + eye_size], 
                fill=(255, 255, 255), outline=(0, 0, 0, 255))
    # 右目
    draw.ellipse([center + 6, eye_y, center + 6 + eye_size, eye_y + eye_size], 
                fill=(255, 255, 255), outline=(0, 0, 0, 255))
    
    # 瞳（小さな黒い円）
    pupil_size = 2
    # 左瞳
    draw.ellipse([center - 10, eye_y + 2, center - 10 + pupil_size, eye_y + 2 + pupil_size], 
                fill=(0, 0, 0))
    # 右瞳
    draw.ellipse([center + 8, eye_y + 2, center + 8 + pupil_size, eye_y + 2 + pupil_size], 
                fill=(0, 0, 0))
    
    # 口（小さな線）
    mouth_y = center + 4
    draw.line([center - 6, mouth_y, center + 6, mouth_y], fill=(0, 0, 0), width=2)

def main():
    parser = argparse.ArgumentParser(description='64x64スプライト.png作成ツール')
    parser.add_argument('--name', required=True, help='スプライトの名前')
    parser.add_argument('--color', default='red', choices=COLORS.keys(), 
                       help='スプライトの色')
    parser.add_argument('--pattern', default='square', 
                       choices=['square', 'circle', 'diamond', 'triangle', 'star', 'enemy'],
                       help='スプライトのパターン')
    parser.add_argument('--size', type=int, default=64, help='スプライトのサイズ（デフォルト: 64）')
    parser.add_argument('--output-dir', default='Assets/MyGame/Sprites', 
                       help='出力ディレクトリ（デフォルト: Assets/MyGame/Sprites）')
    
    args = parser.parse_args()
    
    # 出力ディレクトリの作成
    os.makedirs(args.output_dir, exist_ok=True)
    
    # 色の取得
    if args.color not in COLORS:
        print(f"エラー: 未対応の色 '{args.color}'")
        print(f"対応色: {', '.join(COLORS.keys())}")
        return 1
    
    color = COLORS[args.color]
    
    # スプライト作成
    sprite = create_basic_sprite(args.size, color, args.pattern)
    
    # ファイル名の生成
    filename = f"{args.name}.png"
    output_path = os.path.join(args.output_dir, filename)
    
    # 保存
    sprite.save(output_path, 'PNG')
    
    print(f"スプライト作成完了: {output_path}")
    print(f"  - サイズ: {args.size}x{args.size}")
    print(f"  - 色: {args.color}")
    print(f"  - パターン: {args.pattern}")
    
    return 0

if __name__ == '__main__':
    try:
        sys.exit(main())
    except Exception as e:
        print(f"エラー: {e}")
        sys.exit(1)
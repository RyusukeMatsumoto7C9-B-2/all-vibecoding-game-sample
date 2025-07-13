#!/usr/bin/env python3
"""
Create simple sprite textures for Unity tilemap system
"""

from PIL import Image
import os

def create_sprite(color, filename, size=(32, 32)):
    """Create a simple colored sprite"""
    img = Image.new('RGBA', size, color)
    img.save(filename)
    print(f"Created {filename}")

# Create sprites directory if it doesn't exist
sprites_dir = "Assets/MyGame/Sprites"
os.makedirs(sprites_dir, exist_ok=True)

# Create wall sprite (gray)
create_sprite((128, 128, 128, 255), os.path.join(sprites_dir, "WallSprite.png"))

# Create ground sprite (brown)
create_sprite((160, 100, 50, 255), os.path.join(sprites_dir, "GroundSprite.png"))

print("Sprite creation completed!")
using System;
using UnityEngine;

namespace MyGame.TilemapSystem.Core
{
    [CreateAssetMenu(fileName = "SpriteManagerData", menuName = "MyGame/Tilemap/SpriteManager")]
    public class SpriteManager : ScriptableObject
    {
        [System.Serializable]
        public class BlockTypeSprite
        {
            public BlockType blockType;
            public Sprite sprite;
        }
        
        [SerializeField] private BlockTypeSprite[] _blockTypeSprites = new BlockTypeSprite[]
        {
            new BlockTypeSprite { blockType = BlockType.Sky, sprite = null },
            new BlockTypeSprite { blockType = BlockType.Empty, sprite = null },
            new BlockTypeSprite { blockType = BlockType.Ground, sprite = null },
            new BlockTypeSprite { blockType = BlockType.Rock, sprite = null },
            new BlockTypeSprite { blockType = BlockType.Treasure, sprite = null }
        };
        
        public Sprite GetSpriteForBlockType(BlockType blockType)
        {
            foreach (var blockTypeSprite in _blockTypeSprites)
            {
                if (blockTypeSprite.blockType == blockType)
                {
                    return blockTypeSprite.sprite;
                }
            }
            
            Debug.LogWarning($"[SpriteManager] BlockType '{blockType}' に対応するSpriteが見つかりません。");
            return null;
        }
        
        public bool HasSpriteForBlockType(BlockType blockType)
        {
            foreach (var blockTypeSprite in _blockTypeSprites)
            {
                if (blockTypeSprite.blockType == blockType)
                {
                    return blockTypeSprite.sprite != null;
                }
            }
            return false;
        }
        
        public void SetSpriteForBlockType(BlockType blockType, Sprite sprite)
        {
            for (int i = 0; i < _blockTypeSprites.Length; i++)
            {
                if (_blockTypeSprites[i].blockType == blockType)
                {
                    _blockTypeSprites[i].sprite = sprite;
                    return;
                }
            }
            
            Debug.LogWarning($"[SpriteManager] BlockType '{blockType}' の設定項目が見つかりません。");
        }
        
        public BlockTypeSprite[] GetAllBlockTypeSprites()
        {
            return _blockTypeSprites;
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            int spriteCount = 0;
            foreach (var blockTypeSprite in _blockTypeSprites)
            {
                if (blockTypeSprite.sprite != null)
                    spriteCount++;
            }
            Debug.Log($"[SpriteManager] 設定済みスプライト数: {spriteCount}/{_blockTypeSprites.Length}");
        }
        #endif
    }
}
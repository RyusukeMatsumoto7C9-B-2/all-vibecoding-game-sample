using UnityEngine;

namespace MyGame.TilemapSystem.Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TileController : MonoBehaviour
    {
        [Header("Tile Settings")]
        [SerializeField] private BlockType _blockType = BlockType.Empty;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteManager _spriteManager;
        
        [Header("Debug Settings")]
        [SerializeField] private bool _enableDebugLog = false;
        [SerializeField] private bool _showDebugInfo = false;
        
        private BlockType _previousBlockType;
        private bool _isInitialized = false;
        
        public BlockType BlockType
        {
            get => _blockType;
            set
            {
                if (_blockType != value)
                {
                    var oldType = _blockType;
                    _blockType = value;
                    
                    if (_enableDebugLog)
                    {
                        Debug.Log($"[TileController] BlockType変更: {oldType} → {_blockType} (Position: {transform.position})");
                    }
                    
                    UpdateSprite();
                }
            }
        }
        
        public SpriteManager SpriteManager
        {
            get => _spriteManager;
            set
            {
                if (_spriteManager != value)
                {
                    _spriteManager = value;
                    
                    if (_enableDebugLog)
                    {
                        Debug.Log($"[TileController] SpriteManager設定: {(_spriteManager != null ? "有効" : "無効")} (Position: {transform.position})");
                    }
                    
                    UpdateSprite();
                }
            }
        }
        
        public bool IsInitialized => _isInitialized;
        public Vector3 Position => transform.position;
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            if (!_isInitialized)
            {
                Initialize(_blockType, _spriteManager);
            }
        }
        
        private void InitializeComponents()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
                
                if (_spriteRenderer == null)
                {
                    Debug.LogError($"[TileController] SpriteRendererが見つかりません。RequireComponentが設定されているか確認してください。(Position: {transform.position})");
                }
            }
        }
        
        public void Initialize(BlockType blockType, SpriteManager spriteManager = null)
        {
            if (_enableDebugLog)
            {
                Debug.Log($"[TileController] 初期化開始: BlockType={blockType}, SpriteManager={spriteManager != null} (Position: {transform.position})");
            }
            
            _blockType = blockType;
            _previousBlockType = blockType;
            
            if (spriteManager != null)
            {
                _spriteManager = spriteManager;
            }
            
            InitializeComponents();
            
            if (_spriteManager == null)
            {
                Debug.LogWarning($"[TileController] SpriteManagerが設定されていません。スプライト表示ができません。(Position: {transform.position})");
            }
            
            UpdateSprite();
            _isInitialized = true;
            
            if (_enableDebugLog)
            {
                Debug.Log($"[TileController] 初期化完了: BlockType={_blockType} (Position: {transform.position})");
            }
        }
        
        public bool ChangeBlockType(BlockType newBlockType, bool forceUpdate = false)
        {
            if (!forceUpdate && _blockType == newBlockType)
            {
                if (_enableDebugLog)
                {
                    Debug.Log($"[TileController] BlockType変更スキップ: 既に{newBlockType}です (Position: {transform.position})");
                }
                return false;
            }
            
            _previousBlockType = _blockType;
            BlockType = newBlockType;
            return true;
        }
        
        public void ForceUpdateSprite()
        {
            if (_enableDebugLog)
            {
                Debug.Log($"[TileController] スプライト強制更新 (Position: {transform.position})");
            }
            
            UpdateSprite();
        }
        
        private void UpdateSprite()
        {
            if (_spriteRenderer == null)
            {
                if (_enableDebugLog)
                {
                    Debug.LogWarning($"[TileController] SpriteRendererがnullのため、スプライト更新をスキップ (Position: {transform.position})");
                }
                return;
            }
            
            if (_spriteManager == null)
            {
                if (_enableDebugLog)
                {
                    Debug.LogWarning($"[TileController] SpriteManagerがnullのため、スプライト更新をスキップ (Position: {transform.position})");
                }
                return;
            }
            
            var sprite = _spriteManager.GetSpriteForBlockType(_blockType);
            _spriteRenderer.sprite = sprite;
            
            // EmptyブロックはSprite非表示
            bool shouldBeVisible = _blockType != BlockType.Empty && sprite != null;
            _spriteRenderer.enabled = shouldBeVisible;
            
            if (_enableDebugLog)
            {
                Debug.Log($"[TileController] スプライト更新: {_blockType}, 表示={shouldBeVisible} (Position: {transform.position})");
            }
        }
        
        public void SetVisible(bool visible)
        {
            if (_spriteRenderer != null)
            {
                bool shouldBeVisible = visible && _blockType != BlockType.Empty;
                _spriteRenderer.enabled = shouldBeVisible;
                
                if (_enableDebugLog)
                {
                    Debug.Log($"[TileController] 表示設定: {shouldBeVisible} (Position: {transform.position})");
                }
            }
        }
        
        public void SetDebugMode(bool enableDebugLog, bool showDebugInfo = false)
        {
            _enableDebugLog = enableDebugLog;
            _showDebugInfo = showDebugInfo;
        }
        
        public string GetDebugInfo()
        {
            return $"Position: {transform.position}, BlockType: {_blockType}, " +
                   $"PreviousType: {_previousBlockType}, Initialized: {_isInitialized}, " +
                   $"SpriteManager: {(_spriteManager != null ? "OK" : "NULL")}, " +
                   $"SpriteRenderer: {(_spriteRenderer != null ? "OK" : "NULL")}, " +
                   $"Visible: {(_spriteRenderer != null ? _spriteRenderer.enabled : false)}";
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            InitializeComponents();
            
            if (Application.isPlaying && _isInitialized)
            {
                UpdateSprite();
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (_showDebugInfo)
            {
                UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, 
                    $"{_blockType}\n{transform.position}");
            }
        }
        #endif
    }
}
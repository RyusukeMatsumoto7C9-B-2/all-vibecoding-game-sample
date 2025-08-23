using UnityEngine;
using MyGame.TilemapSystem;
using MyGame.TilemapSystem.Core;
using MyGame.Common;
using VContainer;
using R3;
using System;

namespace MyGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        private readonly float _moveSpeed = 5f;

        private PlayerInputHandler _inputHandler;
        private PlayerMover _mover;
        private Vector3 _targetPosition;
        private bool _isMoving;
        private  ITilemapService _tilemapService;
        private IDisposable _inputSubscription;
        private IDisposable _scrollStartedSubscription;
        private IDisposable _scrollCompletedSubscription;
        private TilemapSystemController _tilemapSystemController;

        [Inject]
        public void Construct(ITilemapService tilemapManager)
        {
            _tilemapService = tilemapManager;
            
            // TilemapSystemControllerを取得（同じITilemapServiceインスタンス）
            _tilemapSystemController = tilemapManager as TilemapSystemController;
        }

        private void Awake()
        {
            if (_inputHandler == null)
                _inputHandler = GetComponent<PlayerInputHandler>();
            _mover = new PlayerMover(_tilemapService);
        }

        private void Start()
        {
            SetInitialPosition();
            
            // PlayerをTilemapSystemControllerの子オブジェクトとして配置
            // これにより、タイルマップのスクロールと一緒にPlayerも移動する
            if (_tilemapSystemController != null)
            {
                transform.SetParent(_tilemapSystemController.transform, true);
                Debug.Log("[PlayerController] TilemapSystemControllerの子オブジェクトとして配置");
            }
        }

        private void OnEnable()
        {
            if (_inputHandler != null)
            {
                _inputSubscription = _inputHandler.OnMoveInput.Subscribe(HandleMoveInput);
            }
            
            // スクロールイベントを購読
            if (_tilemapSystemController != null && _tilemapSystemController.ScrollController != null)
            {
                _scrollStartedSubscription = _tilemapSystemController.ScrollController.OnScrollStarted.Subscribe(OnScrollStarted);
                _scrollCompletedSubscription = _tilemapSystemController.ScrollController.OnScrollCompleted.Subscribe(OnScrollCompleted);
            }
        }

        private void OnDisable()
        {
            _inputSubscription?.Dispose();
            _scrollStartedSubscription?.Dispose();
            _scrollCompletedSubscription?.Dispose();
        }

        private void Update()
        {
            if (_isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
                
                if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
                {
                    transform.position = _targetPosition;
                    _isMoving = false;
                }
            }
        }


        private void SetInitialPosition()
        {
            var initialPosition = new Vector2Int(10, 15);
            _mover.SetPosition(initialPosition);
            
            var worldPosition = _tilemapService?.GetPosition(initialPosition.x, initialPosition.y) ?? new Vector3(initialPosition.x, initialPosition.y, 0);
            transform.position = worldPosition;
            _targetPosition = worldPosition;
        }

        private void HandleMoveInput(Direction direction)
        {
            if (_isMoving) return;

            var currentPos = _mover.CurrentPosition;
            Debug.Log($"[PlayerController] 移動入力: {direction}, 現在位置: ({currentPos.x}, {currentPos.y})");

            if (_mover.Move(direction))
            {
                var newPosition = _mover.CurrentPosition;
                _targetPosition = _tilemapService?.GetPosition(newPosition.x, newPosition.y) ?? new Vector3(newPosition.x, newPosition.y, 0);
                _isMoving = true;
                Debug.Log($"[PlayerController] 移動成功: ({newPosition.x}, {newPosition.y})");
            }
            else
            {
                Debug.Log($"[PlayerController] 移動失敗: {direction}方向への移動が制限されています");
            }
        }
        
        private void OnScrollStarted(int currentLevel)
        {
            Debug.Log($"[PlayerController] スクロール開始: レベル {currentLevel}");
        }
        
        private void OnScrollCompleted(int newLevel)
        {
            Debug.Log($"[PlayerController] スクロール完了: 新しいレベル {newLevel}");
            
            // スクロール完了後、Playerのレベルを更新
            _mover.SetCurrentLevel(newLevel);
            
            // PlayerがTilemapSystemControllerの子オブジェクトなら、
            // スクロールによって自動的に位置が更新されるため、
            // 追加の位置調整は不要
            Debug.Log($"[PlayerController] 現在のワールド位置: {transform.position}");
        }
    }
}
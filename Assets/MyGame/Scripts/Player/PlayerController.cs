using UnityEngine;
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
        private PlayerMoveService _moveService;
        private Vector3 _targetPosition;
        private bool _isMoving;
        private  ITilemapManager _tilemapManager;
        private IDisposable _inputSubscription;

        [Inject]
        public void Construct(ITilemapManager tilemapManager)
        {
            _tilemapManager = tilemapManager;
        }

        private void Awake()
        {
            if (_inputHandler == null)
                _inputHandler = GetComponent<PlayerInputHandler>();
            _moveService = new PlayerMoveService(_tilemapManager);
        }

        private void Start()
        {
            SetInitialPosition();
        }

        private void OnEnable()
        {
            if (_inputHandler != null)
            {
                _inputSubscription = _inputHandler.OnMoveInput.Subscribe(HandleMoveInput);
            }
        }

        private void OnDisable()
        {
            _inputSubscription?.Dispose();
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
            _moveService.SetPosition(initialPosition);
            
            var worldPosition = _tilemapManager?.GetPosition(initialPosition.x, initialPosition.y) ?? new Vector3(initialPosition.x, initialPosition.y, 0);
            transform.position = worldPosition;
            _targetPosition = worldPosition;
        }

        private void HandleMoveInput(Direction direction)
        {
            if (_isMoving) return;

            var currentPos = _moveService.CurrentPosition;
            Debug.Log($"[PlayerController] 移動入力: {direction}, 現在位置: ({currentPos.x}, {currentPos.y})");

            if (_moveService.Move(direction))
            {
                var newPosition = _moveService.CurrentPosition;
                _targetPosition = _tilemapManager?.GetPosition(newPosition.x, newPosition.y) ?? new Vector3(newPosition.x, newPosition.y, 0);
                _isMoving = true;
                Debug.Log($"[PlayerController] 移動成功: ({newPosition.x}, {newPosition.y})");
            }
            else
            {
                Debug.Log($"[PlayerController] 移動失敗: {direction}方向への移動が制限されています");
            }
        }
    }
}
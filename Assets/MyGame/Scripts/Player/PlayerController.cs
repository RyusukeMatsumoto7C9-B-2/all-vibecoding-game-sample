using UnityEngine;
using MyGame.TilemapSystem.Core;

namespace MyGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputHandler _inputHandler;
        [SerializeField] private float _moveSpeed = 5f;

        private PlayerMoveService _moveService;
        private Vector3 _targetPosition;
        private bool _isMoving;

        private void Awake()
        {
            _moveService = new PlayerMoveService();
            
            if (_inputHandler == null)
                _inputHandler = GetComponent<PlayerInputHandler>();
        }

        private void Start()
        {
            SetInitialPosition();
        }

        private void OnEnable()
        {
            if (_inputHandler != null)
                _inputHandler.OnMoveInput += HandleMoveInput;
        }

        private void OnDisable()
        {
            if (_inputHandler != null)
                _inputHandler.OnMoveInput -= HandleMoveInput;
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

        public void SetTilemapManager(TilemapManager tilemapManager, int level = 0)
        {
            _moveService.SetTilemapManager(tilemapManager, level);
        }

        private void SetInitialPosition()
        {
            var initialPosition = new Vector2Int(10, 15);
            _moveService.SetPosition(initialPosition);
            transform.position = new Vector3(initialPosition.x, initialPosition.y, 0);
            _targetPosition = transform.position;
        }

        private void HandleMoveInput(Direction direction)
        {
            if (_isMoving) return;

            if (_moveService.Move(direction))
            {
                var newPosition = _moveService.CurrentPosition;
                _targetPosition = new Vector3(newPosition.x, newPosition.y, 0);
                _isMoving = true;
            }
            // 移動できない場合は何もしない（移動アニメーションを開始しない）
        }
    }
}
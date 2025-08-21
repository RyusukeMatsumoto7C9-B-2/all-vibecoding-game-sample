using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.Common;

namespace MyGame.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 3f;

        private EnemyMoveService _moveService;
        private Vector3 _targetPosition;
        private bool _isMoving;

        private void Awake()
        {
            _moveService = new EnemyMoveService();
        }

        private void Start()
        {
            SetInitialPosition();
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

        public void SetTilemapManager(ITilemapService tilemapManager, int level = 0)
        {
            _moveService.SetTilemapManager(tilemapManager, level);
        }

        public void SetPosition(Vector2Int position)
        {
            _moveService.SetPosition(position);
            transform.position = new Vector3(position.x, position.y, 0);
            _targetPosition = transform.position;
        }

        public bool Move(Direction direction)
        {
            if (_isMoving) return false;

            if (_moveService.Move(direction))
            {
                var newPosition = _moveService.CurrentPosition;
                _targetPosition = new Vector3(newPosition.x, newPosition.y, 0);
                _isMoving = true;
                return true;
            }

            return false;
        }

        public bool CanMove(Direction direction)
        {
            return _moveService.CanMove(direction);
        }

        public Vector2Int CurrentPosition => _moveService.CurrentPosition;

        private void SetInitialPosition()
        {
            var initialPosition = new Vector2Int(0, 15);
            SetPosition(initialPosition);
        }
    }
}
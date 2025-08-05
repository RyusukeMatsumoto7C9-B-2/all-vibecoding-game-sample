using UnityEngine;
using MyGame.TilemapSystem.Core;
using MyGame.Common;
using System.Linq;

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
            AutoDetectTilemapManager();
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
            Debug.Log($"[PlayerController] TilemapManager設定完了 - Level: {level}");
        }

        private void AutoDetectTilemapManager()
        {
            // シーン内でTilemapManagerを使用しているコンポーネントを検索
            var tilemapControllers = FindObjectsOfType<MonoBehaviour>()
                .Where(mb => mb.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                    .Any(field => field.FieldType == typeof(TilemapManager)))
                .ToList();

            Debug.Log($"[PlayerController] TilemapManagerを持つコンポーネントを{tilemapControllers.Count}個発見");

            if (tilemapControllers.Count == 0)
            {
                Debug.LogWarning("[PlayerController] TilemapManagerが見つかりません。移動制約が機能しない可能性があります。");
                Debug.LogWarning("[PlayerController] シーン内のGameObjectでSetTilemapManager()を呼び出してください。");
            }
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

            var currentPos = _moveService.CurrentPosition;
            Debug.Log($"[PlayerController] 移動入力: {direction}, 現在位置: ({currentPos.x}, {currentPos.y})");

            if (_moveService.Move(direction))
            {
                var newPosition = _moveService.CurrentPosition;
                _targetPosition = new Vector3(newPosition.x, newPosition.y, 0);
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
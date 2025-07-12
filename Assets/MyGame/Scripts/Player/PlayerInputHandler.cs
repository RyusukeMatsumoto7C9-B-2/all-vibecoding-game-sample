using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyGame.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public event Action<Direction> OnMoveInput;

        private InputAction _moveAction;

        private void Awake()
        {
            _moveAction = new InputAction();
            _moveAction.AddBinding("<Keyboard>/w").WithInteraction("press");
            _moveAction.AddBinding("<Keyboard>/a").WithInteraction("press");
            _moveAction.AddBinding("<Keyboard>/s").WithInteraction("press");
            _moveAction.AddBinding("<Keyboard>/d").WithInteraction("press");
        }

        private void OnEnable()
        {
            _moveAction.Enable();
            _moveAction.performed += OnMovePerformed;
        }

        private void OnDisable()
        {
            _moveAction.performed -= OnMovePerformed;
            _moveAction.Disable();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            var inputKey = context.control.name;
            
            Direction direction = inputKey switch
            {
                "w" => Direction.Up,
                "s" => Direction.Down,
                "a" => Direction.Left,
                "d" => Direction.Right,
                _ => throw new ArgumentException($"Unexpected input key: {inputKey}")
            };

            OnMoveInput?.Invoke(direction);
        }

        private void OnDestroy()
        {
            _moveAction?.Dispose();
        }
    }
}
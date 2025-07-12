using UnityEngine;

namespace MyGame.Player
{
    public class PlayerMoveService
    {
        public Vector2Int CurrentPosition { get; private set; }

        public void SetPosition(Vector2Int position)
        {
            CurrentPosition = position;
        }

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    CurrentPosition = new Vector2Int(CurrentPosition.x, CurrentPosition.y + 1);
                    break;
                case Direction.Down:
                    CurrentPosition = new Vector2Int(CurrentPosition.x, CurrentPosition.y - 1);
                    break;
                case Direction.Left:
                    CurrentPosition = new Vector2Int(CurrentPosition.x - 1, CurrentPosition.y);
                    break;
                case Direction.Right:
                    CurrentPosition = new Vector2Int(CurrentPosition.x + 1, CurrentPosition.y);
                    break;
            }
        }
    }
}
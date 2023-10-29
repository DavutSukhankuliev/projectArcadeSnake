using UnityEngine;

namespace ArcadeSnake
{
    public class SnakeMovePosition
    {
        private SnakeMovePosition _previousSnakeMovePosition;
        private Vector3 _movePosition;
        private Direction _direction;

        public SnakeMovePosition(
            SnakeMovePosition previousSnakeMovePosition,
            Vector3 movePosition,
            Direction direction)
        {
            _previousSnakeMovePosition = previousSnakeMovePosition;
            _movePosition = movePosition;
            _direction = direction;
        }


        public Vector3 GetGridPosition() 
        {
            return _movePosition;
        }
        public Vector3 GetGridPreviousPosition() 
        {
            return _previousSnakeMovePosition.GetGridPosition();
        }

        public Direction GetDirection() 
        {
            return _direction;
        }

        public Direction GetPreviousDirection() 
        {
            if (_previousSnakeMovePosition == null) 
            {
                return Direction.Right;
            } 
            else 
            {
                return _previousSnakeMovePosition._direction;
            }
        }
    }
}
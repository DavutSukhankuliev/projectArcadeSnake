using DG.Tweening;
using UnityEngine;
using Zenject;

namespace ArcadeSnake
{
    public class SnakeBodyPart 
    {
        private readonly IInstantiator _instantiator;
        private readonly SnakeConfig _snakeConfig;
        private readonly GridConfig _gridConfig;

        private SnakeMovePosition _snakeMovePosition;
        private Transform _transform;

        private Snake _body;

        private bool _edgeCrossed;
   
        public SnakeBodyPart(
            Vector3 pos,
            IInstantiator instantiator,
            int snakeBodySize,
            GridConfig gridConfig)
        {
            _instantiator = instantiator;
            _gridConfig = gridConfig;

            var command = _instantiator.Instantiate<CreateBodyPartCommand>(
                new object[] {
                    new SceneObjectProtocol(new Vector3(pos.x,pos.y))});
       
            command.Execute();

            _body = command.GetBody();
            SpriteRenderer spriteRenderer = _body.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 5 - snakeBodySize;
            _transform = _body.transform;
        }

        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition,float speed/*, Sequence sequence*/) 
        {
            _snakeMovePosition = snakeMovePosition;

            float angle = snakeMovePosition.GetDirection() switch
            {
                Direction.Up => snakeMovePosition.GetPreviousDirection() switch
                {
                    Direction.Left => 0 + 45, Direction.Right => 0 - 45, _ => 0
                },
                Direction.Down => snakeMovePosition.GetPreviousDirection() switch
                {
                    Direction.Left => 180 - 45, Direction.Right => 180 + 45, _ => 180
                },
                Direction.Left => snakeMovePosition.GetPreviousDirection() switch
                {
                    Direction.Down => 180 - 45, Direction.Up => 45, _ => +90
                },
                Direction.Right => snakeMovePosition.GetPreviousDirection() switch
                {
                    Direction.Down => 180 + 45, Direction.Up => -45, _ => -90
                },
                _ => snakeMovePosition.GetPreviousDirection() switch
                {
                    Direction.Left => 0 + 45, Direction.Right => 0 - 45, _ => 0
                }
            };
            
            
            if (snakeMovePosition.GetGridPreviousPosition().x == _gridConfig.Width - 1 && snakeMovePosition.GetGridPosition().x == 1)
            {
                _transform.DOKill();
               
                var pos = _transform.position;
                pos.x = 0;
                _transform.position = pos;
            }
           
            
            if (snakeMovePosition.GetGridPreviousPosition().y == _gridConfig.Height - 1 && snakeMovePosition.GetGridPosition().y == 1)
            {
                _transform.DOKill();
                    
                var pos = _transform.position;
                pos.y = 0;
                _transform.position = pos;
            }
            if (snakeMovePosition.GetGridPreviousPosition().x == 0 && snakeMovePosition.GetGridPosition().x == _gridConfig.Width - 2)
            {
                _transform.DOKill();
                    
                var pos = _transform.position;
                pos.x = _gridConfig.Width-1;
                _transform.position = pos;
            }
            if (snakeMovePosition.GetGridPreviousPosition().y == 0 && snakeMovePosition.GetGridPosition().y == _gridConfig.Height - 2)
            {
                _transform.DOKill();
                    
                var pos = _transform.position;
                pos.y = _gridConfig.Height-1;;
                _transform.position = pos;
            }

            _transform.DOMove(snakeMovePosition.GetGridPosition(),speed).SetEase(Ease.Linear);

            _transform.eulerAngles = new Vector3(0, 0, angle);
        }

        public void Death()
        {
            _body.Dispose();
        }
    }
}
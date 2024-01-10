namespace ArcadeSnake
{
    public class GridCreateCommand : Command 
    {
        private readonly GridCell.Pool _pool;
        private readonly SceneObjectProtocol _protocol;

        public GridCreateCommand(GridCell.Pool pool, 
            SceneObjectProtocol protocol)
        {
            _pool = pool;
            _protocol = protocol;
        }
        public override void Execute()
        {
            _pool.Spawn(_protocol);
            OnDone();
        }
    }
}

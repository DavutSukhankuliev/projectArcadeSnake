namespace ArcadeSnake
{
    public class BackCreateCommand : Command
    {
        private readonly Back.Pool _pool;
        private readonly SceneObjectProtocol _protocol;

        public BackCreateCommand(Back.Pool pool,
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

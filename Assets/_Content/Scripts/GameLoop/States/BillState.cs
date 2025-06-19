namespace Quackery
{
    public class BillState : GameState
    {
        public BillState(GameLoop gameLoop) : base(gameLoop)
        { }

        public override void Enter()
        {
            _gameLoop.BillApp.Show();
        }
    }
}
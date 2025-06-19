namespace Quackery
{
    internal class ShopState : GameState
    {

        public ShopState(GameLoop gameLoop) : base(gameLoop)
        { }
        public override void Enter()
        {
            _gameLoop.ShopApp.Show();
        }

    }
}
using Quackery.GameMenu;

namespace Quackery
{
    internal class ShopState : GameState
    {

        public ShopState(GameLoop gameLoop) : base(gameLoop)
        { }
        public override void Enter()
        {
            GameMenuController.ShowRequest();
            _gameLoop.ShopApp.Open();
        }

    }
}
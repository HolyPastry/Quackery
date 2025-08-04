using Quackery.GameMenu;

namespace Quackery
{
    public class BillState : GameState
    {
        public BillState(GameLoop gameLoop) : base(gameLoop)
        { }

        public override void Enter()
        {
            GameMenuController.HideRequest();
            _gameLoop.BillApp.Open();
        }
    }
}
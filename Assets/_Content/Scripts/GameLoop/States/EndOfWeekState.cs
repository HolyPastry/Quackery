using Quackery.GameMenu;

namespace Quackery
{
    internal class EndOfWeekState : GameState
    {
        public EndOfWeekState(GameLoop gameLoop) : base(gameLoop)
        { }

        public override void Enter()
        {
            GameMenuController.ShowRequest();
            _gameLoop.EndOfWeekApp.Open();
        }

        public override void Exit()
        {
            _gameLoop.EndOfWeekApp.Close();
        }
    }
}

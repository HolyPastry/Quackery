using Quackery.Clients;
using Quackery.GameMenu;

namespace Quackery
{
    internal class NotificationState : GameState
    {
        public NotificationState(GameLoop stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            ClientServices.GenerateDailyQueue();
            GameMenuController.ShowRequest();
            _gameLoop.NotificationApp.Open();
        }
    }
}
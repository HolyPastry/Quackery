using Quackery.Clients;

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
            _gameLoop.NotificationApp.Open();
        }
    }
}
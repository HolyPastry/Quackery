using Quackery.Clients;

namespace Quackery
{
    public class PassiveClientChatState : GameState
    {
        public PassiveClientChatState(GameLoop gameLoop) : base(gameLoop)
        { }
        public override void Enter()
        {
            _gameLoop.ClientTextChat.Show(CustomerPanelSize.Long);
            _gameLoop.ClientTextChat.OnExitingChat += OnBackButtonPressed;
        }

        public override void Exit()
        {
            _gameLoop.ClientTextChat.OnExitingChat -= OnBackButtonPressed;
            _gameLoop.ClientTextChat.Hide();
        }

        private void OnBackButtonPressed()
        {
            ClientServices.ClientLeaves();
        }

    }
}
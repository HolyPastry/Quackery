using System;
using Quackery.Clients;
using Quackery.StateMachines;

namespace Quackery
{

    public class MainScreenState : GameState
    {
        public MainScreenState(GameLoop stateMachine) : base(stateMachine)
        {
        }
    }
    public class ClientListState : GameState
    {

        public ClientListState(GameLoop gameLoop) : base(gameLoop)
        { }

        public override void Enter()
        {
            _gameLoop.ChatApp.Show();
        }


    }
}



using System;
using System.Collections;
using Quackery.Clients;
using Quackery.Decks;
using Quackery.StateMachines;
using UnityEngine;

namespace Quackery
{
    public class GameLoop : StateMachine
    {
        public CardGameController CardGameController;
        //   public ClientListUI ClientListUI;
        public ClientChatRotatingPanels ClientTextChat;

        [SerializeField] private AppData _chatApp;

        private Client SelectedClient => ClientServices.SelectedClient();
        private bool ActiveClientSelected => SelectedClient != null
                && SelectedClient.IsInQueue && !SelectedClient.Served;
        private bool PassiveClientSelected =>
                SelectedClient != null &&
                (!SelectedClient.IsInQueue || SelectedClient.Served);


        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            yield return ClientServices.WaitUntilReady();
            InitLoop();
        }

        private void InitLoop()
        {
            var clientListState = new ClientListState(this);
            var activeClientChatState = new ActiveClientChatState(this);
            var passiveClientChatState = new PassiveClientChatState(this);
            var mainMenuState = new MainScreenState(this);

            At(mainMenuState, clientListState, () => AppServices.IsAppSelected(_chatApp));

            At(clientListState, activeClientChatState, () => ActiveClientSelected);
            At(activeClientChatState, clientListState, () => SelectedClient == null);

            At(clientListState, passiveClientChatState, () => PassiveClientSelected);
            At(passiveClientChatState, clientListState, () => SelectedClient == null);

            ChangeState(mainMenuState);

        }
    }
}
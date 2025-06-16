using System.Collections;
using Quackery.Clients;
using Unity.VisualScripting;
using UnityEngine;

namespace Quackery
{
    public class ActiveClientChatState : GameState
    {
        public ActiveClientChatState(GameLoop gameLoop) : base(gameLoop)
        { }

        public override IEnumerator StateRoutine()
        {
            var controller = _gameLoop.CardGameController;
            var textChat = _gameLoop.ClientTextChat;

            textChat.Show(CustomerPanelSize.Short);

            DialogQueueServices.QueueDialog("MeIntro");
            yield return DialogQueueServices.WaitUntilAllDialogEnds();
            yield return new WaitForSeconds(0.3f);
            controller.Show();
            yield return new WaitForSeconds(1f);
            bool firstTime = true;
            while (true)
            {
                var client = ClientServices.SelectedClient();
                if (!firstTime)
                {
                    DialogQueueServices.QueueDialog("MeIntro");
                    yield return DialogQueueServices.WaitUntilAllDialogEnds();

                }
                firstTime = false;
                controller.StartNewRound();

                yield return controller.WaitUntilEndOfRound();

                DialogQueueServices.QueueDialog("MeConclusion");
                DialogQueueServices.QueueDialog($"{client.DialogKey}Success");

                controller.ResetDeck();
                yield return DialogQueueServices.WaitUntilAllDialogEnds();

                controller.TransfertCartToPurse();
                EffectServices.CleanEffects();
                ClientServices.ClientServed(client.Data);
                if (ClientServices.HasNextClient())
                {
                    ClientServices.GetNextClient();
                    yield return textChat.WaitUntilClientCameIn();
                }
                else
                {
                    ClientServices.ClientLeaves();
                    break;
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
            _gameLoop.CardGameController.Hide();
            _gameLoop.ClientTextChat.Hide();
        }
    }
}
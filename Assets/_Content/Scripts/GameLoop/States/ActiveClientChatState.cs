using System.Collections;
using Quackery.Clients;

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
                controller.StartNewRound(client);

                yield return controller.WaitUntilEndOfRound();

                if (controller.RoundInterrupted)
                {
                    client.BadReview();
                    ClientServices.ClientServed(client.Data);

                }
                else
                {
                    DialogQueueServices.QueueDialog("MeConclusion");
                    DialogQueueServices.QueueDialog($"{client.DialogKey}Success");
                    controller.TransfertCartToPurse();
                    client.GoodReview();
                    ClientServices.ClientServed(client.Data);
                    yield return DialogQueueServices.WaitUntilAllDialogEnds();
                }

                controller.ResetDeck();
                EffectServices.CleanEffects();

                controller.ShowEndRoundScreen(!controller.RoundInterrupted);
                yield return controller.WaitUntilEndOfRoundScreenClosed();
                controller.HideEndRoundScreen();
                yield return new WaitForSeconds(1f);


                if (ClientServices.HasNextClient())
                {
                    ClientServices.GetNextClient();
                    yield return textChat.WaitUntilClientCameIn();
                }
                else
                {
                    controller.ShowEndDayScreen();
                    yield return controller.WaitUntilEndOfDayValidated();
                    ClientServices.ClientLeaves();
                    break;
                }
            }
        }

        public override void Exit()
        {
            base.Exit();

            _gameLoop.CardGameController.Hide();
            //  _gameLoop.ClientTextChat.Hide();
            _gameLoop.ChatApp.Hide();
        }
    }
}
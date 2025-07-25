using System.Collections;
using Quackery.Clients;
using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    public class CardGameState : GameState
    {
        private CardGameController _controller;
        public CardGameState(GameLoop gameLoop) : base(gameLoop)
        {
            _controller = gameLoop.CardGameController;
        }

        public CardGameState(CardGameController controller) : base(null)
        {
            _controller = controller;
        }

        public override IEnumerator StateRoutine()
        {

            //    var textChat = _gameLoop.ClientTextChat;

            _controller.Show();

            DeckServices.Shuffle();

            //bool firstTime = true;
            while (true)
            {

                var client = ClientServices.GetNextClient();

                // else
                // {
                //     textChat.Show(CustomerPanelSize.Special);
                //     yield return new WaitForSeconds(0.5f);
                //     DialogQueueServices.QueueDialog($"{client.DialogName}Quest");
                //     yield return DialogQueueServices.WaitUntilAllDialogEnds();
                // }
                //yield return new WaitForSeconds(0.5f);
                _controller.StartNewRound(client);




                yield return _controller.WaitUntilEndOfRound();

                DeckServices.DiscardHand();
                if (_controller.RoundInterrupted)
                {
                    CartServices.ResetCartValue();
                    client.BadReview();
                    ClientServices.ClientServed(client, false);

                }
                else
                {
                    DialogQueueServices.QueueDialog("MeConclusion");
                    DialogQueueServices.QueueDialog($"{client.DialogName}Success");

                    client.GoodReview();
                    ClientServices.ClientServed(client, true);
                    yield return DialogQueueServices.WaitUntilAllDialogEnds();
                }

                _controller.ShowEndRoundScreen(!_controller.RoundInterrupted, out bool wasBoss);
                yield return _controller.WaitUntilEndOfRoundScreenClosed();
                _controller.HideEndRoundScreen();
                _controller.TransfertCartToPurse();

                CartServices.DiscardCart();
                EffectServices.CleanEffects();
                yield return new WaitForSeconds(1f);

                if (!ClientServices.HasNextClient() || wasBoss)
                {
                    _controller.ShowEndDayScreen();
                    yield return new WaitForSeconds(1f);
                    DeckServices.ResetDecks();
                    yield return _controller.WaitUntilEndOfDayValidated();
                    ClientServices.ClientLeaves();
                    break;
                }
                else
                {
                    ClientServices.GetNextClient();
                    // yield return textChat.WaitUntilClientCameIn();
                }
            }
        }

        public override void Exit()
        {
            base.Exit();

            _controller.Hide();
            if (_gameLoop != null)
                _gameLoop.ChatApp.Hide();
        }
    }
}
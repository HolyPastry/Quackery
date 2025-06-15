


using System;
using System.Collections;
using Quackery.StateMachines;

namespace Quackery
{
    public class CardGameplayLoop : StateMachine
    {
        public int CartSize = 2;
        public int CashInCart = 0;

        IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            InitLoop();
        }

        private void InitLoop()
        {

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Quackery.Clients;
using UnityEngine;

namespace Quackery
{
    public class EndRealTest : MonoBehaviour
    {
        [SerializeField] private EndOfRoundScreen _endOfRoundScreen;

        [SerializeField] private bool _revealClient = false;
        [SerializeField] private ClientData _data;

        private void Start()
        {
            StartCoroutine(EndRoundTest());
        }

        private IEnumerator EndRoundTest()
        {
            yield return new WaitForSeconds(1f);
            ClientServices.GenerateDailyQueue();
            Client client = ClientServices.GetNextClient();
            yield return null;
            if (_revealClient)
                ClientServices.SwapCurrentClientTo(_data);

            // StartCoroutine(_endOfRoundScreen.Show(client, true));

        }
    }
}

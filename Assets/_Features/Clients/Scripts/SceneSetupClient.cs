using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Clients
{
    public class SceneSetupClient : SceneSetupScript
    {
        [SerializeField] private List<ClientData> clients;
        [SerializeField] private bool GenerateDailyQueue = true;
        [SerializeField] private bool InfiniteQueue = false;
        protected override IEnumerator Routine()
        {
            yield return FlowServices.WaitUntilReady();
            yield return ClientServices.WaitUntilReady();
            ClientServices.SetInfiniteQueue(InfiniteQueue);
            foreach (var client in clients)
            {
                ClientServices.AddClient(client);
            }
            if (GenerateDailyQueue)
            {
                ClientServices.GenerateDailyQueue();
            }
            EndScript();
        }
    }
}

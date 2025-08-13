using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery.Flow;
using Quackery.Effects;
using UnityEngine;

namespace Quackery.Clients
{
    public class SceneSetupClient : SceneSetupScript
    {
        [SerializeField] private List<ClientData> clients;

        [SerializeField] private List<Effect> UnknownEffectsToAdd;
        [SerializeField] private bool GenerateDailyQueue = true;
        [SerializeField] private bool InfiniteQueue = false;
        public override IEnumerator Routine()
        {
            yield return FlowServices.WaitUntilReady();
            yield return ClientServices.WaitUntilReady();
            ClientServices.SetInfiniteQueue(InfiniteQueue);
            foreach (var client in clients)
            {
                ClientServices.AddKnownClient(client);
                if (client.FirstQuest != null)
                    QuestServices.StartQuest(client.FirstQuest);


            }
            foreach (var effect in UnknownEffectsToAdd)
            {
                ClientServices.AddUnknownClient(effect);
            }
            if (GenerateDailyQueue)
            {
                ClientServices.GenerateDailyQueue();
            }
        }
    }
}

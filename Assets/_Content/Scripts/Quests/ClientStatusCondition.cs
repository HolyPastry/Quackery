using Holypastry.Bakery.Quests;
using Quackery.Clients;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "ClientStatusCondition", menuName = "Quackery/Quests/Conditions/Client Status Condition")]
    public class ClientStatusCondition : Condition
    {
        [SerializeField] private Client.EnumState _state;
        [SerializeField] private ClientData _clientData;

        public override bool Check => ClientServices.CheckStatus(_clientData, _state);

    }
}

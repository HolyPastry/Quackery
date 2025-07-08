using Holypastry.Bakery.Quests;
using Quackery.Clients;
using UnityEngine;

namespace Quackery
{



    [CreateAssetMenu(fileName = "ClientReadyResult", menuName = "Quackery/Quests/Set Client State")]
    public class SetClientStateResult : Result
    {
        [SerializeField] private Client.EnumState _state;
        [SerializeField] private ClientData _clientData;
        public override void Execute()
        {
            ClientServices.SetClientState(_clientData, _state);
        }
    }
}

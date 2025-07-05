using Holypastry.Bakery.Quests;
using Quackery.Clients;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "RevealClientResult", menuName = "Quackery/Quests/Reveal Client")]
    public class RevealClientResult : Result
    {
        [SerializeField] private ClientData _clientData;
        public override void Execute()
        {
            ClientServices.SwapCurrentClientTo(_clientData);
        }
    }
}

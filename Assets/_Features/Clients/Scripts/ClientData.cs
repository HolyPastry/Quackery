using System.Collections.Generic;
using Bakery.Dialogs;

using UnityEngine;

namespace Quackery.Clients
{
    [CreateAssetMenu(
        fileName = "ClientData",
        menuName = "Quackery/ClientData",
        order = 1)]
    public class ClientData : ScriptableObject
    {
        public CharacterData CharacterData;
        public Sprite Icon;
        public List<Effect> Effects;

        public string Name => CharacterData.MasterText;
    }
}

using System.Collections.Generic;
using Bakery.Dialogs;
using Quackery.Effects;
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
        public List<EffectData> Effects;

        public string Name => CharacterData.MasterText;
    }
}

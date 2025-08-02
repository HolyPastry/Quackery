using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "DestroyCardTypeEffect", menuName = "Quackery/Effects/Deck/Destroy Card Type", order = 1)]
    public class DestroyCardTypeEffect : EffectData
    {
        [SerializeField] private ItemData _cardType;

        public override IEnumerator Execute(Effect effect)
        {
            yield return DeckServices.DestroyCardType(_cardType);
        }
    }

}

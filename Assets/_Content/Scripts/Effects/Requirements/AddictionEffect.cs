using Quackery.Decks;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery
{


    [CreateAssetMenu(fileName = "AddictionEffect", menuName = "Quackery/Effects/Requirements/Addiction", order = 0)]
    public class AddictionEffect : EffectData, IEffectRequirement, ICategoryEffect
    {
        [SerializeField] private EnumItemCategory _category = EnumItemCategory.Any;
        public EnumItemCategory Category => _category;

        public bool IsFulfilled(Effect effect, Card card)
        {
            return card.Category == _category ||
                DeckServices.GetMatchingCards(c => c.Category == _category, EnumCardPile.Hand).Count == 0;
        }

    }
}

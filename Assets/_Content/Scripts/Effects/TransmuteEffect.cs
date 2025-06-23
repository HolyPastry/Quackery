using Quackery.Decks;
using Quackery.Inventories;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "TransmuteEffect", menuName = "Quackery/Effects/TransmuteEffect", order = 1)]
    public class TransmuteEffect : EffectData
    {
        [SerializeField] private EnumItemCategory _newCategory;

        public override string GetDescription()
        {
            return Sprites.Replace(_newCategory, Description);
        }

        public override void Execute(Effect effect, CardPile pile)
        {
            DeckServices.ChangeRandomgTableCardCategory(_newCategory);
        }
    }
}

using UnityEngine;
using Quackery.Inventories;
using System;


namespace Quackery.Decks
{
    public class CardFactory
    {
        private readonly Card _itemCardPrefab;
        private readonly Card _skillCardPrefab;
        private readonly Card _curseCardPrefab;
        private readonly Transform _parent;

        public CardFactory(Card itemCardPrefab, Card skillCardPrefab, Card curseCardPrefab)
        {
            _itemCardPrefab = itemCardPrefab;
            _skillCardPrefab = skillCardPrefab;
            _curseCardPrefab = curseCardPrefab;
        }

        public Card Create(Item item)
        {
            Card card;

            if (item.Category == EnumItemCategory.Skill)
                card = GameObject.Instantiate(_skillCardPrefab, _parent);
            else if (item.Category == EnumItemCategory.Curse || EnumItemCategory.TempCurse == item.Category)
                card = GameObject.Instantiate(_curseCardPrefab, _parent);
            else
                card = GameObject.Instantiate(_itemCardPrefab, _parent);

            card.Item = item;
            return card;
        }

        internal Card Create(ItemData data)
        {
            var Item = new Item(data);
            return Create(Item);
        }
    }
}

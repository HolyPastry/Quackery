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

        public CardFactory(Card itemCardPrefab, Card skillCardPrefab, Card curseCardPrefab)
        {
            _itemCardPrefab = itemCardPrefab;
            _skillCardPrefab = skillCardPrefab;
            _curseCardPrefab = curseCardPrefab;
        }

        public Card Create(Item item)
        {
            Card card;

            if (item.Category == EnumItemCategory.Skills)
                card = GameObject.Instantiate(_skillCardPrefab);
            else if (item.Category == EnumItemCategory.Fatigues)
                card = GameObject.Instantiate(_curseCardPrefab);
            else
                card = GameObject.Instantiate(_itemCardPrefab);

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

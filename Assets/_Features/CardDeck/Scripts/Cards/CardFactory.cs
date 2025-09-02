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
        private readonly Card _tempCurseCardPrefab;
        private readonly Transform _parent;

        public CardFactory(Card itemCardPrefab, Card skillCardPrefab, Card curseCardPrefab, Card tempCurseCardPrefab)
        {
            _itemCardPrefab = itemCardPrefab;
            _skillCardPrefab = skillCardPrefab;
            _curseCardPrefab = curseCardPrefab;
            _tempCurseCardPrefab = tempCurseCardPrefab;
        }

        public Card Create(Item item)
        {
            Card card;

            if (item.Category == EnumItemCategory.Skills)
                card = GameObject.Instantiate(_skillCardPrefab, _parent);
            else if (item.Category == EnumItemCategory.Curse)
                card = GameObject.Instantiate(_curseCardPrefab, _parent);
            else if (item.Category == EnumItemCategory.TempCurse)
                card = GameObject.Instantiate(_tempCurseCardPrefab, _parent);
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

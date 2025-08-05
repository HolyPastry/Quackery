using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Quackery.Inventories;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;



namespace Quackery.Decks
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private Card _itemCardPrefab;
        [SerializeField] private Card _skillCardPrefab;
        [SerializeField] private Card _curseCardPrefab;
        [SerializeField] private Card _tempCurseCardPrefab;
        private CardFactory _cardFactory;
        void Awake()
        {
            _cardFactory = new CardFactory(_itemCardPrefab,
                                            _skillCardPrefab,
                                            _curseCardPrefab,
                                            _tempCurseCardPrefab);
        }

        void OnDisable()
        {
            DeckServices.DestroyCard = (cards) => { };
            DeckServices.DuplicateCard = (card) => null;

            DeckServices.CreateCard = (itemData) => null;

        }
        void OnEnable()
        {
            DeckServices.DestroyCard = DestroyCard;
            DeckServices.DuplicateCard = DuplicateCard;

            DeckServices.CreateCard = CreateCard;

        }
        private Card CreateCard(ItemData data) => _cardFactory.Create(data);
        private void DestroyCard(Card card)
        {
            if (card == null) return;
            DeckServices.RemoveFromAllPiles(card);
            card.transform.DOKill();
            card.Destroy();
        }

        private Card DuplicateCard(Card card)
        {
            if (card == null) return null;

            var duplicate = Instantiate(card);

            duplicate.name = card.name;
            duplicate.Item = card.Item;
            duplicate.OverrideCategory(card.Category);

            return duplicate;
        }
    }
}

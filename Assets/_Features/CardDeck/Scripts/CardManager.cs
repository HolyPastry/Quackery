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

        [SerializeField] private Material _destroyMaterial;

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
            DeckServices.DestroyCard = (cards) => null;
            DeckServices.DuplicateCard = (card) => null;

            DeckServices.CreateCard = (itemData) => null;

        }
        void OnEnable()
        {
            DeckServices.DestroyCard = (card) => StartCoroutine(DestroyCard(card));
            DeckServices.DuplicateCard = DuplicateCard;

            DeckServices.CreateCard = CreateCard;

        }
        private Card CreateCard(ItemData data) => _cardFactory.Create(data);
        private IEnumerator DestroyCard(Card card)
        {
            if (card == null) yield break;
            DeckServices.RemoveFromAllPiles(card);
            card.transform.DOKill();
            yield return card.PlayDestroyEffect(Tempo.WholeBeat, Instantiate(_destroyMaterial));
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

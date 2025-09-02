
using System;
using System.Collections.Generic;

using Quackery.Inventories;


namespace Quackery.Decks
{
    // public class CardList
    // {
    //     private List<Card> _cards = new();

    //     public bool IsEmpty => _cards.Count == 0;

    //     public Card TopCard => PeekTopCard();

    //     public EnumItemCategory Category => IsEmpty ? EnumItemCategory.Any : TopCard.Category;

    //     public int Count => _cards.Count;
    //     public bool Playable = true;
    //     public bool Enabled = true;

    //     public void Shuffle() => _cards.Shuffle();

    //     public Card PeekTopCard() => IsEmpty ? null : _cards[0];

    //     public Card PeekBottomCard() => IsEmpty ? null : _cards[^1];


    //     public bool DrawTopCard(out Card card)
    //     {
    //         card = null;
    //         if (IsEmpty) return false;
    //         card = _cards[0];
    //         _cards.RemoveAt(0);
    //         return true;
    //     }

    //     public void Clear() => _cards.Clear();

    //     public bool RemoveCard(Card card) => _cards.Remove(card);

    //     public void AddOnTop(Card card, bool isInstant = false)
    //     {
    //         if (card == null) return;
    //         _cards.Insert(0, card);
    //         DeckEvents.OnCardMovedTo(card, Type, Index, true, isInstant);
    //     }
    //     public void AddAtTheBottom(Card card, bool isInstant = false)
    //     {
    //         if (card == null) return;
    //         _cards.Add(card);
    //         DeckEvents.OnCardMovedTo(card, Type, Index, false, isInstant);
    //     }

    //     public void MergeOnTop(CardList pile)
    //     {
    //         if (pile == null || pile.IsEmpty) return;

    //         foreach (var card in pile._cards)
    //         {
    //             if (card != null)
    //             {
    //                 AddOnTop(card);
    //                 //DeckEvents.OnCardMovedTo(card, Type, Index, true);
    //             }
    //         }
    //         pile.Clear();
    //     }

    //     internal void MergeBelow(CardList pile)
    //     {
    //         if (pile == null || pile.IsEmpty) return;

    //         foreach (var card in pile._cards.ToArray())
    //         {
    //             if (card != null)
    //             {
    //                 AddAtTheBottom(card);
    //                 // DeckEvents.OnCardMovedTo(card, Type, Index, false);
    //             }
    //         }
    //         pile.Clear();
    //     }

    //     internal List<CardReward> CalculateCartRewards(List<CardList> otherPiles)
    //     {
    //         if (IsEmpty || !Enabled) return new();
    //         var allCards = new List<Card>(_cards);
    //         allCards.Remove(TopCard);
    //         return TopCard.CalculateCardReward(allCards, otherPiles);

    //     }

    //     internal void RestoreCategory()
    //     {
    //         foreach (var card in _cards)
    //         {
    //             if (card != null)
    //             {
    //                 card.RemoveCategoryOverride();
    //             }
    //         }
    //     }

    //     internal void OverrideStackCategory(EnumItemCategory category)
    //     {
    //         for (int i = 0; i < _cards.Count; i++)
    //         {
    //             _cards[i].OverrideCategory(category);
    //         }

    //     }


    //     public bool Contains(Card card)
    //     {
    //         return _cards.Contains(card);
    //     }

    //     internal void UpdateUI()
    //     {
    //         if (IsEmpty || !Enabled) return;
    //         TopCard.UpdateUI();

    //     }

    //     internal void Add(Card card, EnumPlacement placement)
    //     {
    //         switch (placement)
    //         {
    //             case EnumPlacement.OnTop:
    //                 AddOnTop(card);
    //                 break;
    //             case EnumPlacement.AtTheBottom:
    //                 AddAtTheBottom(card);
    //                 break;
    //             case EnumPlacement.ShuffledIn:
    //                 AddShuffledIn(card);
    //                 break;
    //             default:
    //                 throw new ArgumentOutOfRangeException(nameof(placement), placement, null);
    //         }
    //     }

    //     private void AddShuffledIn(Card card)
    //     {
    //         int index = UnityEngine.Random.Range(0, _cards.Count);
    //         _cards.Insert(index, card);
    //         DeckEvents.OnCardMovedTo(card, Type, Index, true, false);
    //     }
    // }
}

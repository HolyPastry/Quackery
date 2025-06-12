using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holypastry.Bakery.Flow;
using Quackery.Inventories;
using DG.Tweening;
using System.Linq;
using System;

namespace Quackery.Decks
{
    public class DeckManager : Service
    {
        [SerializeField] private Card _cardPrefab;

        private readonly List<CardPile> _piles = new()
        {
            new CardPile { Type = EnumPileType.DrawPile },
            new CardPile { Type = EnumPileType.DiscardPile },
            new CardPile { Type = EnumPileType.OnTable1 },
            new CardPile { Type = EnumPileType.OnTable2 },
            new CardPile { Type = EnumPileType.OnTable3 },
            new CardPile { Type = EnumPileType.OnTable4 },
            new CardPile { Type = EnumPileType.InCart1 },
            new CardPile { Type = EnumPileType.InCart2 },
            new CardPile { Type = EnumPileType.InCart3 }
        };
        private List<EnumPileType> _tablePileTypes = new()
        {
           EnumPileType.OnTable1 ,
           EnumPileType.OnTable2 ,
           EnumPileType.OnTable3 ,
            EnumPileType.OnTable4
        };


        private List<EnumPileType> _cartPileTypes = new()
        {
            EnumPileType.InCart1,
            EnumPileType.InCart2,
            EnumPileType.InCart3
        };

        private CardPile _drawPile => _piles.Find(p => p.Type == EnumPileType.DrawPile);
        private CardPile _discardPile => _piles.Find(p => p.Type == EnumPileType.DiscardPile);
        private List<CardPile> _tablePiles => _piles.FindAll(p => _tablePileTypes.Contains(p.Type));
        private List<CardPile> _cartPiles => _piles.FindAll(p => _cartPileTypes.Contains(p.Type));

        public bool CartIsFull => _cartPiles.TrueForAll(p => !p.IsEmpty);

        void OnDisable()
        {
            DeckServices.WaitUntilReady = () => new WaitUntil(() => true);
            DeckServices.DrawOne = () => null;
            DeckServices.DrawMany = (numberCards) => new();
            DeckServices.Shuffle = () => { };
            DeckServices.Discard = (card) => { };
            DeckServices.AddToCart = (card) => { };
            DeckServices.AddToDeck = (itemData) => { };
            DeckServices.DiscardHand = () => { };
            DeckServices.DestroyPile = (pileType) => { };
            DeckServices.ResetDeck = () => { };

            DeckServices.GetTablePile = () => new();
            DeckServices.MovetoCardInCart = (card, targetCard) => { };
            DeckServices.PileClicked = (pileType) => { };

            // DeckServices.GetTotalCashInCart = () => 0;
            DeckServices.ShuffleDiscardIn = () => { };
            DeckServices.EvaluatePileValue = (pileType) => new();
            DeckServices.IsCartFull = () => false;
            DeckServices.GetPile = (pileType) => null;
            DeckServices.MovePileTo = (sourcePile, targetPile) => { };
        }

        void OnEnable()
        {
            DeckServices.WaitUntilReady = () => new WaitUntil(() => _isReady);
            DeckServices.DrawOne = DrawOne;
            DeckServices.DrawMany = DrawMany;
            DeckServices.Shuffle = ShuffleDiscardPileIn;
            DeckServices.Discard = Discard;
            DeckServices.AddToCart = AddToCart;
            DeckServices.AddToDeck = AddToDeck;
            DeckServices.DiscardHand = DiscardHand;
            DeckServices.DestroyPile = DestroyPile;
            DeckServices.ResetDeck = ResetDeck;
            DeckServices.PileClicked = PileClicked;

            //DeckServices.GetTablePile = () => new List<Card>(_onTable);
            DeckServices.MovetoCardInCart = MoveToCardInCart;
            // DeckServices.GetTotalCashInCart = GetTotalCashInCart;
            DeckServices.ShuffleDiscardIn = ShuffleDiscardPileIn;
            DeckServices.EvaluatePileValue = EvaluatePileValue;
            DeckServices.IsCartFull = () => CartIsFull;
            DeckServices.GetPile = (pileType) => _piles.Find(p => p.Type == pileType);
            DeckServices.GetTablePile = () => _tablePiles;
            DeckServices.MovePileTo = MovePileTo;

        }

        private void MovePileTo(CardPile pile1, CardPile pile2)
        {
            if (pile1 == null || pile2 == null || pile1.IsEmpty) return;

            pile2.MergeIn(pile1);
            DeckEvents.OnPileMoved(pile2.Type);
        }

        private List<CardReward> EvaluatePileValue(EnumPileType type)
        {
            var pile = _piles.Find(p => p.Type == type);
            if (pile == null || pile.IsEmpty) return new();
            int index = _cartPiles.IndexOf(pile);
            List<CardPile> otherCartPiles = _cartPiles.Where((p, i) => i != index).ToList();

            return pile.CalculateCartRewards(otherCartPiles);
        }

        // private int GetTotalCashInCart()
        // {
        //     int value = _cartPiles[0].CalculateCartRewards(null, _cartPiles[1]);
        //     value += _cartPiles[1].CalculateCartRewards(_cartPiles[0], _cartPiles[2]);
        //     value += _cartPiles[2].CalculateCartRewards(_cartPiles[1], null);

        //     return value;
        // }

        private void PileClicked(EnumPileType type)
        {
            if (!_tablePileTypes.Contains(type)) return;
            MergePileToCart(type);

        }



        private void MergePileToCart(EnumPileType type)
        {
            var pile = _piles.Find(p => p.Type == type);
            if (pile == null || pile.IsEmpty) return;

            foreach (var cartPile in _cartPiles)
            {
                if (cartPile.IsEmpty)
                {
                    var topCard = pile.TopCard;
                    cartPile.MergeIn(pile);

                    topCard.ExecutePower(EnumPowerTrigger.OnCardMoveToCart, cartPile);
                    DeckEvents.OnPileMoved(cartPile.Type);
                    return; // Exit after merging to the first empty cart pile
                }
            }

        }

        private void MoveToCardInCart(Card card1, Card targetCard)
        {
            if (card1 == null || targetCard == null) return;

            // _inCart.Add(card1);

            // DeckEvents.OnCardMovedTo(card1, PileType.InCart);
        }

        private void ResetDeck()
        {
            DiscardHand();
            ShuffleDiscardIntoDrawPile();
        }

        private void DestroyPile(EnumPileType type)
        {
            var pile = _piles.Find(p => p.Type == type);
            pile?.Clear();
            DeckEvents.OnPileDestroyed(type);
        }

        protected override IEnumerator Start()
        {
            yield return InventoryServices.WaitUntilReady();
            var allItems = InventoryServices.GetAllItems();
            AddToDeck(allItems);
            _isReady = true;
        }

        private void DiscardHand()
        {

            var cardsToDiscard = new List<Card>();
            foreach (var pile in _tablePiles)
            {
                cardsToDiscard.AddRange(pile.Cards);
            }
            foreach (var card in cardsToDiscard)
            {
                Discard(card);
            }
        }

        private void AddToDeck(List<Item> items)
        {
            foreach (var item in items)
            {
                var card = Instantiate(_cardPrefab, transform);
                card.Item = item;
                _drawPile.AddAtTheBottom(card);
            }
        }

        private void AddToDeck(List<ItemData> list)
        {
            foreach (var itemData in list)
            {
                var item = new Item(itemData)
                {
                    Price = itemData.StartPrice,
                    Rating = itemData.StartRating
                };
                var card = Instantiate(_cardPrefab, transform);
                card.Item = item;
                _drawPile.AddAtTheBottom(card);
                DeckEvents.OnCardMovedTo(card, EnumPileType.DrawPile);
            }

        }

        private void AddToCart(List<Card> card)
        {
            foreach (var c in card)
            {
                if (c == null) continue;
                AddToCart(c);
            }
        }
        private void AddToCart(Card card)
        {
            RemoveFromAllPiles(card);
            foreach (var pile in _cartPiles)
            {
                if (pile.IsEmpty)
                {
                    pile.AddAtTheTop(card);
                    DeckEvents.OnCardMovedTo(card, pile.Type);
                    return; // Exit after adding to the first empty cart pile
                }
            }
            Discard(card); // If no empty cart pile found, discard the card
        }

        private void Discard(Card card)
        {
            RemoveFromAllPiles(card);
            _discardPile.AddAtTheTop(card);
            DeckEvents.OnCardMovedTo(card, EnumPileType.DiscardPile);
        }

        private void RemoveFromAllPiles(Card card)
        {
            _drawPile.RemoveCard(card);
            _discardPile.RemoveCard(card);
            foreach (var pile in _tablePiles)
                pile.RemoveCard(card);

            foreach (var pile in _cartPiles)
                pile.RemoveCard(card);
        }

        private void ShuffleDiscardPileIn()
        {
            _drawPile.MergeIn(_discardPile);
            _drawPile.Shuffle();
            DeckEvents.OnShuffle(EnumPileType.DrawPile, _drawPile.Cards);
        }

        private Card DrawOne()
        {
            if (!_drawPile.DrawTopCard(out Card card))
            {
                if (_discardPile.IsEmpty)
                {
                    Debug.LogWarning("No cards left to draw.");
                    return null;
                }
                ShuffleDiscardIntoDrawPile();
                return DrawOne();
            }
            AddCardToTable(card);
            return card;
        }

        private void AddCardToTable(Card card)
        {
            foreach (var pile in _tablePiles)
            {
                if (pile.IsEmpty)
                {
                    pile.AddAtTheTop(card);
                    DeckEvents.OnCardMovedTo(card, pile.Type);
                    return; // Exit after adding to the first empty table pile
                }
            }
            // If no empty table pile found, discard the card
            Discard(card);
        }

        private void ShuffleDiscardIntoDrawPile()
        {
            if (_discardPile.IsEmpty) return;
            var cardsToMove = new List<Card>(_discardPile.Cards);
            foreach (var card in cardsToMove)
            {
                if (card == null) continue;
                _drawPile.AddAtTheBottom(card);
                _discardPile.RemoveCard(card);
                DeckEvents.OnCardMovedTo(card, EnumPileType.DrawPile);
            }
            _drawPile.Shuffle();
        }

        private List<Card> DrawMany(int number)
        {
            var cards = new List<Card>();
            for (int i = 0; i < number; i++)
            {
                Card card = DrawOne();
                if (card == null)
                    break;
                cards.Add(card);
            }
            return cards;
        }
    }
}

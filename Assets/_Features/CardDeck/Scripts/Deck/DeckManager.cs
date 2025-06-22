using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holypastry.Bakery.Flow;
using Quackery.Inventories;
using System.Linq;
using Quackery.Effects;
using Quackery.Clients;

namespace Quackery.Decks
{
    public class DeckManager : Service
    {
        [SerializeField] private Card _itemCardPrefab;
        [SerializeField] private Card _skillCardPrefab;

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
            new CardPile { Type = EnumPileType.InCart3 },
            new CardPile { Type = EnumPileType.InCart4 },
            new CardPile { Type = EnumPileType.InCart5 },
            new CardPile { Type = EnumPileType.SelectPile1 },
            new CardPile { Type = EnumPileType.SelectPile2 },
            new CardPile { Type = EnumPileType.SelectPile3 },
            new CardPile { Type = EnumPileType.SelectPile4 }
        };
        private List<EnumPileType> _tablePileTypes = new()
        {
           EnumPileType.OnTable1 ,
           EnumPileType.OnTable2 ,
           EnumPileType.OnTable3 ,
            EnumPileType.OnTable4
        };

        private List<EnumPileType> _selectPileTypes = new()
        {
            EnumPileType.SelectPile1,
            EnumPileType.SelectPile2,
            EnumPileType.SelectPile3,
            EnumPileType.SelectPile4
        };

        private List<EnumPileType> _cartPileTypes = new()
        {
            EnumPileType.InCart1,
            EnumPileType.InCart2,
            EnumPileType.InCart3,
            EnumPileType.InCart4,
            EnumPileType.InCart5
        };
        private CardPile _lastCartPile;
        private bool _interruptDraw;

        private CardPile _drawPile => _piles.Find(p => p.Type == EnumPileType.DrawPile);
        private CardPile _discardPile => _piles.Find(p => p.Type == EnumPileType.DiscardPile);
        private List<CardPile> _tablePiles => _piles.FindAll(p => _tablePileTypes.Contains(p.Type));
        private List<CardPile> _cartPiles => _piles.FindAll(p => _cartPileTypes.Contains(p.Type));
        private List<CardPile> _selectPiles => _piles.FindAll(p => _selectPileTypes.Contains(p.Type));

        public bool CartIsFull => _cartPiles.TrueForAll(p => !p.IsEmpty || !p.Enabled);

        public int CartSize => _cartPiles.Count(p => p.Enabled);



        void OnDisable()
        {
            DeckServices.WaitUntilReady = () => new WaitUntil(() => true);


            DeckServices.AddToDeck = (itemData) => { };

            DeckServices.Shuffle = () => { };
            DeckServices.ShuffleDiscardIn = () => { };

            DeckServices.DrawBackToFull = () => { };
            DeckServices.Draw = (numberCards) => new List<Card>();
            DeckServices.InterruptDraw = () => { };

            DeckServices.DiscardHand = () => { };
            DeckServices.Discard = (card) => { };
            DeckServices.DiscardCart = () => { };
            DeckServices.Destroy = (cards) => { };


            DeckServices.GetPileRewards = (pileType) => new();
            DeckServices.GetTablePile = () => new();
            DeckServices.PileClicked = (pileType) => { };
            DeckServices.MovePileTo = (sourcePile, targetPile) => { };
            DeckServices.GetTopCard = (pileType) => null;
            DeckServices.EvaluatePileValue = (pileType) => 0;


            DeckServices.MoveToCardSelect = (cards) => { };
            DeckServices.MoveToTable = (card) => { };


            DeckServices.IsCartFull = () => false;
            DeckServices.SetCartSize = (newSize) => { };
            DeckServices.ExpandCart = (amount) => { };

            DeckServices.MergeCart = (amount) => { };

            DeckServices.RecountCart = () => { };


            EffectEvents.OnAdded -= CheckEffects;
            EffectEvents.OnRemoved -= CheckEffects;
            EffectEvents.OnUpdated -= CheckEffects;


        }

        void OnEnable()
        {
            DeckServices.WaitUntilReady = () => new WaitUntil(() => _isReady);

            DeckServices.AddToDeck = AddNewCardToDeck;

            DeckServices.Shuffle = ShuffleDiscardPileIn;
            DeckServices.ShuffleDiscardIn = ShuffleDiscardPileIn;

            DeckServices.DrawBackToFull = DrawBackToFull;
            DeckServices.Draw = DrawMany;
            DeckServices.InterruptDraw = () => _interruptDraw = true;

            DeckServices.Discard = Discard;
            DeckServices.DiscardHand = DiscardHand;
            DeckServices.DiscardCart = DiscardCart;
            DeckServices.Destroy = DestroyCard;



            DeckServices.PileClicked = PileClicked;
            DeckServices.GetPileRewards = GetPileRewards;
            DeckServices.GetTablePile = () => _tablePiles;
            DeckServices.MovePileTo = MovePileTo;
            DeckServices.GetTopCard = (pileType) => _piles.Find(p => p.Type == pileType)?.TopCard;
            DeckServices.EvaluatePileValue = (pileType) => GetPileRewards(pileType).Sum(r => r.Value);


            DeckServices.MoveToCardSelect = MoveToCardSelectionPile;
            DeckServices.MoveToTable = MoveToTable;


            DeckServices.SetCartSize = (newSize) => UpdateCartSize(newSize);
            DeckServices.ExpandCart = (amount) => UpdateCartSize(CartSize + amount);
            DeckServices.IsCartFull = () => CartIsFull;

            DeckServices.MergeCart = MergeCart;
            DeckServices.RecountCart = RecountCart;


            EffectEvents.OnAdded += CheckEffects;
            EffectEvents.OnRemoved += CheckEffects;
            EffectEvents.OnUpdated += CheckEffects;
        }

        private void RecountCart()
        {
            foreach (var pile in _cartPiles)
            {
                if (pile.IsEmpty || !pile.Enabled) continue;
                DeckEvents.OnCachingTheCart(pile.Type);
            }
        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            yield return InventoryServices.WaitUntilReady();
            var allItems = InventoryServices.GetAllItems();
            PopulateDeck(allItems);
            _isReady = true;
        }

        private void UpdateCartSize(int cartSize)
        {
            for (int i = 0; i < _cartPiles.Count; i++)
            {
                _cartPiles[i].Enabled = i < cartSize;
            }
            DeckEvents.OnCartSizeUpdated(cartSize);
        }

        private void CheckEffects(Effect _)
        {
            _cartPiles.ForEach(pile =>
            {
                if (pile.IsEmpty || !pile.Enabled) return;
                pile.TopCard.CheckEffects();
            });
            _tablePiles.ForEach(pile =>
            {
                if (pile.IsEmpty || !pile.Enabled) return;
                pile.TopCard.CheckEffects();
            });
            _selectPiles.ForEach(pile =>
            {
                if (pile.IsEmpty || !pile.Enabled) return;
                pile.TopCard.CheckEffects();
            });
        }

        private void DestroyCard(Card card)
        {
            RemoveFromAllPiles(card);
            card.Destroy();
        }

        private void MoveToTable(Card card)
        {
            AddCardToTable(card);
        }

        private void MoveToCardSelectionPile(List<Card> list)
        {
            DeckEvents.OnCardsMovingToSelectPile();
            foreach (var card in list)
            {
                foreach (var pile in _selectPiles)
                {
                    if (pile.IsEmpty)
                    {
                        pile.AddAtTheTop(card);
                        DeckEvents.OnCardMovedTo(card, pile.Type, true);
                        break;
                    }
                }
            }
            SetPileActivation(_selectPiles, true);
        }



        private void DiscardCart()
        {
            EffectServices.RemoveEffectsLinkedToPiles(_cartPiles);
            List<Card> cardsToDiscard = new();
            foreach (var cartPile in _cartPiles)
                foreach (var card in cartPile.Cards)
                    cardsToDiscard.Add(card);

            cardsToDiscard.ForEach(card => Discard(card));

            _lastCartPile = null;
        }

        private void MovePileTo(CardPile pile1, CardPile pile2)
        {
            if (pile1 == null || pile2 == null || pile1.IsEmpty) return;

            pile2.MergeBelow(pile1);
            DeckEvents.OnPileMoved(pile2.Type);
        }

        private List<CardReward> GetPileRewards(EnumPileType type)
        {
            var pile = _piles.Find(p => p.Type == type);
            if (pile == null || pile.IsEmpty) return new();
            int index = _cartPiles.IndexOf(pile);
            List<CardPile> otherCartPiles = _cartPiles.Where((p, i) => i != index).ToList();

            return pile.CalculateCartRewards(otherCartPiles);
        }
        private void MergeCart(int amount)
        {
            int index = _lastCartPile != null ? _cartPiles.IndexOf(_lastCartPile) : -1;
            if (index == -1 || index == 0) return; // No last cart pile to merge into

            MovePileTo(_lastCartPile, _cartPiles[index - 1]);
            DeckEvents.OnCachingTheCart(_lastCartPile.Type);
        }

        private void PileClicked(EnumPileType type)
        {
            DeactivateAllPiles();
            if (_tablePileTypes.Contains(type))
                ClickOnTablePile(type);


            if (_selectPileTypes.Contains(type))
                ClickOnCardSelection(type);
        }



        private void ClickOnCardSelection(EnumPileType type)
        {
            var pile = _piles.Find(p => p.Type == type);
            if (pile == null || pile.IsEmpty) return;

            var otherCards = new List<Card>();
            foreach (var selectPile in _selectPileTypes)
            {
                if (selectPile == type) continue; // Skip the clicked pile
                var otherPile = _piles.Find(p => p.Type == selectPile);
                if (otherPile != null && !otherPile.IsEmpty)
                    otherCards.Add(otherPile.TopCard);
            }
            DeckEvents.OnCardSelected(pile.TopCard, otherCards);
            SetPileActivation(_tablePiles, true);

        }

        private void ClickOnTablePile(EnumPileType type)
        {

            var pile = _piles.Find(p => p.Type == type);
            if (pile == null || pile.IsEmpty) return;

            var meDialog = pile.TopCard.Item.Data.name + "Me";
            string clientResponse = ClientServices.SelectedClient().Data.CharacterData.name + "Answer";
            DialogQueueServices.QueueDialog(meDialog);
            DialogQueueServices.QueueDialog(clientResponse);

            if (pile.TopCard.Category == EnumItemCategory.Skills)
            {
                pile.TopCard.ExecutePowerInCart(pile);
                Discard(pile.TopCard);
                DeckServices.DrawBackToFull();
                return;
            }

            MergePileToCart(type);
        }

        private void MergePileToCart(EnumPileType type)
        {
            var pile = _piles.Find(p => p.Type == type);
            if (pile == null || pile.IsEmpty) return;

            if (ActivatePreviousCard(pile)) return;

            foreach (var cartPile in _cartPiles)
            {
                if (cartPile.IsEmpty && cartPile.Enabled)
                {
                    var topCard = pile.TopCard;
                    cartPile.MergeBelow(pile);
                    _lastCartPile = cartPile;
                    topCard.ExecutePowerInCart(cartPile);
                    DeckEvents.OnPileMoved(cartPile.Type);
                    DeckEvents.OnCachingTheCart(cartPile.Type);
                    return; // Exit after merging to the first empty cart pile
                }
            }

        }

        private bool ActivatePreviousCard(CardPile pile)
        {
            if (_lastCartPile != null && !_lastCartPile.IsEmpty &&
                _lastCartPile.TopCard.HasActivatableEffects &&
                _lastCartPile.Category == pile.TopCard.Category)
            {
                // If the last cart pile is not empty and has the same category, merge into it
                _lastCartPile.MergeBelow(pile);
                DeckEvents.OnPileMoved(_lastCartPile.Type);
                DeckEvents.OnCachingTheCart(_lastCartPile.Type);
                _lastCartPile.TopCard.ActivatePower(_lastCartPile);
                return true; // Exit after merging to the last cart pile
            }

            return false;
        }

        private void DestroyPile(EnumPileType type)
        {
            var pile = _piles.Find(p => p.Type == type);
            pile?.Clear();
            DeckEvents.OnPileDestroyed(type);
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

        private void PopulateDeck(List<Item> items)
        {
            foreach (var item in items)
                AddToDeck(item);
        }
        private void AddToDeck(Item item)
        {
            Card card;
            if (item.Data.Category == EnumItemCategory.Skills)
                card = Instantiate(_skillCardPrefab, transform);
            else
                card = Instantiate(_itemCardPrefab, transform);
            card.Item = item;
            _drawPile.AddAtTheBottom(card);
            DeckEvents.OnCardMovedTo(card, EnumPileType.DrawPile, false);
        }

        private void AddNewCardToDeck(List<ItemData> list)
        {
            foreach (var itemData in list)
            {
                var item = InventoryServices.AddNewItem(itemData);
                AddToDeck(item);
            }
        }

        private void Discard(List<Card> cards)
        {
            foreach (var card in cards)
            {
                Discard(card);
            }
        }

        private void Discard(Card card)
        {
            RemoveFromAllPiles(card);
            _discardPile.AddAtTheTop(card);
            card.Discard();
            DeckEvents.OnCardMovedTo(card, EnumPileType.DiscardPile, true);
        }

        private void RemoveFromAllPiles(Card card)
        {
            _drawPile.RemoveCard(card);
            _discardPile.RemoveCard(card);
            foreach (var pile in _tablePiles)
                pile.RemoveCard(card);

            foreach (var pile in _cartPiles)
                pile.RemoveCard(card);

            foreach (var pile in _selectPiles)
                pile.RemoveCard(card);

        }

        private void ShuffleDiscardPileIn()
        {
            _drawPile.MergeBelow(_discardPile);
            _drawPile.Shuffle();
            DeckEvents.OnShuffle(EnumPileType.DrawPile, _drawPile.Cards);
        }

        private void AddCardToTable(Card card)
        {
            RemoveFromAllPiles(card);
            foreach (var pile in _tablePiles)
            {
                if (pile.IsEmpty)
                {
                    pile.AddAtTheTop(card);
                    DeckEvents.OnCardMovedTo(card, pile.Type, true);
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
                DeckEvents.OnCardMovedTo(card, EnumPileType.DrawPile, false);
            }
            _drawPile.Shuffle();
        }

        private void DrawBackToFull()
        {
            EffectServices.Execute(EnumEffectTrigger.OnDraw, _drawPile);
            if (_interruptDraw)
            {
                _interruptDraw = false;
                return;
            }
            int cardsNeeded = _tablePiles.FindAll(p => p.IsEmpty).Count;
            if (cardsNeeded <= 0) return;

            List<Card> drawnCards = DrawMany(cardsNeeded);
            foreach (var card in drawnCards)
            {
                AddCardToTable(card);
            }
            CheckEffects(null);
            SetPileActivation(_tablePiles, true);
        }



        private void SetPileActivation(List<CardPile> piles, bool activated)
        {
            foreach (var pile in piles)
            {
                if (pile.IsEmpty) continue;
                pile.Activate(activated);
                DeckEvents.OnActivatePile(pile.Type, activated);
            }
        }

        private void DeactivateAllPiles()
        {
            SetPileActivation(_tablePiles, false);
            SetPileActivation(_selectPiles, false);
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

            return card;
        }
    }
}

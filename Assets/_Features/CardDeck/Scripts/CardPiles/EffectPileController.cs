
using System;
using System.Collections;
using System.Collections.Generic;

using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    public class EffectPileController : MonoBehaviour
    {
        private readonly List<CardPile> _piles = new();
        private List<CardPileUI> _pileUIs = new();
        //   [SerializeField] private CardPileUI _prefab;

        void Awake()
        {
            GetComponentsInChildren(true, _pileUIs);
        }

        internal void Teleport(Card card)
        {
            CardPile pile = GetPile();
            pile.AddOnTop(card, isInstant: true);
        }

        internal IEnumerator Move(Card card)
        {
            CardPile pile = GetPile();
            pile.AddOnTop(card, isInstant: false);
            yield return Tempo.WaitForABeat;
        }

        private CardPile GetPile()
        {
            var newIndex = _piles.Count;
            var pile = new CardPile(EnumCardPile.Effect, newIndex);
            _piles.Add(pile);

            var pileUI = _pileUIs[newIndex];
            pileUI.gameObject.SetActive(true);
            pileUI.transform.localScale = Vector3.one;
            pileUI.PileIndex = newIndex;
            pileUI.Type = EnumCardPile.Effect;



            return pile;
        }

        internal void RemoveCard(Card card)
        {
            foreach (var pile in _piles)
                if (pile.RemoveCard(card))
                    DeckEvents.OnPileUpdated(EnumCardPile.Effect, pile.Index);
            RemoveEmptyPiles();
        }

        private void RemoveEmptyPiles()
        {
            var toRemove = _piles.FindAll(p => p.IsEmpty);
            foreach (var pile in toRemove)
            {
                _piles.Remove(pile);
                var ui = _pileUIs.Find(p => p.PileIndex == pile.Index);
                ui.gameObject.SetActive(true);
            }
        }
    }
}

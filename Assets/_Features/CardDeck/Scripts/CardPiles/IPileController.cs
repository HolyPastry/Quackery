

using System.Collections;

using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    [SerializeField]
    public interface IPileController
    {
        public EnumCardPile CardPileType { get; }
        public void Teleport(Card card);
        public IEnumerator Move(Card card);
        public bool RemoveCard(Card card);

    }
}

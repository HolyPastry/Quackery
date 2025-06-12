using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Inventories
{
    public abstract class ValueEvaluator : ScriptableObject
    {
        public abstract List<CardReward> Evaluate(Item item, List<Item> subItems, List<CardPile> otherPiles);

    }

}
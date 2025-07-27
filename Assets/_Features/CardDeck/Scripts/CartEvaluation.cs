using System;
using UnityEngine;

namespace Quackery.Decks
{
    [Serializable]
    public struct CartEvaluation
    {
        public CartMode Mode;
        public GameObject RealizationObjectReference;

        public float Duration;

    }
}

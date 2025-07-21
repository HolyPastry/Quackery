using System;
using UnityEngine;

namespace Quackery.Decks
{
    [Serializable]
    public struct CartEvaluation
    {
        public int Index;
        public float Value;
        public Color Color;
        public string Description;

        public AudioClip SoundBite;
    }
}

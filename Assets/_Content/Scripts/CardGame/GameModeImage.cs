
using System;
using UnityEngine;

namespace Quackery
{
    [Serializable]
    public record CartModeImage()
    {
        public CartMode GameMode;
        public Color Color;
        public Sprite Sprite;
    }
}

using System;
using Quackery.Effects;
using UnityEngine;


namespace Quackery
{
    public static class EffectEvents
    {
        public static Action<Effect> OnUpdated = delegate { };
        public static Action<Effect> OnAdded = delegate { };
        public static Action<Effect> OnRemoved = delegate { };
        public static Action<Effect> OnEffectActivated = delegate { };
    }
}

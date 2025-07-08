using Quackery.Decks;
using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(fileName = "CoverProtectionEffect", menuName = "Quackery/Effects/Stack/CoverProtection", order = 0)]
    public class CoverProtection : EffectData
    {
        public override void Execute(Effect effect)
        {
            //TODO: // Implement cover protection logic
            Debug.Log("Cover Protection Effect Executed");
        }
    }
}

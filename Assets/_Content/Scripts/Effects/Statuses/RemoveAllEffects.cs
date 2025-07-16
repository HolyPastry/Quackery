

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Quackery.Effects
{

    [CreateAssetMenu(
        fileName = "RemoveAllEffects",
        menuName = "Quackery/Effects/Status/RemoveAllEffects",
        order = 1)]
    public class RemoveAllEffects : EffectData
    {
        [Tooltip("List of effects that should not be removed.")]
        [SerializeField] private List<EffectData> _whiteList;
        public override IEnumerator Execute(Effect effect)
        {
            EffectServices.CancelAllEffects(_whiteList);
            yield return new WaitForSeconds(0.5f);
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "Cure Status",
        menuName = "Quackery/Effects/Status/Cure Status")]
    public class CureStatus : EffectData
    {
        [SerializeField] private List<Status> _statusesToCure;
        public override IEnumerator Execute(Effect effect)
        {
            yield return EffectServices.Remove(e => e.Data is IStatusEffect statusEffect &&
                     _statusesToCure.Contains(statusEffect.Status));
        }
    }
}

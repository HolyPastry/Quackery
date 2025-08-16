using System.Collections;
using System.Collections.Generic;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery
{
    [CreateAssetMenu(fileName = "zTest Effect Data", menuName = "Test/Status Effect Data 2")]
    public class TestStatusEffectData2 : EffectData, IStatusEffect
    {
        [SerializeField] private Status _status;

        [SerializeField] private EnumEffectTrigger _trigger;
        public Status Status => _status;

        public EnumEffectTrigger Trigger => _trigger;

        public override IEnumerator Execute(Effect effect)
        {
            EffectServicesUnitTest.SubmitAcknowledgment();
            yield return new WaitForSeconds(0.1f);
        }
    }
}

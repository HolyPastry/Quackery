using System.Collections;
using System.Collections.Generic;
using Quackery.Effects;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.Assertions;

namespace Quackery
{
    [CreateAssetMenu(fileName = "zTest Effect Data", menuName = "Test/Status Effect Data")]
    public class TestStatusEffectData : EffectData, IStatusEffect, ICategoryEffect
    {
        [SerializeField] private Status _status;
        [SerializeField] private EnumItemCategory _category;
        [SerializeField] private EnumEffectTrigger _trigger;
        [SerializeField] private float _value;
        public Status Status => _status;
        public EnumItemCategory Category => _category;
        public EnumEffectTrigger Trigger => _trigger;
        public float Value => _value;

        public override IEnumerator Execute(Effect effect)
        {
            EffectServicesUnitTest.SubmitAcknowledgment();
            yield return new WaitForSeconds(0.1f);
        }
    }
}

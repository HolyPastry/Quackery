using System.Collections;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "zTest Effect Data", menuName = "Test/ Effect Data")]

    public class TestEffectData : EffectData
    {
        public override IEnumerator Execute(Effect effect)
        {
            EffectServicesUnitTest.SubmitAcknowledgment();
            yield return new WaitForSeconds(0.1f);
        }
    }
}

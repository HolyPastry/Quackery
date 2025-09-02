using System.Collections;
using Quackery.Clients;
using UnityEngine;

namespace Quackery.Effects
{
    [CreateAssetMenu(fileName = "Cast Effect", menuName = "Quackery/Effects/CastEffect")]
    public class CastEffect : EffectData
    {
        [SerializeField] private EffectData _effectToCast;
        [SerializeField] private EnumTarget _target;

        public override IEnumerator Execute(Effect effect)
        {
            if (_target == EnumTarget.Client)
            {
                Client client = ClientServices.GetClient();
                yield return EffectServices.AddToCarrier(_effectToCast, client);
            }
            if (_target == EnumTarget.Player)
            {
                yield return EffectServices.AddToCarrier(_effectToCast, null);
            }
            else
            {
                Debug.LogWarning("Not Implemented Yet");
                //TODO:: Define what it means to attach an effect to something else than a client;
            }
        }
    }
}

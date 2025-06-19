using System.Collections;
using DG.Tweening;
using Quackery.Decks;
using UnityEngine;

namespace Quackery.Shops
{
    public class UpgradeRealization : RewardRealization
    {
        [SerializeField] private float _duration = 2f;
        public override IEnumerator RealizationRoutine(ShopReward reward)
        {
            yield return new WaitForSeconds(_duration);
            gameObject.SetActive(false);
            OnRealizationComplete.Invoke();
            // card.transform.DOPunchRotation(new Vector3(0, 0, 60), 1f, 10, 1)
            //         .OnComplete(() => OnRealizationComplete.Invoke());
        }
    }
}

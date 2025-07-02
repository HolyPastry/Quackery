using System;
using KBCore.Refs;
using UnityEngine;

namespace Quackery.Shops
{
    public abstract class ConfirmationPanel : ValidatedMonoBehaviour
    {
        [SerializeField, Self] protected AnimatedRect _animatable;

        protected ShopReward _reward;
        public Action<bool> OnExited = delegate { };

        public virtual void Show(ShopReward reward)
        {
            _reward = reward;
            gameObject.SetActive(true);
            _animatable.ZoomIn();
        }
        public virtual void Hide()
        {
            _animatable.ZoomOut(false);
            gameObject.SetActive(false);
        }
    }
}

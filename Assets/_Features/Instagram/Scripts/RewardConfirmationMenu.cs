using System;
using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    internal class RewardConfirmationMenu : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private AnimatedRect _animatable;
        [SerializeField] private Image _banner;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Button _applyButton;
        [SerializeField] private Button _cancelButton;

        public Action<InstaReward> OnRewardApplied = delegate { };
        public Action OnCancel = delegate { };
        private InstaReward _reward;

        void OnEnable()
        {
            _applyButton.onClick.AddListener(ApplyReward);
            _cancelButton.onClick.AddListener(Cancel);
        }
        void OnDisable()
        {
            _applyButton.onClick.RemoveListener(ApplyReward);
            _cancelButton.onClick.RemoveListener(Cancel);
        }

        private void Cancel()
        {
            OnCancel?.Invoke();
            Hide();
        }

        private void ApplyReward()
        {
            OnRewardApplied?.Invoke(_reward);
            Hide();
        }

        internal void Hide()
        {
            _animatable.ZoomOut(false);

        }

        internal void Show(InstaReward reward)
        {
            _reward = reward;
            _animatable.ZoomIn();
        }
    }
}

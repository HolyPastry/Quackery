using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class InstagramUI : ValidatedMonoBehaviour
    {

        [SerializeField, Child] private Canvas _canvas;
        [SerializeField, Self] private AnimatedRect _animatable;
        [SerializeField] private Button _skipButton;

        [SerializeField] private RewardConfirmationMenu _rewardConfirmationMenu;

        private List<InstaPost> _posts = new();

        public event Action OnRewardChosen = delegate { };

        void Awake()
        {
            GetComponentsInChildren<InstaPost>(true, _posts);
            foreach (var post in _posts)
            {
                post.gameObject.SetActive(false);
            }

        }

        void OnEnable()
        {
            foreach (var post in _posts)
                post.OnPostClicked += ShowConfirmationMenu;

            _rewardConfirmationMenu.OnRewardApplied += ApplyReward;
            _rewardConfirmationMenu.OnCancel += CancelConfirmation;
            _skipButton.onClick.AddListener(Skip);
        }
        void OnDisable()
        {
            foreach (var post in _posts)
                post.OnPostClicked -= ShowConfirmationMenu;
            _rewardConfirmationMenu.OnRewardApplied -= ApplyReward;
            _rewardConfirmationMenu.OnCancel -= CancelConfirmation;
            _skipButton.onClick.RemoveListener(Skip);
        }

        private void Skip()
        {
            OnRewardChosen?.Invoke();
        }

        public void ApplyReward(InstaReward reward)
        {
            InstaServices.ApplyReward(reward);
            _rewardConfirmationMenu.Hide();
        }
        public void CancelConfirmation()
        {
            _rewardConfirmationMenu.Hide();
        }

        private void ShowConfirmationMenu(InstaReward reward)
        {
            _rewardConfirmationMenu.Show(reward);
        }

        public void Show()
        {
            List<InstaReward> rewards = InstaServices.GetRewards(3);
            for (int i = 0; i < _posts.Count; i++)
            {
                if (i < rewards.Count)
                {
                    _posts[i].gameObject.SetActive(true);
                    _posts[i].AddPost(rewards[i]);

                }
                else
                {
                    _posts[i].gameObject.SetActive(false);
                }
            }
        }

        public void Hide(bool instant)
        {
            if (instant)
            {
                _canvas.enabled = false;
                return;
            }
            _animatable.SlideOut(Direction.Left).DoComplete(() => _canvas.enabled = false);
        }

    }
}

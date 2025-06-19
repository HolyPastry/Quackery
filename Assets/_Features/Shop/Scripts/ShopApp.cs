using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Quackery.Shops
{
    public class ShopApp : App
    {
        [Serializable]
        public struct RewardShopPost
        {
            public ShopRewardType Type;
            public ShopPost PostPrefab;
            public RewardRealization RewardRealizationPanel;
            public ConfirmationPanel ConfirmationPanel;
        }

        [Header("References")]

        [SerializeField] private Button _continueButton;
        [SerializeField] private Transform _continueButtonContainer;
        [SerializeField] private Transform _postsContainer;

        [Header("Posts")]
        [SerializeField] private List<RewardShopPost> _rewardPosts = new();

        private List<ShopPost> _posts = new();
        private ShopReward _currentReward;

        public event Action PhaseEnded = delegate { };


        void OnEnable()
        {
            _continueButton.onClick.AddListener(EndShopPhase);
        }
        void OnDisable()
        {
            _continueButton.onClick.RemoveListener(EndShopPhase);
        }

        private void EndShopPhase()
        {
            PhaseEnded?.Invoke();
            Hide();
        }

        public void ShowRewardRealization(ConfirmationPanel panel, ShopReward reward)
        {
            panel.OnConfirmed -= ShowRewardRealization; // Unsubscribe to avoid multiple calls
            panel.Hide();
            RewardRealization realizationPanel = _rewardPosts.Find(x => x.Type == reward.Type).RewardRealizationPanel;
            Assert.IsNotNull(realizationPanel, "RewardRealizationPanel is not set for " + reward.Type);
            _currentReward = reward;
            realizationPanel.gameObject.SetActive(true);
            realizationPanel.OnRealizationComplete += RemovePost;
            StartCoroutine(realizationPanel.RealizationRoutine(reward));

        }

        private void RemovePost()
        {
            var post = _posts.Find(x => x.ShopReward == _currentReward);
            if (post != null)
            {
                post.Hide();
                post.OnPostClicked -= OnPostClicked;
                _posts.Remove(post);
                Destroy(post.gameObject);
            }
            _currentReward = null;
        }

        public override void Show()
        {
            _canvas.gameObject.SetActive(true);
            CleanPosts();

            _animatedRect.SlideIn(Direction.Right)
                .DoComplete(() =>
                    StartCoroutine(ShowPostsRoutine()));

        }

        private void CleanPosts()
        {
            foreach (var post in _posts)
            {
                post.Hide();
                post.OnPostClicked -= OnPostClicked;
                Destroy(post.gameObject);
            }
            _posts.Clear();
        }

        private IEnumerator ShowPostsRoutine()
        {
            _continueButtonContainer.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            List<ShopReward> rewards = ShopServices.GetRewards(3);
            foreach (var reward in rewards)
            {
                var post = InstantiatePost(reward);
                post.SlideIn();
                _posts.Add(post);

                yield return new WaitForSeconds(0.2f);
            }
            _continueButtonContainer.SetAsLastSibling();
            _continueButtonContainer.gameObject.SetActive(true);

        }

        private ShopPost InstantiatePost(ShopReward reward)
        {
            var postPrefab = _rewardPosts.Find(x => x.Type == reward.Type).PostPrefab;

            Assert.IsNotNull(postPrefab, "PostPrefab is not set for " + reward.Type);

            var post = Instantiate(postPrefab, _postsContainer);
            post.SetupPost(reward);
            post.OnPostClicked += OnPostClicked;
            return post;
        }

        private void OnPostClicked(ShopReward reward)
        {
            var confirmationPanel = _rewardPosts.Find(x => x.Type == reward.Type).ConfirmationPanel;
            Assert.IsNotNull(confirmationPanel, "ConfirmationPanel is not set for " + reward.Type);
            confirmationPanel.OnConfirmed += ShowRewardRealization;
            confirmationPanel.Show(reward);
        }
    }
}

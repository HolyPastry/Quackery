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


        [Header("References")]

        [SerializeField] private Button _continueButton;
        [SerializeField] private Transform _continueButtonContainer;
        [SerializeField] private Transform _postsContainer;
        [SerializeField] private ConfirmationPanel _confirmationPanel;
        [SerializeField] private ShopSelectCard _selectCardPanel;

        [SerializeField] private ShopPost _postPrefab;

        private readonly List<ShopPost> _posts = new();
        private ShopReward _currentReward;

        public event Action PhaseEnded = delegate { };


        void OnEnable()
        {
            _continueButton.onClick.AddListener(EndShopPhase);
            _confirmationPanel.OnExited += OnConfirmationExited;
            _selectCardPanel.OnExited += OnConfirmationExited;
        }
        void OnDisable()
        {
            _continueButton.onClick.RemoveListener(EndShopPhase);
            _confirmationPanel.OnExited -= OnConfirmationExited;
            _selectCardPanel.OnExited -= OnConfirmationExited;
        }

        private void OnConfirmationExited(bool succeed)
        {
            if (succeed) RemovePost();
        }

        private void EndShopPhase()
        {
            PhaseEnded?.Invoke();
            Hide();
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
            //yield return new WaitForSeconds(0.5f);
            List<ShopReward> rewards = ShopServices.GetRewards(3);

            foreach (var reward in rewards)
            {
                _posts.Add(InstantiatePost(reward));
                yield return new WaitForSeconds(0.1f);
            }
            _continueButtonContainer.SetAsLastSibling();
            _continueButtonContainer.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_postsContainer as RectTransform);

        }

        private ShopPost InstantiatePost(ShopReward reward)
        {
            var post = Instantiate(_postPrefab, _postsContainer);
            post.SetupPost(reward);
            post.OnPostClicked += OnPostClicked;
            return post;
        }

        private void OnPostClicked(ShopReward reward)
        {
            _currentReward = reward;
            switch (reward)
            {
                case NewCardReward newCardReward:
                    _confirmationPanel.Show(newCardReward);
                    break;

                case RemoveCardReward removeCardReward:

                    _selectCardPanel.Show(removeCardReward);
                    break;

                case QualityOfLifeReward qualityOfLifeReward:
                    _confirmationPanel.Show(qualityOfLifeReward);
                    break;

                default:
                    Debug.LogWarning($"Unknown reward type: {reward.GetType()}");
                    return;
            }

            // // var confirmationPanel = _rewardPosts.Find(x => x.Type == reward.Type).ConfirmationPanel;
            // // Assert.IsNotNull(confirmationPanel, "ConfirmationPanel is not set for " + reward.Type);
            // confirmationPanel.OnConfirmed += ShowRewardRealization;
            // confirmationPanel.Show(reward);
        }
    }
}

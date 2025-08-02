using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Shops
{
    public class ShopApp : App
    {
        [SerializeField] private int _numberOfRewards = 3;

        [Header("References")]

        [SerializeField] private RectTransform _postsContainer;
        [SerializeField] private ConfirmationPanel _confirmationPanel;
        [SerializeField] private ShopSelectCard _selectCardPanel;
        [SerializeField] private ScrollRect _shopScrollRect;

        [SerializeField] private ShopPost _firstPostPrefab;
        [SerializeField] private ShopPost _lastPostPrefab;
        [SerializeField] private ShopPost _defaultPostPrefab;
        [SerializeField] private ShopPost _artifactPost;
        [SerializeField] private ShopPost _cardPostPrefab;
        [SerializeField] private ShopPost _removeCardPostPrefab;
        [SerializeField] private ShopPost _freeRewardPostPrefab;

        private Button _continueButton;

        private readonly List<ShopPost> _posts = new();

        public event Action PhaseEnded = delegate { };
        public event Action<int, int> OnPostListUpdated = (numPost, highlightedIndex) => { };

        public static Action<ShopPost> ShowConfirmation = delegate { };

        internal static Action<ShopPost> RemovePostRequest = delegate { };

        internal static Func<int, Coroutine> SpendMoneyRequest = (amount) => null;
        private ShopPost _selectingPost;

        void OnEnable()
        {
            _confirmationPanel.OnExited += OnConfirmationExited;
            _selectCardPanel.OnExited += OnConfirmationExited;
            OnStartOpening += PopulatingPosts;


            ShowConfirmation = ShowConfirmationPanel;
            RemovePostRequest = RemovePost;
            SpendMoneyRequest = (amount) => StartCoroutine(SpendMoneyRoutine(amount));

        }



        void OnDisable()
        {
            _confirmationPanel.OnExited -= OnConfirmationExited;
            _selectCardPanel.OnExited -= OnConfirmationExited;

            OnStartOpening -= PopulatingPosts;


            ShowConfirmation = delegate { };
            RemovePostRequest = delegate { };
            SpendMoneyRequest = (amount) => null;
        }

        private void OnMoveScreen(int postIndex)
        {
            if (postIndex < 2) return;
            int index = postIndex % _posts.Count; // Ensure index wraps around if it exceeds the number of posts

            var postToMove = _posts[0];
            _posts.Remove(postToMove);
            _posts.Add(postToMove);
            int newIndex = postIndex + _posts.Count - 2;
            postToMove.AnchoredPosition = new Vector2(0, newIndex * -Screen.height);
            postToMove.transform.SetAsLastSibling();
        }


        private IEnumerator SpendMoneyRoutine(int amount)
        {
            PurseServices.Modify(-amount);
            yield return new WaitForSeconds(0.7f);

        }
        private void OnConfirmationExited(bool succeed)
        {
            if (succeed) RemovePost(_selectingPost);
        }

        private void EndShopPhase()
        {
            PhaseEnded?.Invoke();
            Close();
        }

        private void RemovePost(ShopPost postToRemove)
        {
            if (postToRemove is ChooseACardPost chooseACardPost)
            {
                chooseACardPost.HideButton();
                // var button = chooseACardPost.GetComponentInChildren<Button>();
                // button.gameObject.SetActive(false);
            }

            // //var post = _posts.Find(x => x.ShopReward == _currentReward);
            // if (postToRemove != null)
            // {
            //     postToRemove.Hide();
            //     postToRemove.OnBuyClicked -= OnBuyClicked;
            //     int postIndex = _posts.IndexOf(postToRemove);
            //     _posts.Remove(postToRemove);
            //     Destroy(postToRemove.gameObject);
            //     OnPostListUpdated(_posts.Count, postIndex);
            //     //  _shopScrollRect.LockMovement();
            //     for (int i = postIndex; i < _posts.Count; i++)
            //     {
            //         _posts[i].MoveUp();

            //     }

            //     int height = _posts.Count * Screen.height;
            //     _postsContainer.sizeDelta = new Vector2(_postsContainer.sizeDelta.x, height);

            // }
        }

        public void PopulatingPosts()
        {
            CleanPosts();
            StartCoroutine(ShowPostsRoutine());

        }

        private void CleanPosts()
        {
            foreach (var post in _posts)
            {
                post.Hide();
                post.OnBuyClicked -= OnBuyClicked;
                Destroy(post.gameObject);
            }
            _posts.Clear();
        }

        private IEnumerator ShowPostsRoutine()
        {


            List<ShopReward> rewards = ShopServices.GetRewards(_numberOfRewards);

            AddPost(_firstPostPrefab);
            for (int i = 0; i < rewards.Count; i++)
            {
                var reward = rewards[i];
                var post = InstantiatePost(reward, verticalPosition: _posts.Count * -Screen.height);
                post.name = $"Post {i}";
                _posts.Add(post);

            }

            var lastPost = AddPost(_lastPostPrefab);
            _continueButton = lastPost.GetComponentInChildren<Button>();
            _continueButton.onClick.AddListener(EndShopPhase);

            int height = _posts.Count * Screen.height;
            _postsContainer.sizeDelta = new Vector2(_postsContainer.sizeDelta.x, height);
            OnPostListUpdated?.Invoke(_posts.Count, 0);
            yield return null;

        }

        private ShopPost AddPost(ShopPost prefab)
        {
            var lastPost = Instantiate(prefab, _postsContainer);

            lastPost.AnchoredPosition = new Vector2(0, _posts.Count * -Screen.height);
            lastPost.SizeDelta = new Vector2(Screen.width, Screen.height);
            lastPost.gameObject.SetActive(true);
            lastPost.transform.SetAsFirstSibling();

            _posts.Add(lastPost);
            return lastPost;
        }

        private ShopPost InstantiatePost(ShopReward reward, int verticalPosition)
        {
            ShopPost post;

            if (reward is ArtifactReward)
                post = Instantiate(_artifactPost, _postsContainer);
            else if (reward is NewCardReward)
                post = Instantiate(_cardPostPrefab, _postsContainer);
            else if (reward is RemoveCardReward)
                post = Instantiate(_removeCardPostPrefab, _postsContainer);
            else
                post = Instantiate(_defaultPostPrefab, _postsContainer);

            post.SetupPost(reward);
            // post.OnPostClicked += OnPostClicked;
            post.OnBuyClicked += OnBuyClicked;
            post.transform.SetAsFirstSibling();
            post.AnchoredPosition = new Vector2(0, verticalPosition);
            post.SizeDelta = new Vector2(Screen.width, Screen.height);

            return post;
        }

        private void OnBuyClicked(ShopPost post)
        {
            RemovePost(post);
        }

        private void ShowConfirmationPanel(ShopPost post)
        {
            _selectingPost = post;
            switch (post.ShopReward)
            {
                case NewCardReward newCardReward:
                    _confirmationPanel.Show(newCardReward);
                    break;

                case RemoveCardReward removeCardReward:

                    _selectCardPanel.Show(removeCardReward);
                    break;

                case ArtifactReward qualityOfLifeReward:
                    _confirmationPanel.Show(qualityOfLifeReward);
                    break;

                default:
                    Debug.LogWarning($"Unknown reward type: {post.ShopReward.GetType()}");
                    return;
            }
        }


    }
}

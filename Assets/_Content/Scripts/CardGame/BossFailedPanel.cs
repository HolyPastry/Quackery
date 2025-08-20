using Quackery.Clients;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;


namespace Quackery
{
    internal class BossFailedPanel : MonoBehaviour
    {
        [SerializeField] private Image _portrait;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _ratingText;
        [SerializeField] private RatingPanel _ratingPanel;
        [SerializeField] private GameObject _successText;
        [SerializeField] private TextMeshProUGUI _rewardText;

        [SerializeField] private Button _retryButton;

        [SerializeField] private GameObject _hiddable;

        void OnEnable()
        {
            _retryButton.onClick.AddListener(OnRetryButtonClicked);
        }



        void OnDisable()
        {
            _retryButton.onClick.RemoveListener(OnRetryButtonClicked);
        }
        internal void Hide()
        {
            _hiddable.SetActive(false);
            _successText.SetActive(false);
            _rewardText.gameObject.SetActive(false);
        }

        internal void Show(Client client)
        {
            StartCoroutine(ShowRoutine(client));
        }

        private IEnumerator ShowRoutine(Client client)
        {
            _hiddable.SetActive(true);
            yield return null;
            _portrait.sprite = client.Portrait;
            _portrait.color = Color.white;
            _nameText.text = client.LoginName;

            _ratingText.text = client.LastReviewText;
            yield return Tempo.WaitForABeat;
            _successText.SetActive(true);
            yield return new WaitForSeconds(1f);
            _rewardText.gameObject.SetActive(true);


        }
        private void OnRetryButtonClicked()
        {
            //relaod current scene
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
        }
    }
}

using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;


namespace Quackery.Decks
{
    public class RewardPanel : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private float _easeDuration = 0.3f;
        [SerializeField] private Ease _easeType = Ease.OutBack;
        [SerializeField] private TextMeshProUGUI _rewardPanelUI;
        private RectTransform _rectTransform => transform as RectTransform;

        void Awake()
        {
            _rectTransform.localScale = Vector3.zero;
        }
        public void ShowReward(CardReward cardReward)
        {
            _rectTransform.localScale = Vector3.zero;

            _rewardPanelUI.text = $"{cardReward.Type}: {cardReward.Value}$";
            _rectTransform.DOScale(Vector3.one, _easeDuration).SetEase(_easeType).OnComplete(() =>
            {
                // Optionally, you can add a delay before hiding the panel
                StartCoroutine(HideAfterDelay(_duration));
            });
        }

        private IEnumerator HideAfterDelay(float v)
        {
            yield return new WaitForSeconds(v);
            Hide();
        }

        public void Hide()
        {
            // Implement the logic to hide the reward panel
            _rectTransform.localScale = Vector3.zero;
        }
    }
}

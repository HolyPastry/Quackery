using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;


namespace Quackery.Decks
{
    public class RewardPanel : MonoBehaviour
    {
        [SerializeField] private Ease _easeType = Ease.OutBack;
        [SerializeField] private TextMeshProUGUI _rewardLabelGUI;
        [SerializeField] private TextMeshProUGUI _rewardNumberGUI;

        private RectTransform _rectTransform => transform as RectTransform;

        void Awake()
        {
            _rectTransform.localScale = Vector3.zero;
            _rewardNumberGUI.transform.localScale = Vector3.zero;
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
            _rectTransform.localPosition = Vector3.zero;
            _rewardNumberGUI.transform.localScale = Vector3.zero;
        }

        internal IEnumerator PunchNumberRoutine(string numberString)
        {
            _rewardNumberGUI.text = numberString;
            _rewardNumberGUI.transform.localScale = Vector3.zero;

            _rewardNumberGUI.transform.DOScale(Vector3.one, Tempo.EighthBeat).SetEase(_easeType);
            yield return Tempo.WaitForEighthBeat;
            _rewardNumberGUI.transform.DOPunchRotation(Vector3.forward, Tempo.WholeBeat, 1, 0);
            yield return Tempo.WaitForABeat;
            _rewardNumberGUI.transform.DOScale(Vector3.zero, Tempo.EighthBeat).SetEase(_easeType);
            yield return Tempo.WaitForEighthBeat;
            _rewardNumberGUI.transform.localScale = Vector3.zero;

        }

        internal IEnumerator ShowLabel(CardReward reward)
        {
            if (!reward.ShowLabel)
                yield break;

            _rewardLabelGUI.text = reward.Type.ToString();
            _rectTransform.localScale = Vector3.zero;
            _rectTransform.SetAsLastSibling();

            _rectTransform.DOScale(Vector3.one, Tempo.EighthBeat).SetEase(_easeType);
            yield return Tempo.WaitForEighthBeat;
        }

    }
}

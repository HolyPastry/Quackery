using System.Collections;
using TMPro;
using UnityEngine;

namespace Quackery
{
    internal class FollowerBadge : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _badgeText;


        public IEnumerator CountFollowersUpRoutine(int numFollowers)
        {
            gameObject.SetActive(true);
            int currentFollowers = 0;
            _badgeText.text = $"+ {currentFollowers}";
            while (currentFollowers < numFollowers)
            {
                currentFollowers++;
                _badgeText.text = $"+{currentFollowers}";
                yield return new WaitForSeconds((float)0.1 / numFollowers);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

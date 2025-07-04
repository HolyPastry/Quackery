using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class RatingPanel : MonoBehaviour
    {
        [SerializeField] private List<Toggle> _ratingStars;

        void OnEnable()
        {
            SetEmpty();
        }



        public IEnumerator SetRatingRoutine(int rating)
        {

            for (int i = 0; i < _ratingStars.Count; i++)
            {
                if (i < rating)
                {
                    _ratingStars[i].isOn = true;
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    _ratingStars[i].isOn = false;
                }
            }
        }

        internal void SetEmpty()
        {
            foreach (var star in _ratingStars)
            {
                star.isOn = false;
            }
        }
    }
}

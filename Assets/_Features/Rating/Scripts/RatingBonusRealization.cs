using System.Collections;
using System.Collections.Generic;
using Quackery.Ratings;
using UnityEngine;

namespace Quackery
{
    public class RatingBonusRealization : MonoBehaviour
    {
        [SerializeField] private TravelingRealization _travelBonusOne;
        [SerializeField] private TravelingRealization _travelBonusTwo;
        [SerializeField] private TravelingRealization _travelZeroBonus;


        void OnEnable()
        {

            var bonusCard = RatingServices.GetCardBonus();
            if (bonusCard < 0)
                _travelZeroBonus.Travel();
            if (bonusCard >= 1)
                _travelBonusOne.Travel();
            if (bonusCard >= 2)
                _travelBonusTwo.Travel();
        }
    }
}

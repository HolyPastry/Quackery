using System.Collections;
using Bakery.Saves;
using Holypastry.Bakery.Flow;
using UnityEngine;


namespace Quackery.Ratings
{
    public class RatingManager : Service
    {
        public class RatingData : SerialData
        {
            public int Rating;

            public static implicit operator int(RatingData data) => data == null ? 0 : data.Rating;
            public static implicit operator RatingData(int rating) => new() { Rating = rating };
        }

        RatingData _rating;

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            _rating = SaveServices.Load<RatingData>("PlayerRating");
            _rating ??= new RatingData()
            {
                Rating = 0
            };

            _isReady = true;

        }

        void OnEnable()
        {
            RatingServices.WaitUntilReady = () => WaitUntilReady;
            RatingServices.Modify = ModifyRating;
            RatingServices.GetRating = GetRating;
            RatingServices.GetCardBonus = GetCardBonus;
        }

        void OnDisable()
        {
            RatingServices.WaitUntilReady = () => new WaitUntil(() => true);
            RatingServices.Modify = delegate { };
            RatingServices.GetRating = delegate { return 0; };
            RatingServices.GetCardBonus = delegate { return 0; };
        }

        private void Save() => SaveServices.Save("PlayerRating", _rating);

        public void ModifyRating(int amount)
        {
            _rating += amount;
            RatingEvents.OnRatingChanged?.Invoke();
            Save();
        }

        public int GetRating()
        {
            return _rating;
        }

        public int GetCardBonus()
        {
            if (_rating <= 0)
                return -1;

            if (_rating < 3)
                return 0;

            if (_rating < 5)
                return 1;

            if (_rating >= 5)
                return 2;

            return 0;
        }
    }
}

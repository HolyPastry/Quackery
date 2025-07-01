using System.Collections;
using System.Collections.Generic;
using Bakery.Saves;
using Holypastry.Bakery;
using Holypastry.Bakery.Flow;
using Quackery.Followers;
using Quackery.Ratings;
using UnityEngine;

namespace Quackery.QualityOfLife
{


    public class QualityOfLifeManager : Service
    {

        private DataCollection<QualityOfLifeData> _collection;
        private QualityOfLifeSerial _acquired;

        void Awake()
        {
            _collection = new("QualityOfLife");
        }

        void OnEnable()
        {
            QualityOfLifeServices.GetSuitable = GetSuitable;
            QualityOfLifeServices.Acquire = Acquire;
            QualityOfLifeServices.GetRandomSuitable = () =>
            {
                var suitable = GetSuitable(5);
                return suitable.Count == 0 ? null : suitable[Random.Range(0, suitable.Count)];
            };
        }

        void OnDisable()
        {
            QualityOfLifeServices.GetSuitable = static number => new();
            QualityOfLifeServices.Acquire = static (data) => { };
            QualityOfLifeServices.GetRandomSuitable = static () => null;
        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            _acquired = SaveServices.Load<QualityOfLifeSerial>("QualityOfLife");
            _acquired ??= new QualityOfLifeSerial();
        }

        void Save()
        {
            SaveServices.Save("QualityOfLife", _acquired);
        }

        private List<QualityOfLifeData> GetSuitable(int number)
        {
            var suitable = _collection.FindAll(data =>
                         data.FollowersRequirement <= FollowerServices.GetNumberOfFollowers() &&
                         !_acquired.Contains(data));
            suitable.Sort((a, b) => b.FollowersRequirement.CompareTo(a.FollowersRequirement));
            return suitable.GetRange(0, Mathf.Min(number, suitable.Count));
        }

        private void Acquire(QualityOfLifeData data)
        {

            _acquired.Add(data);
            Save();

            if (data.FollowerBonus > 0)
                FollowerServices.ModifyFollowers(data.FollowerBonus);

            PurseServices.Modify(-data.Price);
            if (data.Bill != null)
                BillServices.AddNewBill(data.Bill);

            if (data.RatingBonus > 0)
                RatingServices.Modify(data.RatingBonus);

            if (data.CardBonus != null)
                DeckServices.AddNewToDraw(data.CardBonus, true);
        }
    }
}

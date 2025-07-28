using System;
using System.Collections;
using System.Linq;
using Bakery.Saves;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Followers
{

    public class FollowerManager : Service
    {
        [SerializeField] private FollowerData _followerData;

        public class FollowerSerial : SerialData
        {
            public int NumFollowers;

        }
        private FollowerSerial _serial;

        void OnEnable()
        {
            FollowerServices.WaitUntilReady = () => new WaitUntil(() => _isReady);
            FollowerServices.GetNumberOfFollowers = () => _serial?.NumFollowers ?? 0;
            FollowerServices.ModifyFollowers = ModifyFollowers;
            FollowerServices.SetNumberOfFollowers = SetNumFollowers;
            FollowerServices.RewardFollowers = RewardFollowers;
        }



        void OnDisable()
        {
            FollowerServices.WaitUntilReady = () => new WaitUntil(() => true);
            FollowerServices.GetNumberOfFollowers = () => 0;
            FollowerServices.ModifyFollowers = delegate { };
            FollowerServices.SetNumberOfFollowers = delegate { };
            FollowerServices.RewardFollowers = (SuccessRating) => 0;

        }
        private int RewardFollowers(int successRating)
        {
            int newFollowers = (UnityEngine.Random.Range(1, 5) + successRating);
            ModifyFollowers(newFollowers);
            return newFollowers;
        }

        private void ModifyFollowers(int number)
        {
            _serial.NumFollowers += number;
            FollowerEvents.OnFollowersChanged?.Invoke();
            Save();
        }

        private void SetNumFollowers(int number)
        {
            _serial.NumFollowers = number;
            FollowerEvents.OnFollowersChanged?.Invoke();
            Save();
        }


        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();

            _serial = SaveServices.Load<FollowerSerial>("Followers");
            _serial ??= new();
            _isReady = true;

        }

        private void Save()
        {
            SaveServices.Save("Followers", _serial);
        }


    }
}

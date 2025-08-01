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
            FollowerServices.GetCurrentLevel = GetCurrentLevel;
            FollowerServices.GetNumFollowersToNextLevel = GetNumFollowersToNextLevel;
            FollowerServices.GetNextLevel = GetNextLevel;
        }



        void OnDisable()
        {
            FollowerServices.WaitUntilReady = () => new WaitUntil(() => true);
            FollowerServices.GetNumberOfFollowers = () => 0;
            FollowerServices.ModifyFollowers = delegate { };
            FollowerServices.SetNumberOfFollowers = delegate { };
            FollowerServices.RewardFollowers = (cartMode, concluded) => 0;
            FollowerServices.GetCurrentLevel = () => default;
            FollowerServices.GetNumFollowersToNextLevel = () => 0;
            FollowerServices.GetNextLevel = () => default;


        }

        private FollowerLevel GetCurrentLevel()
        {
            if (_serial.NumFollowers < 0)
                return default;

            return _followerData.Levels
                .Where(level => level.FollowerRequirement <= _serial.NumFollowers)
                .OrderBy(level => level.Level)
                .FirstOrDefault();
        }

        private FollowerLevel GetNextLevel()
        {
            var currentLevel = GetCurrentLevel();
            if (_followerData.Levels.Exists(level => level.Level == currentLevel.Level + 1) == false)
                return default;
            return _followerData.Levels.Find(level => level.Level == currentLevel.Level + 1);
        }
        private int GetNumFollowersToNextLevel()
        {
            var currentLevel = GetCurrentLevel();
            if (_followerData.Levels.Exists(level => level.Level == currentLevel.Level + 1) == false)
                return -1;
            var nextLevel = _followerData.Levels.Find(level => level.Level == currentLevel.Level + 1);
            return nextLevel.FollowerRequirement - _serial.NumFollowers;
        }
        private int RewardFollowers(CartMode cartMode, bool concluded)
        {
            if (!concluded) return 0;
            int successRating = cartMode switch
            {
                CartMode.Survival => 2,
                CartMode.Normal => 4,
                CartMode.SuperSaiyan => 8,
                _ => throw new NotImplementedException()
            };
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

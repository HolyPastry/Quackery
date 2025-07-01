using System.Collections;
using Bakery.Saves;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Followers
{
    public class FollowerManager : Service
    {
        public class FollowerSerial : SerialData
        {
            public int NumFollowers;

            public static implicit operator
                int(FollowerSerial data) => data == null ? 0 : data.NumFollowers;

            public static implicit operator
                FollowerSerial(int numFollowers) => new() { NumFollowers = numFollowers };
        }
        private FollowerSerial _numberOfFollowers;

        void OnEnable()
        {
            FollowerServices.WaitUntilReady = () => new WaitUntil(() => _isReady);
            FollowerServices.GetNumberOfFollowers = () => _numberOfFollowers ?? 0;
            FollowerServices.ModifyFollowers = ModifyFollowers;
            FollowerServices.SetNumberOfFollowers = SetNumFollowers;
        }

        void OnDisable()
        {
            FollowerServices.WaitUntilReady = () => new WaitUntil(() => true);
            FollowerServices.GetNumberOfFollowers = () => 0;
            FollowerServices.ModifyFollowers = delegate { };
            FollowerServices.SetNumberOfFollowers = delegate { };
        }

        private void ModifyFollowers(int number)
        {
            _numberOfFollowers += number;
            FollowerEvents.OnFollowersChanged?.Invoke();
            Save();
        }

        private void SetNumFollowers(int number)
        {
            _numberOfFollowers = number;
            FollowerEvents.OnFollowersChanged?.Invoke();
            Save();
        }


        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();

            _numberOfFollowers = SaveServices.Load<FollowerSerial>("Followers");
            _numberOfFollowers ??= 0;

            _isReady = true;

        }

        private void Save()
        {
            SaveServices.Save("Followers", _numberOfFollowers);
        }


    }
}

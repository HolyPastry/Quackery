using System;
using System.Collections.Generic;
using Holypastry.Bakery;
using UnityEngine;

namespace Quackery
{
    public class InstaManager : MonoBehaviour
    {
        private DataCollection<InstaReward> _dataCollection;
        private void Awake()
        {
            _dataCollection = new DataCollection<InstaReward>("InstaData");
        }

        void OnEnable()
        {
            InstaServices.GetRewards = GetRewards;
            InstaServices.ApplyReward = ApplyReward;
        }

        void OnDisable()
        {
            InstaServices.GetRewards = (amount) => new();
            InstaServices.ApplyReward = (data) => { };
        }

        private void ApplyReward(InstaReward data)
        {
            throw new NotImplementedException();
        }

        private List<InstaReward> GetRewards(int NumRewards)
        {
            List<InstaReward> rewards = new();
            List<int> indexes = new();

            for (int i = 0; i < _dataCollection.Count; i++)
                indexes.Add(i);

            while (rewards.Count < NumRewards && indexes.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, indexes.Count);
                int index = indexes[randomIndex];
                indexes.RemoveAt(randomIndex);
                rewards.Add(_dataCollection.Data[index]);
            }

            return rewards;

        }


    }
}

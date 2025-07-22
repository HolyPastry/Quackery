using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using Quackery.Followers;
using UnityEngine;

namespace Quackery
{
    public class FollowerTextUpdate : LiveTextUpdate
    {
        private Coroutine _routine;
        private int _lastValue = -1;

        protected override void UpdateUI()
        {
            int followerCount = FollowerServices.GetNumberOfFollowers();

            if (_lastValue == followerCount)
                return;
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }

            _routine = StartCoroutine(CountUpRoutine(followerCount - _lastValue));

            _lastValue = followerCount;


        }

        private IEnumerator CountUpRoutine(int deltaNumFollowers)
        {
            PlayAudio();
            int currentValue = _lastValue == -1 ? 0 : _lastValue;
            int targetValue = _lastValue + deltaNumFollowers;
            int interval = 0;
            Text = $"{currentValue}";
            while (currentValue < targetValue)
            {
                currentValue++;
                Text = $"{currentValue}";
                yield return new WaitForSeconds((float)1 / deltaNumFollowers);
                interval++;
                if (interval % 10 == 0)
                {
                    Punch();
                }
            }
            StopAudio();

        }
    }
}

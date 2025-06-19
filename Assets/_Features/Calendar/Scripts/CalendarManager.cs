using System.Collections;
using System.Collections.Generic;
using Bakery.Saves;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery
{
    public class DaysPassed : SerialData
    {
        public int days;
        public DaysPassed() { }
        public DaysPassed(int initialDays = 1)
        {
            days = initialDays;
        }
    }
    public class CalendarManager : Service
    {
        DaysPassed _daysPassed;

        void OnEnable()
        {
            CalendarServices.WaitUntilReady = () => WaitUntilReady;
            CalendarServices.GoToNextDay = GotoNextDay;
            CalendarServices.Today = () => _daysPassed.days;

        }

        void OnDisable()
        {
            CalendarServices.WaitUntilReady = () => new WaitUntil(() => true);
            CalendarServices.GoToNextDay = delegate { };
            CalendarServices.Today = () => 0;
        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();

            _daysPassed = SaveServices.Load<DaysPassed>("DaysPassed") ?? new DaysPassed(1);
            _isReady = true;
        }

        public void GotoNextDay()
        {
            _daysPassed.days++;
            SaveServices.Save("DaysPassed", _daysPassed);
        }
    }
}

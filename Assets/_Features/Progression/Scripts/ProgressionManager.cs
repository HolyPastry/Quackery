using System;
using System.Collections;
using System.Collections.Generic;
using Bakery.Saves;
using Holypastry.Bakery.Flow;
using JetBrains.Annotations;
using UnityEngine;

namespace Quackery.Progression
{
    public class ProgressionManager : Service
    {
        SerialLevel _level;
        private bool _hasLeveledUp;

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            _level = SaveServices.Load<SerialLevel>("Progression");
            _level ??= new SerialLevel();
        }

        void OnEnable()
        {
            ProgressionServices.WaitUntilReady = () => new WaitUntil(() => _isReady);
            ProgressionServices.GetLevel = () => _level;
            ProgressionServices.SetLevel = SetLevel;
            ProgressionServices.HasLeveledUp = () => _hasLeveledUp;
            ProgressionServices.ConsumeLeveledUp = () => _hasLeveledUp = false;
        }

        private void SetLevel(int level)
        {
            if (level > _level)
            {
                _level = level;
                Save();
                _hasLeveledUp = true;
            }
        }

        void OnDisable()
        {
            ProgressionServices.WaitUntilReady = () => new WaitUntil(() => true);
            ProgressionServices.GetLevel = () => 0;
            ProgressionServices.SetLevel = level => { };
            ProgressionServices.HasLeveledUp = () => false;
            ProgressionServices.ConsumeLeveledUp = () => { };
        }
        private void Save()
        {
            SaveServices.Save("Progression", _level);
        }

    }
}

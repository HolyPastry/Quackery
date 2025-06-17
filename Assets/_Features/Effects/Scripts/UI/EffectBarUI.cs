using System;
using System.Collections.Generic;

using UnityEngine;

namespace Quackery
{
    public class EffectBarUI : MonoBehaviour
    {

        private readonly List<EffectUI> _statusUIPool = new();

        private StackMultiplierUI _stackMultiplierUI;

        void Awake()
        {
            GetComponentsInChildren(_statusUIPool);
            for (int i = 0; i < _statusUIPool.Count; i++)
            {
                _statusUIPool[i].gameObject.SetActive(false);
            }
            _stackMultiplierUI = GetComponentInChildren<StackMultiplierUI>(true);
            _stackMultiplierUI.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            EffectEvents.OnStackMultiplerUpdate += UpdateStackMultiplierUI;
            EffectEvents.OnUpdated += UpdateStatusUI;
            EffectEvents.OnAdded += AddStatusUI;
            EffectEvents.OnRemoved += RemoveStatusUI;
        }

        void OnDisable()
        {
            EffectEvents.OnStackMultiplerUpdate -= UpdateStackMultiplierUI;
            EffectEvents.OnUpdated -= UpdateStatusUI;
            EffectEvents.OnAdded -= AddStatusUI;
            EffectEvents.OnRemoved -= RemoveStatusUI;
        }

        private void UpdateStackMultiplierUI(int multiplier)
        {
            if (multiplier <= 1)
            {
                _stackMultiplierUI.gameObject.SetActive(false);
                return;
            }
            _stackMultiplierUI.gameObject.SetActive(true);
            _stackMultiplierUI.UpdateMultipler(multiplier);
        }

        private void RemoveStatusUI(Effect effect)
        {
            for (int i = 0; i < _statusUIPool.Count; i++)
            {
                if (_statusUIPool[i].Effect == effect)
                {
                    _statusUIPool[i].Hide();
                    return;
                }
            }
            Debug.LogWarning($"StatusUI for {effect.Data.name} not found in pool.");
        }

        private void AddStatusUI(Effect effect)
        {
            foreach (var statusUI in _statusUIPool)
            {
                if (statusUI.gameObject.activeSelf) continue;

                statusUI.UpdateStatus(effect, animate: true);
                return;
            }
        }



        private void UpdateStatusUI(Effect effect)
        {
            List<Effect> effects = EffectServices.GetCurrent();

            for (int i = 0; i < effects.Count; i++)
            {
                var statusUI = _statusUIPool.Find(ui
                        => ui.Effect == effects[i] &&
                            ui.gameObject.activeSelf == true
                            );
                if (statusUI != null)
                {
                    statusUI.UpdateStatus(effects[i]);
                    continue;
                }

            }
            for (int i = 0; i < _statusUIPool.Count; i++)
            {
                if (_statusUIPool[i].gameObject.activeSelf == false ||
                   effects.Exists(s => s.Data == _statusUIPool[i].Effect.Data))
                    continue;

                _statusUIPool[i].Hide();
            }
        }


    }
}

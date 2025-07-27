using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{

    public class EffectBarUI : MonoBehaviour
    {
        [SerializeField] private bool _listenToChange;
        [SerializeField] private SpeechBubble _clientSpeechBubble;
        [SerializeField] private List<EnumEffectTag> _tagFilters;
        private readonly List<EffectUI> _statusUIPool = new();


        void Awake()
        {
            GetComponentsInChildren(true, _statusUIPool);
            for (int i = 0; i < _statusUIPool.Count; i++)
                _statusUIPool[i].gameObject.SetActive(false);
        }


        void OnEnable()
        {
            if (!_listenToChange) return;

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
            // if (multiplier <= 1)
            // {
            //     _stackMultiplierUI.gameObject.SetActive(false);
            //     return;
            // }
            // _stackMultiplierUI.gameObject.SetActive(true);
            // _stackMultiplierUI.UpdateMultipler(multiplier);
        }

        public void RemoveStatusUI(Effect effect)
        {
            for (int i = 0; i < _statusUIPool.Count; i++)
            {
                if (!_statusUIPool[i].gameObject.activeSelf) continue;
                if (_statusUIPool[i].Effect == null) continue;
                if (_statusUIPool[i].Effect.Data == effect.Data)
                {
                    _statusUIPool[i].Hide();
                    return;
                }
            }
            Debug.LogWarning($"StatusUI for {effect.Data.name} not found in pool.");
        }

        public void AddStatusUI(Effect effect)
        {
            bool containsTag = _tagFilters.TrueForAll(tag => effect.Tags.Contains(tag));
            if (!containsTag) return;
            foreach (var statusUI in _statusUIPool)
            {
                if (statusUI.gameObject.activeSelf) continue;
                StartCoroutine(AddClientStatusRoutine(statusUI, effect));
                return;
            }
        }

        private IEnumerator AddClientStatusRoutine(EffectUI statusUI, Effect effect)
        {
            if (_clientSpeechBubble == null)
            {
                statusUI.UpdateStatus(effect, animate: true);
                yield break;
            }

            _clientSpeechBubble.SetText($"<sprite name={effect.Data.name}>");
            yield return new WaitForSeconds(1f);

            statusUI.UpdateStatus(effect, origin: _clientSpeechBubble.transform.position);
            _clientSpeechBubble.Hide();
        }

        public void UpdateStatusUI(Effect effect)
        {
            List<Effect> effects = EffectServices.GetCurrent();

            for (int i = 0; i < effects.Count; i++)
            {
                var statusUI = _statusUIPool.Find(ui
                        => ui.Effect != null &&
                             ui.Effect.Data == effects[i].Data &&
                            ui.gameObject.activeSelf == true
                            );
                if (statusUI != null)
                {
                    statusUI.UpdateStatus(effects[i], animate: true);
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

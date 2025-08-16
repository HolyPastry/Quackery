using System;
using System.Collections;
using System.Collections.Generic;
using Quackery.Effects;
using UnityEngine;

namespace Quackery
{

    public class StatusBarUI : MonoBehaviour
    {
        [SerializeField] private bool _listenToChange;
        [SerializeField] private SpeechBubble _clientSpeechBubble;
        [SerializeField] private List<EnumTarget> _targetFilter;
        private readonly List<StatusUI> _statusUIPool = new();


        void Awake()
        {

            GetComponentsInChildren(true, _statusUIPool);
            for (int i = 0; i < _statusUIPool.Count; i++)
                _statusUIPool[i].gameObject.SetActive(false);
        }


        void OnEnable()
        {
            if (!_listenToChange) return;

            EffectEvents.OnUpdated += UpdateStatusUI;
            EffectEvents.OnAdded += UpdateStatusUI;
            EffectEvents.OnRemoved += UpdateStatusUI;
        }

        void OnDisable()
        {
            EffectEvents.OnUpdated -= UpdateStatusUI;
            EffectEvents.OnAdded -= UpdateStatusUI;
            EffectEvents.OnRemoved -= UpdateStatusUI;
        }

        private void RemoveStatus(Status status)
        {
            for (int i = 0; i < _statusUIPool.Count; i++)
            {
                if (!_statusUIPool[i].gameObject.activeSelf) continue;
                if (_statusUIPool[i].Status == null) continue;
                if (_statusUIPool[i].Status == status)
                {
                    _statusUIPool[i].Hide();
                    return;
                }
            }
            Debug.LogWarning($"StatusUI for {status.name} not found in pool.");
        }

        private void AddStatus(Status status, int value)
        {
            foreach (var statusUI in _statusUIPool)
            {
                if (statusUI.gameObject.activeSelf) continue;
                StartCoroutine(AddClientStatusRoutine(statusUI, status, value));
                return;
            }
        }

        private IEnumerator AddClientStatusRoutine(StatusUI statusUI, Status status, int value)
        {
            if (_clientSpeechBubble == null)
            {
                statusUI.UpdateStatus(status, value, animate: true);
                yield break;
            }

            _clientSpeechBubble.SetText($"<sprite name={status.name}>");
            yield return new WaitForSeconds(1f);

            statusUI.UpdateStatus(status, value, origin: _clientSpeechBubble.transform.position);
            _clientSpeechBubble.Hide();
        }

        public void UpdateStatusUI(Effect effect)
        {
            if (effect.Data is not IStatusEffect statusEffect) return;
            if (_targetFilter.Count > 0 &&
                 !_targetFilter.Contains(statusEffect.Status.Target)) return;

            Dictionary<Status, int> statuses = EffectServices.GetActiveStatuses();

            //if the status is no longer active, it should be removed
            if (!statuses.TryGetValue(statusEffect.Status, out int newValue))
            {
                RemoveStatus(statusEffect.Status);
                return;
            }

            foreach (var statusUI in _statusUIPool)
            {
                if (statusUI.Status != statusEffect.Status) continue;

                statusUI.UpdateStatus(statusEffect.Status, newValue, true);
                return;
            }
            //if the status is not found in the pool, it should be added
            AddStatus(statusEffect.Status, newValue);
        }
    }
}

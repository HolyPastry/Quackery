using System.Collections;
using DG.Tweening;
using KBCore.Refs;
using TMPro;
using UnityEngine;


namespace Quackery
{
    public abstract class LiveTextUpdate : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private TextMeshProUGUI _textGUI;
        [SerializeField] private bool _liveUpdate = false;
        [SerializeField] private float _updateRate = 2;
        [SerializeField] private AudioSource _audioSource;

        protected string Text
        {
            get => _textGUI.text;
            set => _textGUI.text = Sprites.Replace(value);
        }

        private bool _initialized;
        void OnEnable()
        {
            if (!_initialized) return;
            if (_liveUpdate)
                StartCoroutine(LiveUpdateRoutine());
            else UpdateUI();
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator Start()
        {
            yield return WaitUntilReady;
            if (_liveUpdate)
                StartCoroutine(LiveUpdateRoutine());
            else UpdateUI();
            _initialized = true;
        }

        protected abstract void UpdateUI();

        protected virtual WaitUntil WaitUntilReady => null;

        protected void Punch()
        {
            _textGUI.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        }

        protected void PlayAudio()
        {
            if (_audioSource != null)
                _audioSource.Play();

        }
        protected void StopAudio()
        {
            if (_audioSource != null)
                _audioSource.Stop();
        }


        private IEnumerator LiveUpdateRoutine()
        {
            while (true)
            {
                UpdateUI();
                yield return new WaitForSeconds(_updateRate);
            }
        }
    }
}

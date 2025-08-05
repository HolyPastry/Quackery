using System;
using System.Collections;
using System.Collections.Generic;
using Holypastry.Bakery;
using KBCore.Refs;
using Quackery.Decks;
using Quackery.GameMenu;
using UnityEngine;

namespace Quackery
{
    [RequireComponent(typeof(AudioSource))]
    public class CardMoveSound : ValidatedMonoBehaviour
    {
        [SerializeField] private List<AudioClip> _sounds;
        [SerializeField, Self] private AudioSource _audioSource;
        [SerializeField] private float _minTimeBetweenSounds = 0.2f;
        [SerializeField] private float _timeSound = 0.5f;

        private CountdownTimer _timerBetweenSounds;
        private CountdownTimer _timerSound;

        void Awake()
        {
            _timerBetweenSounds = new CountdownTimer(_minTimeBetweenSounds);
            _timerSound = new CountdownTimer(_timeSound);
        }

        void OnEnable()
        {
            DeckEvents.OnCardMovedTo += PlaySound;
            GameMenuController.OnCardMovement += PlaySound;
        }



        void OnDisable()
        {
            DeckEvents.OnCardMovedTo -= PlaySound;
            GameMenuController.OnCardMovement -= PlaySound;
        }

        void Update()
        {
            _timerBetweenSounds.Tick(Time.deltaTime);
            _timerSound.Tick(Time.deltaTime);
            if (!_timerSound.IsRunning) return;

            if (_timerBetweenSounds.IsRunning) return;
            int randomIndex = UnityEngine.Random.Range(0, _sounds.Count);
            AudioClip clip = _sounds[randomIndex];
            _audioSource.PlayOneShot(clip);
            _timerBetweenSounds.Start();
        }


        private void PlaySound(Card card, EnumCardPile pile, int arg3, bool arg4, bool isInstant)
        {
            if (isInstant) return;
            PlaySound();
        }

        private void PlaySound()
        {
            int randomIndex = UnityEngine.Random.Range(0, _sounds.Count);
            AudioClip clip = _sounds[randomIndex];
            _audioSource.PlayOneShot(clip);
            // _timerSound.Stop();
            // _timerSound.Start();
        }
    }
}

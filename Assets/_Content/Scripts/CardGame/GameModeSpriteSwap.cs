
using System;
using System.Collections.Generic;
using KBCore.Refs;
using Quackery.Decks;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class GameModeSpriteSwap : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Image _image;
        [SerializeField] private List<CartModeImage> _gameModeSprites = new();

        private void OnEnable()
        {
            CartEvents.OnModeChanged += OnGameModeChanged;
        }
        private void OnDisable()
        {
            CartEvents.OnModeChanged -= OnGameModeChanged;
        }

        private void OnGameModeChanged(CartMode mode)
        {
            if (!_gameModeSprites.Exists(x => x.GameMode == mode))
            {
                Debug.LogWarning($"GameModeSpriteSwap: No sprite found for mode {mode}. Using default sprite.");
                return;
            }
            var gameModeSprite = _gameModeSprites.Find(x => x.GameMode == mode);

            _image.color = gameModeSprite.Color;
            if (gameModeSprite.Sprite != null)
                _image.sprite = gameModeSprite.Sprite;
        }
    }
}



using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;


namespace Quackery
{
    public class GameModeActivation : MonoBehaviour
    {
        [SerializeField] private List<CartMode> _activatedGameModes = new();

        void Awake()
        {
            CartEvents.OnModeChanged += OnGameModeChanged;
            gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            CartEvents.OnModeChanged -= OnGameModeChanged;
        }

        private void OnGameModeChanged(CartMode mode)
            => gameObject.SetActive(_activatedGameModes.Contains(mode));

    }
}

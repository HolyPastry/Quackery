using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using Quackery.Decks;
using Quackery.GameMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.GameMenu.Test
{
    public class MoveCardInButton : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private Button _button;
        [SerializeField] private List<Card> _cards;

        // Start is called before the first frame update
        void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            GameMenuController.AddToDeckRequest?.Invoke(_cards);
        }


    }
}

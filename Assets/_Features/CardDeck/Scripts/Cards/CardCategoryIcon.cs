using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using Quackery.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Decks
{
    public class CardCategoryIcon : ValidatedMonoBehaviour
    {
        [SerializeField, Parent] private Card _card;
        [SerializeField, Self] private Image _image;
        [SerializeField] private SpriteLibrary _spriteLibrary;

        void Awake()
        {
            _card.OnCategoryChanged += SetCategoryIcon;
        }

        void OnDestroy()
        {
            _card.OnCategoryChanged -= SetCategoryIcon;
        }

        private void SetCategoryIcon(EnumItemCategory category)
        {
            if (!Card.IsItemCard(_card.Item))
            {
                gameObject.SetActive(false);
                return;
            }


            gameObject.SetActive(true);

            _image.sprite = _spriteLibrary.GetCategoryIcon(category);

        }


    }
}

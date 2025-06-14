using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Quackery.Effects
{
    public class EffectIcon : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        public Effect Effect
        {
            get => _effect;
            set
            {
                _effect = value;
                if (_icon != null && _effect != null)
                {
                    _icon.sprite = _effect.Icon;
                }
            }
        }
        private Effect _effect;

        public bool Activated
        {
            set
            {
                if (_icon != null)
                {
                    _icon.color = value ? Color.red : Color.black;
                }
            }
        }
    }
}

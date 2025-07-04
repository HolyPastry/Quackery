using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Quackery
{

    public class TestSprite : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _sourceText;
        [SerializeField] private TextMeshProUGUI _destinationText;



        void Start()
        {
            _destinationText.text = Sprites.Replace(_sourceText.text);
        }
    }
}

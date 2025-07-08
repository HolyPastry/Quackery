using System.Collections;
using System.Collections.Generic;
using Quackery.Decks;
using UnityEngine;

namespace Quackery
{
    public class HandController : CardPool
    {
        [SerializeField] private float _cardHandRadius = 100f;
        [SerializeField] private float _sideMargin = 200f;
        [SerializeField] private GameObject Aobj;
        [SerializeField] private GameObject Bobj;
        [SerializeField] private GameObject Cobj;
        [SerializeField] private GameObject Dobj;

        private const float NightyDegree = 90f;

        private CardPileUI _selectedPileUI;

        protected override void BringToFront(CardPileUI pileUI)
        {

            foreach (var cardPile in _cardPileUIs)
            {
                cardPile.transform.SetAsLastSibling();
            }
            if (_selectedPileUI != pileUI)
            {
                pileUI.transform.SetAsLastSibling();
                _selectedPileUI = pileUI;
            }

        }

        protected override void DestroyCardPile(int index)
        {
            base.DestroyCardPile(index);
            if (_selectedPileUI.PileIndex == index)
                _selectedPileUI = null;

        }

        void Update()
        {
            Vector2 B = (Vector2)transform.position;
            Vector2 A = B - 0.5f * (Screen.width - _sideMargin) * Vector2.right;
            Vector2 C = B + 0.5f * (Screen.width - _sideMargin) * Vector2.right;
            Vector2 D = B + _cardHandRadius * Vector2.down;


            Aobj.transform.position = A;
            Bobj.transform.position = B;
            Cobj.transform.position = C;
            Dobj.transform.position = D;

            var angle = Vector2.Angle(D - B, D - C);

            for (int i = 0; i < _cardPileUIs.Count; i++)
            {
                var cardPile = _cardPileUIs[i];
                float t = (float)i / (_cardPileUIs.Count - 1);
                float angleOffset = Mathf.Lerp(-angle, angle, t);
                Vector2 position = D +
                        _cardHandRadius *
                        new Vector2(
                            Mathf.Cos((angleOffset + NightyDegree) * Mathf.Deg2Rad),
                             Mathf.Sin((angleOffset + NightyDegree) * Mathf.Deg2Rad));

                cardPile.transform.SetPositionAndRotation(position,
                             Quaternion.Euler(0, 0, angleOffset));
            }

        }
    }
}

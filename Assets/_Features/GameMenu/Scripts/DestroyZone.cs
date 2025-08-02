using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using Quackery.Decks;
using Quackery.TetrisBill;
using UnityEngine;
using UnityEngine.UI;

namespace Quackery
{
    public class DestroyZone : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private Ease _ease = Ease.InOutQuad;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private Material _destroyMaterial;
        public IEnumerator DestroyCard(Card card)
        {
            gameObject.SetActive(true);
            card.transform.SetParent(_parent);
            card.transform.DOMove(Vector3.zero, _duration).SetEase(_ease);
            card.transform.DOScale(Vector3.one, _duration).SetEase(_ease);
            card.transform.DORotate(Vector3.zero, _duration);
            yield return new WaitForSeconds(_duration);
            yield return StartCoroutine(card.PlayDestroyEffect(0.5f, Instantiate(_destroyMaterial)));

            Destroy(card.gameObject);
            gameObject.SetActive(false);
        }

        public IEnumerator DestroyArtifact(Image artifactImage)
        {
            gameObject.SetActive(true);
            artifactImage.transform.SetParent(_parent);
            artifactImage.transform.DOMove(Vector3.zero, _duration).SetEase(_ease);
            artifactImage.transform.DOScale(Vector3.one, _duration).SetEase(_ease);
            artifactImage.transform.DORotate(Vector3.zero, _duration);

            yield return new WaitForSeconds(_duration);
            artifactImage.material = Instantiate(_destroyMaterial);
            artifactImage.material.DOFloat(1, "_disolveAmount", 1);

            yield return new WaitForSeconds(1f);
            Destroy(artifactImage.gameObject);
            gameObject.SetActive(false);
        }
        public IEnumerator DestroyBill(TetrisBlock bill)
        {
            gameObject.SetActive(true);
            bill.transform.SetParent(_parent);
            bill.transform.DOMove(Vector3.zero, _duration).SetEase(_ease);
            bill.transform.DOScale(Vector3.one, _duration).SetEase(_ease);
            bill.transform.DORotate(Vector3.zero, _duration);

            yield return new WaitForSeconds(_duration);
            bill.PlayDestroyEffect(_duration, Instantiate(_destroyMaterial));
            yield return new WaitForSeconds(_duration);
            Destroy(bill.gameObject);
            gameObject.SetActive(false);
        }
    }
}

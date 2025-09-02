using System;
using System.Collections;
using DG.Tweening;
using Holypastry.Bakery;
using Quackery.GameMenu;
using UnityEngine;




namespace Quackery.Artifacts
{
    public class ArtifactBarUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _rootPrefab;
        // public static Action<ArtifactUI> TransferRequest = delegate { };

        void OnEnable()
        {
            // ArtifactEvents.OnArtifactAdded += OnArtifactAdded;
            // TransferRequest = Transfer;
            ArtifactEvents.OnArtifactOut += OnArtifactOut;
            ArtifactEvents.OnArtifactPack += OnArtifactPack;

        }


        void OnDisable()
        {
            //ArtifactEvents.OnArtifactAdded -= OnArtifactAdded;
            // TransferRequest = delegate { };
            ArtifactEvents.OnArtifactOut -= OnArtifactOut;
            ArtifactEvents.OnArtifactPack -= OnArtifactPack;
        }

        private void OnArtifactPack()
        {
            //destroy all transform children
            foreach (Transform child in transform)

                Destroy(child.gameObject);

        }

        private void OnArtifactOut(ArtifactData data)
        {

            var parent = Instantiate(_rootPrefab, transform);
            var ui = GameMenuController.SpawnArtifactUIRequest.Invoke(data);
            parent.transform.SetParent(transform);
            parent.localScale = Vector3.one;
            ui.transform.SetParent(parent.transform, false);
            ui.rectTransform.DOAnchorPos(Vector2.zero, 0.5f);
            ui.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }



        private void OnArtifactUpdated(ArtifactData data)
        {

        }


    }
}

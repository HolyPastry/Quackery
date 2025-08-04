using System;
using System.Collections;
using DG.Tweening;
using Holypastry.Bakery;
using UnityEngine;




namespace Quackery.Artifacts
{
    public class ArtifactBarUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _rootPrefab;
        public static Action<ArtifactUI> TransferRequest = delegate { };


        void OnEnable()
        {
            // ArtifactEvents.OnArtifactAdded += OnArtifactAdded;
            TransferRequest = Transfer;
        }


        void OnDisable()
        {
            //ArtifactEvents.OnArtifactAdded -= OnArtifactAdded;
            TransferRequest = delegate { };
        }


        private void Transfer(ArtifactUI ui)
        {
            var parent = Instantiate(_rootPrefab, transform);
            parent.transform.SetParent(transform);
            parent.localScale = Vector3.one;
            ui.transform.SetParent(parent.transform, false);
            ui.RectTransform.DOAnchorPos(Vector2.zero, 0.5f);
            ui.RectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {

                    foreach (var effect in ui.Artifact.Effects)
                    {
                        effect.Initialize();
                        effect.Tags.AddUnique(Effects.EnumEffectTag.Artifact);
                        effect.LinkedArtifact = ui.Artifact;
                        EffectServices.AddEffect(effect);
                    }
                    ;
                }
                );

            // ui.RectTransform.sizeDelta = new Vector2(100, 100);
        }

        private void OnArtifactUpdated(ArtifactData data)
        {

        }


    }
}

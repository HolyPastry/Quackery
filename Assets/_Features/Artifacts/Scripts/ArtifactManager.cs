using System;
using System.Collections;
using Bakery.Saves;
using Holypastry.Bakery;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Artifacts
{
    public class ArtifactManager : Service
    {

        [SerializeField] private string Key = "Artifacts";
        private DataCollection<ArtifactData> _collection;

        private SerialArtifacts _ownedArtifacts;
        void Awake()
        {
            _collection = new(Key);
        }

        void OnEnable()
        {
            ArtifactServices.WaitUntilReady = () => WaitUntilReady;
            ArtifactServices.Add = AddArtifact;
            ArtifactServices.GetAllArtifacts = () => _ownedArtifacts.All;
        }



        void OnDisable()
        {
            ArtifactServices.WaitUntilReady = () => new WaitUntil(() => true);
            ArtifactServices.Add = delegate { };
            ArtifactServices.GetAllArtifacts = () => new();
        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            _ownedArtifacts = SaveServices.Load<SerialArtifacts>(Key);
            _ownedArtifacts ??= new();
            _isReady = true;
        }

        private void Save()
        {
            SaveServices.Save(Key, _ownedArtifacts);
        }


        private void AddArtifact(ArtifactData data)
        {
            if (data.IsUpgrade)
            {
                RemoveArtifact(data.UpgradeFor);
            }
            _ownedArtifacts.Add(data);
            foreach (var effect in data.Effects)
            {
                effect.Initialize();
                effect.Tags.AddUnique(Effects.EnumEffectTag.Artifact);
                effect.LinkedArtifact = data;
                EffectServices.AddEffect(effect);
            }
            Save();
            ArtifactEvents.OnArtifactAdded(data);
        }

        private void RemoveArtifact(ArtifactData data)
        {
            if (_ownedArtifacts.Remove(data))
            {
                EffectServices.RemoveArtifactEffects(data);
                Save();
            }
            else
            {
                Debug.LogWarning($"Artifact {data.name} not found in collection.");
            }
        }
    }
}

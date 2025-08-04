using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            ArtifactServices.GetRandomSuitable = GetRandomMatching;
            ArtifactServices.Owns = (artifactData) => _ownedArtifacts.Contains(artifactData);
        }


        void OnDisable()
        {
            ArtifactServices.WaitUntilReady = () => new WaitUntil(() => true);
            ArtifactServices.Add = delegate { };
            ArtifactServices.GetAllArtifacts = () => new();
            ArtifactServices.GetRandomSuitable = (level, amount) => new();
            ArtifactServices.Owns = (artifactData) => false;
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

        private List<ArtifactData> GetRandomMatching(int level, int amount)
        {

            return _collection.Data
                .Where(artifact => artifact.Level == level &&
                    (!artifact.IsUpgrade || _ownedArtifacts.Contains(artifact.UpgradeFor)) &&
                     (artifact.Requirements.Count == 0 || artifact.Requirements.TrueForAll(req => _ownedArtifacts.Contains(req))))
                .Shuffle()
                .Take(amount)
                .ToList();
        }

    }
}

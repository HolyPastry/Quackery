
using System;
using System.Collections.Generic;
using Bakery.Saves;
using Holypastry.Bakery;

namespace Quackery.Artifacts
{
    public class SerialArtifacts : SerialData
    {


        [NonSerialized]
        private List<ArtifactData> _artifacts = new();
        public List<string> Keys = new();

        public List<ArtifactData> All => new(_artifacts);

        public void Init(DataCollection<ArtifactData> collection)
        {
            foreach (string key in Keys)
            {
                var artifactData = collection.GetFromName(key);
                _artifacts.Add(artifactData);
            }

        }

        public override void Serialize()
        {
            base.Serialize();
            Keys = _artifacts.ConvertAll(a => a.name);
        }

        public void Upgrade(ArtifactData data)
        {
            if (!data.IsUpgrade) return;
            _artifacts.Remove(data);
            _artifacts.AddUnique(data.UpgradeFor);
        }
        public void Add(ArtifactData data)
        {
            _artifacts.AddUnique(data);
        }

        internal bool Remove(ArtifactData data)
        {
            return _artifacts.Remove(data);
        }

        public bool Contains(ArtifactData data)
        {
            return _artifacts.Contains(data);
        }
    }
}

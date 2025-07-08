using System.Collections;

using Bakery.Saves;
using Holypastry.Bakery;
using Holypastry.Bakery.Flow;
using UnityEngine;

namespace Quackery.Narrative
{

    public class NarrativeManager : Service
    {
        [SerializeField] private string ServiceKey = "Narratives";
        private DataCollection<NarrativeData> _collection;

        private SerialNarrativeData _narratives;

        void Awake()
        {
            _collection = new DataCollection<NarrativeData>(ServiceKey);
        }

        void OnEnable()
        {
            NarrativeServices.AddNarrative = AddNarrative;
            NarrativeServices.ArchiveNarrative = ArchiveNarrative;
            NarrativeServices.GetInProgressNarratives = () => _narratives.GetInProgress();
        }

        void OnDisable()
        {
            NarrativeServices.AddNarrative = delegate { };
            NarrativeServices.ArchiveNarrative = delegate { };
            NarrativeServices.GetInProgressNarratives = () => new();
        }

        protected override IEnumerator Start()
        {
            yield return FlowServices.WaitUntilReady();
            _narratives = SaveServices.Load<SerialNarrativeData>(ServiceKey);
            _narratives ??= new SerialNarrativeData();
            _narratives.Init(_collection);
            _isReady = true;
        }
        private void ArchiveNarrative(NarrativeData data)
        {
            _narratives.Archive(data);
            _narratives.Save(ServiceKey);
        }

        private void AddNarrative(NarrativeData data)
        {
            _narratives.Add(data);
            _narratives.Save(ServiceKey);
        }
    }
}

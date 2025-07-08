

using System;
using System.Collections.Generic;

using Bakery.Saves;
using Holypastry.Bakery;

namespace Quackery.Narrative
{
    public class SerialNarrativeData : SerialData
    {

        [NonSerialized]
        public List<NarrativeData> InProgress = new();
        public List<string> InProgressIds = new();



        public override void Serialize()
        {
            base.Serialize();
            InProgressIds.Clear();
            InProgressIds.AddRange(InProgress.ConvertAll(narrative => narrative.name));
        }

        internal void Init(DataCollection<NarrativeData> collection)
        {
            InProgress.Clear();
            foreach (var id in InProgressIds)
            {
                var narrative = collection.GetFromName(id);
                if (narrative != null)
                    InProgress.Add(narrative);
            }
        }

        internal void Archive(NarrativeData data)
        {
            if (!InProgress.Contains(data))
                return;

            InProgress.Remove(data);

        }
        internal void Add(NarrativeData data)
        {
            if (InProgress.Contains(data))
                return;

            InProgress.Add(data);

        }

        internal void Save(string key)
        {
            SaveServices.Save(key, this);
        }

        internal List<NarrativeData> GetInProgress()
        {
            return new List<NarrativeData>(InProgress);
        }
    }
}

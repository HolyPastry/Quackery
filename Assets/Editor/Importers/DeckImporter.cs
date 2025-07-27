
#if UNITY_EDITOR
using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using Bakery.GoogleSheet;
using Holypastry.Bakery;

using Quackery.Decks;

using Quackery.Inventories;
using UnityEditor;
using UnityEngine;

namespace Quackery
{
    [CreateAssetMenu(fileName = "DeckImporter", menuName = "Quackery/GoogleSheet/DeckImporter")]
    public class DeckImporter : ScriptableObject
    {
        [SerializeField] protected GoogleSheetAPI _googleSheetAPI;
        [SerializeField] protected string SHEET_NAME;
        [SerializeField] private string _decksPath = "Assets/_Content/ScriptableObjects/Decks";
        private DataCollection<ItemData> _cardCollection;

        public async Task ImportDecks()
        {
            List<List<string>> data = await _googleSheetAPI.Import(SHEET_NAME);

            List<DeckData> decks = ParseDeck(data[0]);

            for (int i = 1; i < data.Count; i++)
            {
                Populate(decks, data[i]);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

        private List<DeckData> ParseDeck(List<string> deckNameRow)
        {
            _cardCollection ??= new("Cards");
            List<string> decks = new();
            if (deckNameRow.Count == 0) return new();
            int index = 0;

            do
            {
                string name = deckNameRow[index++];
                if (string.IsNullOrEmpty(name)) break;
                decks.Add(name);
                index++;
            } while (index < deckNameRow.Count && !string.IsNullOrEmpty(name));

            List<DeckData> cleanDecks = new();

            foreach (var deckName in decks)
            {
                var trimmedName = deckName.Trim();
                if (string.IsNullOrEmpty(trimmedName)) continue;

                DeckData deckData = AssetDatabase.LoadAssetAtPath<DeckData>($"{_decksPath}/{trimmedName}.asset");
                if (deckData == null)
                {
                    deckData = ScriptableObject.CreateInstance<DeckData>();
                    deckData.name = trimmedName;
                    AssetDatabase.CreateAsset(deckData, $"{_decksPath}/{trimmedName}.asset");
                }
                deckData.Cards.Clear();
                cleanDecks.Add(deckData);
                EditorUtility.SetDirty(deckData);

            }

            return cleanDecks;
        }

        protected void Populate(List<DeckData> decks, List<string> fields)
        {
            int fieldIndex = 0;
            foreach (var deck in decks)
            {

                string cardName = fields[fieldIndex++];
                int numCard = IntParse(fields[fieldIndex++]);
                if (string.IsNullOrEmpty(cardName)) continue;
                var cardData = _cardCollection.GetFromName(cardName);
                numCard.Times(() => deck.Cards.Add(cardData));
            }
        }

        public static int IntParse(string v)
        {
            if (string.IsNullOrEmpty(v))
            {
                return 0;
            }
            return int.Parse(v);
        }
    }



}

#endif
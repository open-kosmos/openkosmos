using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arkship.Parts
{
    public static class PartDictionary
    {
        private static bool IsInitialised = false;
        private static List<PartDefinition> AllPartDefs;
        private static Dictionary<PartDefinition, PartBase> PartPrefabDict;
        
        public static void Initialise()
        {
            if (IsInitialised)
            {
                return;
            }

            PartPrefabDict = new();

            AllPartDefs = new();
            var allPartDefJson = Resources.LoadAll<TextAsset>("Parts").ToList();
            foreach (var def in allPartDefJson)
            {
                AllPartDefs.Add(JsonUtility.FromJson<PartDefinition>(def.text));
            }
        }

        public static IReadOnlyList<PartDefinition> GetParts()
        {
            return AllPartDefs;
        }

        public static PartBase SpawnPart(PartDefinition def)
        {
            if (!PartPrefabDict.ContainsKey(def))
            {
                var part = Resources.Load<PartBase>(def.Path);
                Debug.Assert(part != null, $"Part {def.Name} couldn't be loaded");
                PartPrefabDict[def] = part;
            }

            PartBase prefab = null;
            PartPrefabDict.TryGetValue(def, out prefab);

            if (prefab != null)
            {
                return GameObject.Instantiate(prefab);
            }

            return null;
        }
    }
}

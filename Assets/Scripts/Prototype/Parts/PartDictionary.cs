using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kosmos.Prototype.Parts.Serialization;
using UnityEngine;

namespace Kosmos.Prototype.Parts
{
    //A class that provides access to all parts supported by the game
    public static class PartDictionary
    {
        private static bool _isInitialised = false;
        
        //Flat list of all defs
        private static List<PartDefinition> _allPartDefs;
        
        //Guid->PartDef
        private static Dictionary<System.Guid, PartDefinition> _partDefDict;
        
        //PartDef->Loaded part
        private static Dictionary<PartDefinition, PartPrefabData> _partPrefabDict;

        public static void Initialise()
        {
            if (_isInitialised)
            {
                return;
            }

            _partPrefabDict = new();
            _partDefDict = new();

            _allPartDefs = new();
            var allPartDefJson = Resources.LoadAll<TextAsset>("Parts/Definitions").ToList();
            foreach (var defText in allPartDefJson)
            {
                var def = JsonUtility.FromJson<PartDefinition>(defText.text);
                
                if (string.IsNullOrEmpty(def.Guid))
                {
                    Debug.LogError($"Part definition {def.Name} has no Guid");
                    continue;
                }
                
                System.Guid guid = Guid.Parse(def.Guid);
                if (!_partDefDict.TryAdd(guid, def))
                {
                    Debug.LogError($"Duplicate part Guid found for {def.Name} and {_partDefDict[guid].Name}");
                    continue;
                }

                _allPartDefs.Add(def);
            }
        }

        public static IReadOnlyList<PartDefinition> GetParts()
        {
            return _allPartDefs;
        }
        
        public static PartDefinition GetPart(System.Guid guid)
        {
            PartDefinition def = null;
            _partDefDict.TryGetValue(guid, out def);
            return def;
        }

        public static PartBase SpawnPart(PartDefinition def)
        {
            if (!_partPrefabDict.ContainsKey(def))
            {
                string partPath = def.Path;
                var part = Resources.Load<TextAsset>(partPath);
                Debug.Assert(part != null, $"Part {def.Name} couldn't be loaded");
                var partPrefabData = JsonUtility.FromJson<PartPrefabData>(part.text);
                _partPrefabDict[def] = partPrefabData;
            }
            

            PartPrefabData prefab = null;
            _partPrefabDict.TryGetValue(def, out prefab);

            if (prefab != null)
            {
                var part = prefab.CreateGoPart();
                var partBase = part.AddComponent<PartBase>();
                partBase.SetCreatedFromDefinition(def);
                return partBase;
            }

            return null;
        }

        public static PartPrefabData GetPartPrefabData(PartDefinition partDef)
        {
            string partPath = partDef.Path;
            var part = Resources.Load<TextAsset>(partPath);
            Debug.Assert(part != null, $"Part {partDef.Name} couldn't be loaded");
            var partPrefabData = JsonUtility.FromJson<PartPrefabData>(part.text);

            return partPrefabData;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Kosmos.Prototype.Parts
{
    public class PartTypeReflectionEntry
    {
        public List<FieldInfo> TweakableFields;
    }
    
    public static class PartDictionary
    {
        private static bool _isInitialised = false;
        
        //Flat list of all defs
        private static List<PartDefinition> _allPartDefs;
        
        //Guid->PartDef
        private static Dictionary<System.Guid, PartDefinition> _partDefDict;
        
        //PartDef->Loaded part
        private static Dictionary<PartDefinition, PartBase> _partPrefabDict;
        
        //PartBase type -> reflection info
        private static Dictionary<System.Type, PartTypeReflectionEntry> _partTypeReflectionDict;
        
        public static void Initialise()
        {
            if (_isInitialised)
            {
                return;
            }

            _partPrefabDict = new();
            _partTypeReflectionDict = new();
            _partDefDict = new();

            _allPartDefs = new();
            var allPartDefJson = Resources.LoadAll<TextAsset>("Parts").ToList();
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
                var part = Resources.Load<PartBase>(def.Path);
                Debug.Assert(part != null, $"Part {def.Name} couldn't be loaded");
                _partPrefabDict[def] = part;
            }

            PartBase prefab = null;
            _partPrefabDict.TryGetValue(def, out prefab);

            if (prefab != null)
            {
                PartBase newPart = GameObject.Instantiate(prefab);
                newPart.SetCreatedFromDefinition(def);
                return newPart;
            }

            return null;
        }

        public static IReadOnlyList<FieldInfo> GetPartTweakableFields(PartBase part)
        {
            var type = part.GetType();
            if (!_partTypeReflectionDict.ContainsKey(type))
            {
                PartTypeReflectionEntry entry = new();
                entry.TweakableFields = new();
                
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    if (field.GetCustomAttribute<TweakableAttribute>() != null)
                    {
                        entry.TweakableFields.Add(field);
                    }
                }
                _partTypeReflectionDict.Add(type, entry);
            }

            return _partTypeReflectionDict[type].TweakableFields;
        }
    }
}

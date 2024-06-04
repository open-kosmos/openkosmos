using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Arkship.Parts
{
    public class PartTypeReflectionEntry
    {
        public List<FieldInfo> TweakableFields;
    }
    
    public static class PartDictionary
    {
        private static bool _isInitialised = false;
        private static List<PartDefinition> _allPartDefs;
        private static Dictionary<PartDefinition, PartBase> _partPrefabDict;
        private static Dictionary<System.Type, PartTypeReflectionEntry> _partTypeReflectionDict;
        
        public static void Initialise()
        {
            if (_isInitialised)
            {
                return;
            }

            _partPrefabDict = new();
            _partTypeReflectionDict = new();

            _allPartDefs = new();
            var allPartDefJson = Resources.LoadAll<TextAsset>("Parts").ToList();
            foreach (var def in allPartDefJson)
            {
                _allPartDefs.Add(JsonUtility.FromJson<PartDefinition>(def.text));
            }
        }

        public static IReadOnlyList<PartDefinition> GetParts()
        {
            return _allPartDefs;
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

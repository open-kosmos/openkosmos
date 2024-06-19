
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kosmos.Prototype.Parts.TraitComponents
{
    public static class TweakableDictionary
    {
        private static readonly Dictionary<System.Type, List<FieldInfo>> Dict = new();
        
        public static List<FieldInfo> GetTweakablesForType(System.Type type)
        {
            if (!Dict.ContainsKey(type))
            {
                Dict[type] = type.GetFields(BindingFlags.Instance | BindingFlags.Public).ToList();
            }

            return Dict[type];
        }
    }
}
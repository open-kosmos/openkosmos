using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kosmos.Prototype.Parts.TraitComponents
{
    //A dictionary of the traits that exist in the game, and their factory classes
    public static class TraitDictionary
    {
        private static Dictionary<string, TraitFactory> _factoryDict;
        
        //TODO - Once we have mod loading, we'll need to make sure that this happens after the 
        // mod assemblies are loaded
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Initialize()
        {
            if (_factoryDict != null)
            {
                return;
            }

            _factoryDict = new();
            
            //Find all the factories
            var factories = AppDomain.CurrentDomain.GetAssemblies()
                // alternative: .GetExportedTypes()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(TraitFactory))
                ).ToArray();

            //Instantiate and store
            foreach (var factType in factories)
            {
                var fact = Activator.CreateInstance(factType) as TraitFactory;
                _factoryDict[fact.GetTraitType().FullName] = fact;
            }
        }

        public static TraitFactory GetFactoryForTrait(string traitType)
        {
#if UNITY_EDITOR
            if (_factoryDict == null)
            {
                Initialize();
            }
#endif
            return _factoryDict[traitType];
        }
    }
}
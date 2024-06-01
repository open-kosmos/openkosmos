using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arkship.Parts
{
    public static class PartDictionary
    {
        //TODO - This implementation is placeholder. Needs a complete re-write
        //Specifically it needs to be able to enumerate and get basic info (name, description etc.) of parts
        //without actually loading them.
        
        private static bool IsInitialised = false;
        private static List<PartBase> AllParts;
        
        public static void Initialise()
        {
            if (IsInitialised)
            {
                return;
            }

            AllParts = Resources.LoadAll<PartBase>("Parts").ToList();
            
            Debug.Log("Found parts:");
            foreach (var part in AllParts)
            {
                Debug.Log($"Found part: {part.GetName()}");
            }
        }

        public static IReadOnlyList<PartBase> GetParts()
        {
            return AllParts;
        }
    }
}

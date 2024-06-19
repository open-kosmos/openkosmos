using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Kosmos.Prototype.Parts
{
    //Data structure that defines the structure of a vehicle created in the VAB
    [System.Serializable]
    public class TweakableData
    {
        public string TypeName;
        public string Data;
    }
  
    [System.Serializable]
    public class PartSpec
    {
        public string PartDefGuid;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public List<TweakableData> TweakableData;
    }
    
    [System.Serializable]
    public class ConnectionSpec
    {
        public int ParentPartIndex;
        public int ParentSocketIndex;
        public int ChildPartIndex;
        public int ChildSockedIndex;
    }
    
    [System.Serializable]
    public class VehicleSpec
    {
        public List<PartSpec> Parts;
        public List<ConnectionSpec> Connections;

        public void Serialise(string path)
        {
            File.WriteAllText(path, JsonUtility.ToJson(this, true));
            Debug.Log($"Saved to {path}");
        }

        public static VehicleSpec Deserialise(string path)
        {
            string specJson = File.ReadAllText(path);
            return JsonUtility.FromJson<VehicleSpec>(specJson);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace Arkship.Parts
{
    [System.Serializable]
    public struct TweakableValue
    {
        public string Name;
        public string Value;

        public TweakableValue(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
    
    [System.Serializable]
    public class PartSpec
    {
        public string PartDefName;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
        public List<TweakableValue> Tweakables;
    }
    
    [System.Serializable]
    public class VehicleSpec
    {
        public List<PartSpec> Parts;

        public void Serialise()
        {
            Debug.Log(JsonUtility.ToJson(this));
        }
    }
    
}
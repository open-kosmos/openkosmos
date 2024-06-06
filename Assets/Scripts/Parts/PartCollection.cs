using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arkship.Parts
{
    public class PartCollection : MonoBehaviour
    {
        public PartBase AddPart(PartDefinition def)
        {
            var newPart = PartDictionary.SpawnPart(def);
            newPart.transform.SetParent(transform);

            return newPart;
        }
        
        public void Serialise(string path)
        {
            VehicleSpec vehicleSpec = new();
            vehicleSpec.Parts = new();

            foreach (var part in GetComponentsInChildren<PartBase>())
            {
                PartSpec partSpec = new();
                partSpec.PartDefName = part.GetDefinition().Name;
                partSpec.LocalPosition = part.transform.localPosition;
                partSpec.LocalRotation = part.transform.localRotation;
                
                //Get tweakables
                partSpec.Tweakables = new();
                foreach (var tweakableField in PartDictionary.GetPartTweakableFields(part))
                {
                    partSpec.Tweakables.Add(
                        new TweakableValue(tweakableField.Name, 
                            tweakableField.GetValue(part).ToString()));
                }

                vehicleSpec.Parts.Add(partSpec);
            }
            
            vehicleSpec.Serialise(path);
        }

        public void Deserialise(string path)
        {
            //Delete all children of our transform
            var children = new List<GameObject>();
            foreach (Transform child in transform) children.Add(child.gameObject);
            children.ForEach(child => DestroyImmediate(child));
            
            VehicleSpec spec = VehicleSpec.Deserialise(path);

            foreach (var partSpec in spec.Parts)
            {
                var partDef = PartDictionary.GetParts().FirstOrDefault(x => x.Name == partSpec.PartDefName);
                if (partDef != null)
                {
                    var part = AddPart(partDef);
                    part.transform.localPosition = partSpec.LocalPosition;
                    part.transform.localRotation = partSpec.LocalRotation;
                    
                    foreach (var tweakableField in PartDictionary.GetPartTweakableFields(part))
                    {
                        TweakableValue? field = partSpec.Tweakables.FirstOrDefault(x => x.Name == tweakableField.Name);
                        if (field.HasValue)
                        {
                            if (tweakableField.FieldType == typeof(float))
                            {
                                tweakableField.SetValue(part, float.Parse(field.Value.Value));
                            }
                            else
                            {
                                Debug.LogError($"Failed to deserialise type {tweakableField.FieldType}");
                            }
                        }
                    }
                }
            }
        }
    }
}
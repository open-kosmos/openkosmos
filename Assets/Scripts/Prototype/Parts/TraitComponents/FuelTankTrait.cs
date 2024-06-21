using System;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototype.Parts.TraitComponents
{
    public class FuelTankFactory : TraitFactory
    {
        public override Type GetTraitType()
        {
            return typeof(FuelTankTrait);
        }

        public override string SerializeTrait(MonoBehaviour trait)
        {
            return JsonUtility.ToJson((trait as FuelTankTrait).Data);
        }

        public override void DeserializeGo(string trait, GameObject gameObj)
        {
            var component = gameObj.AddComponent<FuelTankTrait>();
            component.Data = JsonUtility.FromJson<FuelTankData>(trait);
        }

        public override void DeserializeEcs(string trait, string tweakables, Entity entity, ref EntityManager entityManager)
        {
            FuelTankData traitData = JsonUtility.FromJson<FuelTankData>(trait);
            entityManager.AddComponentData(entity, traitData);
        }
    }
    
    [System.Serializable]
    public struct FuelTankData : Unity.Entities.IComponentData
    {
        public float Capacity;
    }

    [System.Serializable]
    public class FuelTankTweakableData
    {
        public float FillAmount = 1.0f;
    }
    
    public class FuelTankTrait : TraitMonoBase
    {
        public FuelTankData Data;
        public FuelTankTweakableData Tweakables = new();
        
        public override string SerializeTweakables()
        {
            return JsonUtility.ToJson(Tweakables);
        }

        public override object GetTweakables()
        {
            return Tweakables;
        }

        public override void ApplyTweakables(string data)
        {
            Tweakables = JsonUtility.FromJson<FuelTankTweakableData>(data);
        }
    }
}
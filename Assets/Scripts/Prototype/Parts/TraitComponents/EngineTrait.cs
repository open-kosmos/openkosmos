using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System;

namespace Kosmos.Prototype.Parts.TraitComponents
{
    public class EngineTraitFactory : TraitFactory
    {
        public override Type GetTraitType()
        {
            return typeof(EngineTrait);
        }
        
        public override string SerializeTrait(MonoBehaviour trait)
        {
            return JsonUtility.ToJson((trait as EngineTrait).Data);
        }

        public override void DeserializeGo(string trait,GameObject gameObj)
        {
            var component = gameObj.AddComponent<EngineTrait>();
            component.Data = JsonUtility.FromJson<EngineTraitData>(trait);
        }

        public override void DeserializeEcs(string trait, string tweakables, Entity entity, ref EntityManager entityManager)
        {
            EngineTraitData traitData = JsonUtility.FromJson<EngineTraitData>(trait);
            entityManager.AddComponentData(entity, traitData);
        }
    }
    
    public class EngineTrait : TraitMonoBase
    {
        public EngineTraitData Data;
    }
    
    [System.Serializable]
    public struct EngineTraitData : Unity.Entities.IComponentData
    {
        public float MaxPower;
        public float TestVector;
    }
}
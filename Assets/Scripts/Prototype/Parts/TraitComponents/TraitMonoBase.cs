using System;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototype.Parts.TraitComponents
{
    //Base for all traits in the game. Each trait must have at least a Monobehavior and a factory
    
    //Would be great if this could be generic (TraitFactory<T> where T: TraitMonoBase)
    //But it makes the TraitDictionary unable to store a list of instances
    //There's probably a way around that - figure it out!
    public abstract class TraitFactory
    {
        public abstract System.Type GetTraitType();
        public abstract string SerializeTrait(MonoBehaviour trait);
        public abstract void DeserializeGo(string trait, GameObject gameObj);
        public abstract void DeserializeEcs(string trait, string tweakables, Entity entity, ref SystemState state); 
    }
    
    public class TraitMonoBase : MonoBehaviour
    {
        public virtual string SerializeTweakables()
        {
            return null;
        }

        public virtual System.Object GetTweakables()
        {
            return null;
            
        }

        public virtual void ApplyTweakables(string data)
        {
        }
    }
}
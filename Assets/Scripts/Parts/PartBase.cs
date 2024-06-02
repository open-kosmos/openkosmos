using UnityEngine;

namespace Arkship.Parts
{
    public class PartBase : MonoBehaviour
    {
        //TODO:
        //COM etc.
        
        private PartDefinition CreatedFromDef;
        
        public void SetCreatedFromDefinition(PartDefinition def)
        {
            CreatedFromDef = def;
        }

        public PartDefinition GetDefinition()
        {
            return CreatedFromDef;
        }
        
        public virtual float GetMass()
        {
            return 1.0f;
        }
        
        public virtual int GetCost()
        {
            return 1;
        }
    }
}
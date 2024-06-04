using UnityEngine;

namespace Arkship.Parts
{
    public class PartBase : MonoBehaviour
    {
        //TODO:
        //COM etc.
        
        private PartDefinition _createdFromDef;
        
        public void SetCreatedFromDefinition(PartDefinition def)
        {
            _createdFromDef = def;
        }

        public PartDefinition GetDefinition()
        {
            return _createdFromDef;
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
using UnityEngine;

namespace Arkship.Parts
{
    public class PartBase : MonoBehaviour
    {
        //TODO:
        //COM etc.
        
        [SerializeField]
        private string PartName;

        [SerializeField] private string Category;
        
        [SerializeField]
        public string PartDescription;

        public string GetName()
        {
            return PartName;
        }
        
        public string GetCategory()
        {
            return Category;
        }

        public string GetDescription()
        {
            return PartDescription;
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
using System.Collections.Generic;
using UnityEngine;

namespace Kosmos.Prototype.Parts
{
    public class PartBase : MonoBehaviour
    {
        //TODO:
        //COM etc.
        
        private PartDefinition _createdFromDef;
        private IReadOnlyList<PartSocket> _sockets;
        
        public virtual void Awake()
        {
            _sockets = GetComponentsInChildren<PartSocket>();
        }
        
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

        public IReadOnlyList<PartSocket> GetSockets()
        {
            return _sockets;
        }
    }
}
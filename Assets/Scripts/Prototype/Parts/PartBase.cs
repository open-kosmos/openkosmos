using System;
using System.Collections.Generic;
using UnityEngine;

//Attached to the root of a part in the Vehicle Assembly Building. 
//Purely a runtime thing - not serialized
namespace Kosmos.Prototype.Parts
{
    public class PartBase : MonoBehaviour
    {
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

        public IReadOnlyList<PartSocket> GetSockets()
        {
            return _sockets;
        }
    }
}
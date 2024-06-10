using Kosmos.Prototype.Parts;
using UnityEngine;

namespace Kosmos.Prototype.Vab
{
    public class GizmoBase : MonoBehaviour
    {
        protected PartBase _currentPart;
        protected PartCollection _partCollection;
        
        public void AttachToPart(PartBase part, PartCollection collection)
        {
            _currentPart = part;
            _partCollection = collection;
            transform.position = part.transform.position;
        }
        
        //Used to test agains the widget's geomentry. If it hits it starts the drag
        public virtual bool TestClick(Vector2 mousePos, Camera cam)
        {
            return false;
        }
        
        public virtual void StartDrag(Vector2 mousePos, Camera cam)
        {
            
        }

        public virtual void EndDrag(Vector2 mousePos, Camera cam)
        {
            
        }
        
        public virtual void UpdateDrag(Vector2 mousePos, Vector2 mouseDelta, Camera cam)
        {
            
        } 
    }
}
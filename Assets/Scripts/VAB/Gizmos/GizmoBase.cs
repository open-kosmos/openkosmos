using Arkship.Parts;
using UnityEngine;

namespace Arkship.Vab
{
    public class GizmoBase : MonoBehaviour
    {
        protected PartBase CurrentPart;
        
        public void AttachToPart(PartBase part)
        {
            CurrentPart = part;
            transform.position = part.transform.position;
        }
        
        //Used to test agains the widget's geomentry. If it hit's it starts the drag
        public virtual bool TestClick(Vector2 mousePos, Camera cam)
        {
            return false;
        }

        public virtual void EndDrag(Vector2 mousePos, Camera cam)
        {
            
        }
        
        public virtual void UpdateDrag(Vector2 mousePos, Vector2 mouseDelta, Camera cam)
        {
            
        } 
    }
}
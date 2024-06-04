using Arkship.Vab;
using UnityEngine;

namespace Arkship.Vab
{
    public class MoveGizmo : GizmoBase
    {
        [SerializeField] private Collider _xAxis;
        [SerializeField] private Collider _yAxis;
        [SerializeField] private Collider _zAxis;

        private Vector3 _draggedAxis;
        
        public override bool TestClick(Vector2 mousePos, Camera cam)
        {
            Ray ray = cam.ScreenPointToRay(mousePos);
            
            //TODO - Sort hits in case (e.g.) y axis is in front of x
            if (_xAxis.Raycast(ray, out var _, 100.0f))
            {
                _draggedAxis = Vector3.right;
                return true;
            }
            else if (_yAxis.Raycast(ray, out var _, 100.0f))
            {
                _draggedAxis = Vector3.up;
                return true;
            }
            else if (_zAxis.Raycast(ray, out var _, 100.0f))
            {
                _draggedAxis = Vector3.forward;
                return true;
            }

            return false;
        }
        
        public override void UpdateDrag(Vector2 mousePos, Vector2 mouseDelta, Camera cam)
        {
            float partDist = Vector3.Distance(cam.transform.position, _currentPart.transform.position);
            Vector3 lastPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, partDist));
            
            Vector3 newPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x + mouseDelta.x, mousePos.y + mouseDelta.y, partDist));
            Vector3 mouseMoveWorld = newPos - lastPos;

            Vector3 move = Vector3.Dot(mouseMoveWorld, _draggedAxis) * _draggedAxis;
            
            _currentPart.transform.position += move;
            transform.position = _currentPart.transform.position;
        }
    }
}
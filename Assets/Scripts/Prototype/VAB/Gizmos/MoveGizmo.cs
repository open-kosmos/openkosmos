using Kosmos.Prototype.Vab;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kosmos.Prototype.Vab
{
    public class MoveGizmo : GizmoBase
    {
        [SerializeField] private Collider _xAxis;
        [SerializeField] private Collider _yAxis;
        [SerializeField] private Collider _zAxis;
        [SerializeField] private LineRenderer _connectionLinerenderer;
        [SerializeField] private InputActionReference _snapModeAction;

        private Vector3 _draggedAxis;

        public void Awake()
        {
            _connectionLinerenderer.enabled = false;
        }
        
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
        
        public override void StartDrag(Vector2 mousePos, Camera cam)
        {
            //Disconnect the part from any upstream parts
            _partCollection.DisconnectFromParents(_currentPart);
        }
        
        public override void UpdateDrag(Vector2 mousePos, Vector2 mouseDelta, Camera cam)
        {
            if (_snapModeAction.action.inProgress)
            {
                float partDist = Vector3.Distance(cam.transform.position, _currentPart.transform.position);
                Vector3 lastPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, partDist));
                
                Vector3 newPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x + mouseDelta.x, mousePos.y + mouseDelta.y, partDist));
                Vector3 mouseMoveWorld = newPos - lastPos;
                
                _partCollection.MovePart(_currentPart, mouseMoveWorld);
                transform.position = _currentPart.transform.position;
            }
            else
            {
                float partDist = Vector3.Distance(cam.transform.position, _currentPart.transform.position);
                Vector3 lastPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, partDist));
                
                Vector3 newPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x + mouseDelta.x, mousePos.y + mouseDelta.y, partDist));
                Vector3 mouseMoveWorld = newPos - lastPos;

                Vector3 move = Vector3.Dot(mouseMoveWorld, _draggedAxis) * _draggedAxis;
                
                _partCollection.MovePart(_currentPart, move);
                transform.position = _currentPart.transform.position;
            }
            
            //See if we should snap to something
            if (_partCollection.GetClosestPotentialConnection(_currentPart, out var partLink, cam, cam.pixelWidth * 0.05f))
            {
                _connectionLinerenderer.enabled = true;
                _connectionLinerenderer.SetPosition(0, partLink._parentSocket.transform.position);
                _connectionLinerenderer.SetPosition(1, partLink._childSocket.transform.position);
            }
            else
            {
                _connectionLinerenderer.enabled = false;
            }
        }

        public override void EndDrag(Vector2 mousePos, Camera cam)
        {
            _connectionLinerenderer.enabled = false;
            
            //See if we should snap to something
            if (_partCollection.GetClosestPotentialConnection(_currentPart, out var partLink, cam, cam.pixelWidth * 0.05f))
            {
                _partCollection.LinkParts(partLink._parentPart, partLink._parentSocket, _currentPart, partLink._childSocket);
            }
        }
    }
}
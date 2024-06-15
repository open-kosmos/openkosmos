using UnityEngine;
using UnityEngine.InputSystem;

namespace Kosmos.Prototype.Vab
{
    public class CameraController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference _mousePosition;
        [SerializeField] private InputActionReference _mouseDelta;
        [SerializeField] private InputActionReference _zoomInput;
        [SerializeField] private InputActionReference _rotateMoveVertInput;
        [SerializeField] private InputActionReference _rotate2AxisInput;
        
        [Header("Camera Control")]
        [SerializeField] private float _cameraMoveSpeed = 10.0f;
        [SerializeField] private float _cameraRotateSpeed = 10.0f;
        [SerializeField] private float _cameraZoomSpeed = 1.0f;

        [Header("Camera Distance")] 
        [SerializeField] private float _defaultDistance = 20.0f;
        [SerializeField] private float _maxDistance = 100.0f;
        [SerializeField] private float _minDistance = 1.0f;

        private Camera _cam;
        private Vector3 _camFocus;  //Maybe should be Transform + offset at some point to track moving objects?
        private float _camDistance;
        private float _xRot;
        private float _yRot;
        
        //Controls:
        //RMB: Rotate X/Y
        //Middle: Rotate Y, move up down
        //Scroll: Zoom
        //Middle click: Center on part

        public void Awake()
        {
            _cam = GetComponent<Camera>();
            _camDistance = _defaultDistance;
        }

        public void Update()
        {
            //NOTE - None of this uses deltaTime. That's because we're matching the mouse movement
            // which is absolute.
            Vector2 mouseDelta = _mouseDelta.action.ReadValue<Vector2>();
            float rotateY = (_rotate2AxisInput.action.IsPressed() || _rotateMoveVertInput.action.IsPressed()) ? 
                mouseDelta.x : 0.0f;
            float rotateX = _rotate2AxisInput.action.IsPressed() ? mouseDelta.y : 0.0f;
            float moveY = _rotateMoveVertInput.action.IsPressed() ? mouseDelta.y : 0.0f;
            float zoom = -_zoomInput.action.ReadValue<float>();
                
            //Rotate y
            _yRot += rotateY * _cameraRotateSpeed;;

            _xRot += (-rotateX) * _cameraRotateSpeed;
            _xRot = Mathf.Clamp(_xRot, -89.0f, 89.0f);

            //TODO - take the camera distance into account. Probably convert the 
            //mouse delta into a world space movement at the focus distance and use that
            _camFocus += Vector3.up * (-moveY * _cameraMoveSpeed);

            _camDistance += zoom * _cameraZoomSpeed;
            _camDistance = Mathf.Clamp(_camDistance, _minDistance, _maxDistance);
            
            //Calculate the new camera transform
            transform.position = _camFocus - Vector3.forward * _camDistance;
            transform.rotation = Quaternion.identity;
            
            transform.RotateAround(_camFocus, Vector3.up, _yRot);
            transform.RotateAround(_camFocus, transform.right, _xRot);
        }

        public void SetCameraTarget(Vector3 target)
        {
            _camFocus = target;
            
            //TODO - Calculate distance and rotations from current position
            // so the camera doesn't move when switching target
        }

        public float GetCamDistance()
        {
            return _camDistance;
        }
    }
}
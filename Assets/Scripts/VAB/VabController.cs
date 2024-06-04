using Arkship.Parts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Arkship.Vab
{
    public class VabController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private PartPickerPanel _partPickerPanel;
        [SerializeField] private PartInfoPanel _partInfoPanel;
        [SerializeField] private Camera _mainCam;
        
        [Header("Gizmos")]
        [SerializeField] private GizmoBase _moveGizmo;
        
        [Header("Input")]
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _mousePosition;
        [SerializeField] private InputActionReference _mouseDelta;
        
        [Header("Camera Control")]
        [SerializeField] private float _cameraMoveSpeed = 0.5f;
        [SerializeField] private float _cameraRotateSpeed = 10.0f;
        
        private GameObject _vehicleRoot;
        private PartBase _selectedPart;

        private enum EControlState
        {
            None,
            DraggingGizmo,
            MovingCamera,
        }

        private EControlState _controlState = EControlState.None;
        private GizmoBase _currentGizmo; 

        void Start()
        {
            _partPickerPanel.OnPartPicked += OnPartPickerClicked;
            
            _currentGizmo = _moveGizmo;

            _vehicleRoot = new GameObject("VehicleRoot");
            SelectPart(null);

        }

        private void OnPartPickerClicked(PartDefinition part)
        {
            var newPart = PartDictionary.SpawnPart(part);
            newPart.transform.SetParent(_vehicleRoot.transform);
            SelectPart(newPart);
        }

        public void OnScreenClick(InputAction.CallbackContext context)
        {
            //Part selection
            if (context.phase == InputActionPhase.Performed && _controlState == EControlState.None)
            {
                Vector2 mousePos = _mousePosition.action.ReadValue<Vector2>();
                var ray = _mainCam.ScreenPointToRay(mousePos);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    var part = hit.collider.GetComponentInParent<PartBase>();
                    if (part != null)
                    {
                        SelectPart(part);
                    }
                }
                else
                {
                    SelectPart(null);
                }
            }
        }

        public void OnScreenDrag(InputAction.CallbackContext context)
        {
            //Gizmo handling
            if (context.phase == InputActionPhase.Started && _controlState == EControlState.None)
            {
                if (_currentGizmo.TestClick(_mousePosition.action.ReadValue<Vector2>(), _mainCam))
                {
                    _controlState = EControlState.DraggingGizmo;
                }
            }
            else if (context.phase == InputActionPhase.Canceled && _controlState == EControlState.DraggingGizmo)
            {
                _currentGizmo.EndDrag(_mousePosition.action.ReadValue<Vector2>(), _mainCam);
                _controlState = EControlState.None;
            }
        }
        
        public void OnMoveCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    if (_controlState == EControlState.None)
                    {
                        _controlState = EControlState.MovingCamera;
                    }
                    break;
                case InputActionPhase.Canceled:
                    if (_controlState == EControlState.MovingCamera)
                    {
                        _controlState = EControlState.None;
                    }
                    break;
            }
        }

        public void DeleteClicked(InputAction.CallbackContext context)
        {
            TestSerialise();
            // if (SelectedPart != null)
            // {
            //     GameObject.Destroy(SelectedPart.gameObject);
            //     SelectPart(null);
            // }
        }
        
        public void Update()
        {
            switch (_controlState)
            {
                case EControlState.None:
                    //Manual moving with cursor keys
                    if (_selectedPart != null)
                    {
                        Vector3 move = _moveAction.action.ReadValue<Vector3>();
                        _selectedPart.transform.localPosition += move * Time.deltaTime;
                    }
                    break;
                case EControlState.DraggingGizmo:
                    _currentGizmo.UpdateDrag(_mousePosition.action.ReadValue<Vector2>(), _mouseDelta.action.ReadValue<Vector2>(), _mainCam);
                    break;
                case EControlState.MovingCamera:
                    Vector2 delta = _mouseDelta.action.ReadValue<Vector2>();
                    float verticalMove = -delta.y * _cameraMoveSpeed;
                    float rotate = delta.x * _cameraRotateSpeed;
                    _mainCam.transform.position += new Vector3(0, verticalMove * Time.deltaTime, 0.0f);
                    _mainCam.transform.RotateAround(Vector3.zero, Vector3.up, rotate * Time.deltaTime);
                    break;
            }
        }
        
        private void SelectPart(PartBase part)
        {
            _selectedPart = part;

            if (_selectedPart != null)
            {
                _currentGizmo.gameObject.SetActive(true);
                _currentGizmo.AttachToPart(_selectedPart);
                _partInfoPanel.SetPart(_selectedPart);
            }
            else
            {
                _moveGizmo.gameObject.SetActive(false);
                _partInfoPanel.SetPart(null);
            }
        }

        private void TestSerialise()
        {
            VehicleSpec vehicleSpec = new();
            vehicleSpec.Parts = new();

            foreach (var part in _vehicleRoot.GetComponentsInChildren<PartBase>())
            {
                PartSpec partSpec = new();
                partSpec.PartDefName = part.GetDefinition().Name;
                partSpec.LocalPosition = part.transform.localPosition;
                partSpec.LocalRotation = part.transform.localRotation;
                
                //Get tweakables
                partSpec.Tweakables = new();
                foreach (var tweakableField in PartDictionary.GetPartTweakableFields(part))
                {
                    partSpec.Tweakables.Add(
                        new TweakableValue(tweakableField.Name, 
                        tweakableField.GetValue(part).ToString()));
                }

                vehicleSpec.Parts.Add(partSpec);
            }
            
            vehicleSpec.Serialise();
        }
    }
}
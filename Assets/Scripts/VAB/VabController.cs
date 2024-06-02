using System;
using System.Collections.Generic;
using System.Reflection;
using Arkship.Parts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
        
        private GameObject VehicleRoot;
        private PartBase SelectedPart;

        private enum EControlState
        {
            None,
            DraggingGizmo,
            MovingCamera,
        }

        private EControlState ControlState = EControlState.None;
        private GizmoBase CurrentGizmo; 

        void Start()
        {
            _partPickerPanel.OnPartPicked += OnPartPickerClicked;
            
            CurrentGizmo = _moveGizmo;

            VehicleRoot = new GameObject("VehicleRoot");
            SelectPart(null);

        }

        private void OnPartPickerClicked(PartDefinition part)
        {
            var newPart = PartDictionary.SpawnPart(part);
            newPart.transform.SetParent(VehicleRoot.transform);
            SelectPart(newPart);
        }

        public void OnScreenClick(InputAction.CallbackContext context)
        {
            //Part selection
            if (context.phase == InputActionPhase.Performed && ControlState == EControlState.None)
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
            if (context.phase == InputActionPhase.Started && ControlState == EControlState.None)
            {
                if (CurrentGizmo.TestClick(_mousePosition.action.ReadValue<Vector2>(), _mainCam))
                {
                    ControlState = EControlState.DraggingGizmo;
                }
            }
            else if (context.phase == InputActionPhase.Canceled && ControlState == EControlState.DraggingGizmo)
            {
                CurrentGizmo.EndDrag(_mousePosition.action.ReadValue<Vector2>(), _mainCam);
                ControlState = EControlState.None;
            }
        }
        
        public void OnMoveCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    if (ControlState == EControlState.None)
                    {
                        ControlState = EControlState.MovingCamera;
                    }
                    break;
                case InputActionPhase.Canceled:
                    if (ControlState == EControlState.MovingCamera)
                    {
                        ControlState = EControlState.None;
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
            switch (ControlState)
            {
                case EControlState.None:
                    //Manual moving with cursor keys
                    if (SelectedPart != null)
                    {
                        Vector3 move = _moveAction.action.ReadValue<Vector3>();
                        SelectedPart.transform.localPosition += move * Time.deltaTime;
                    }
                    break;
                case EControlState.DraggingGizmo:
                    CurrentGizmo.UpdateDrag(_mousePosition.action.ReadValue<Vector2>(), _mouseDelta.action.ReadValue<Vector2>(), _mainCam);
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
            SelectedPart = part;

            if (SelectedPart != null)
            {
                CurrentGizmo.gameObject.SetActive(true);
                CurrentGizmo.AttachToPart(SelectedPart);
                _partInfoPanel.SetPart(SelectedPart);
            }
            else
            {
                _moveGizmo.gameObject.SetActive(false);
                _partInfoPanel.SetPart(null);
            }
        }

        private void TestSerialise()
        {
            Parts.VehicleSpec vehicleSpec = new();
            vehicleSpec.Parts = new();

            foreach (var part in VehicleRoot.GetComponentsInChildren<PartBase>())
            {
                Parts.PartSpec partSpec = new();
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
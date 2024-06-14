using System;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class PannableEntitiesCamera : MonoBehaviour
    {
        [SerializeField] private float _panSpeed = 1f;
        [SerializeField] private float _currentYawAngle = 0f;
        [SerializeField] private float _currentPitchAngle = 0f;
        [SerializeField] private float _currentZoomSpeed = 10f;

        private UnityEngine.Camera _cam;
        private EntityManager _entityManager;
        private SystemHandle _cameraPositionUpdateSystem;
        
        private void Awake()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            _cameraPositionUpdateSystem = World.DefaultGameObjectInjectionWorld
                .GetExistingSystem<CameraPositionUpdateSystem>();
            
            var currentOffset = _cam.transform.position;
            var distance = currentOffset.magnitude;

            _entityManager.AddComponentData(_cameraPositionUpdateSystem, new PannableCameraData()
            {
                Camera = _cam,
                CameraPanSpeed = _panSpeed,
                CurrentDistance = distance,
                CurrentPitchAngle = _currentPitchAngle,
                CurrentYawAngle = _currentYawAngle,
                CameraZoomSpeed = _currentZoomSpeed
            });
        }

        private void OnValidate()
        {
            if (_cam == null)
            {
                return;
            }
            
            var currentOffset = _cam.transform.position;
            var distance = currentOffset.magnitude;
            
            _entityManager.SetComponentData(_cameraPositionUpdateSystem, new PannableCameraData()
            {
                Camera = _cam,
                CameraPanSpeed = _panSpeed,
                CurrentDistance = distance,
                CurrentPitchAngle = _currentPitchAngle,
                CurrentYawAngle = _currentYawAngle,
                CameraZoomSpeed = _currentZoomSpeed
            });    
        }
    }
}

using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Kosmos.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class OrbitingEntitiesCamera : MonoBehaviour
    {
        [SerializeField] private float _panSpeed = 1f;
        [SerializeField] private float _currentYawAngle = 0f;
        [SerializeField] private float _currentPitchAngle = 0f;
        [SerializeField] private float _currentZoomSpeed = 10f;
        [SerializeField] private float _currentOrbitSpeed = 10f;

        private UnityEngine.Camera _cam;
        private EntityManager _entityManager;
        private SystemHandle _cameraPositionUpdateSystem;
        
        private void Awake()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            _cameraPositionUpdateSystem = World.DefaultGameObjectInjectionWorld
                .GetExistingSystem<CameraOrbitUpdateSystem>();
            
            var currentOffset = (float3)_cam.transform.position;

            _entityManager.AddComponentData(_cameraPositionUpdateSystem, new OrbitingCameraData()
            {
                Camera = _cam,
                CameraPanSpeed = _panSpeed,
                BaseOffset = currentOffset,
                CurrentPitchAngle = _currentPitchAngle,
                CurrentYawAngle = _currentYawAngle,
                CameraZoomSpeed = _currentZoomSpeed,
                CameraOrbitSpeed = _currentOrbitSpeed
            });
        }

        private void OnValidate()
        {
            if (_cam == null)
            {
                return;
            }
            
            var currentOffset = (float3)_cam.transform.position;
            
            _entityManager.SetComponentData(_cameraPositionUpdateSystem, new OrbitingCameraData()
            {
                Camera = _cam,
                CameraPanSpeed = _panSpeed,
                BaseOffset = currentOffset,
                CurrentPitchAngle = _currentPitchAngle,
                CurrentYawAngle = _currentYawAngle,
                CameraZoomSpeed = _currentZoomSpeed,
                CameraOrbitSpeed = _currentOrbitSpeed
            });    
        }
    }
}

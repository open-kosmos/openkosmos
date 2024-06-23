using Assets.Scripts.Prototype.FlightControl.Components;
using Kosmos.Camera;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Prototype.FlightControl.Mono
{
    [RequireComponent(typeof(Camera))]
    public class FlightControlCamera : MonoBehaviour
    {
        private Camera _cam;
        private EntityManager _entityManager;
        private SystemHandle _cameraSystem;

        private void Awake()
        {

            _cam = GetComponent<Camera>();

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _cameraSystem = World.DefaultGameObjectInjectionWorld
                .GetExistingSystem<CameraSystem>();

            _entityManager.AddComponentData(_cameraSystem, new FlightControlCameraData()
            {
                Camera = _cam,
            });
        }
    }
}

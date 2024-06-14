using Unity.Entities;
using UnityEngine;

namespace Kosmos.Camera
{
    [UpdateBefore(typeof(CameraPositionUpdateSystem))]
    public partial class GetCameraPanInputSystem : SystemBase
    {
        private PannableCameraControls _planetaryMapControls;

        protected override void OnCreate()
        {
            _planetaryMapControls = new PannableCameraControls();
        }

        protected override void OnStartRunning()
        {
            _planetaryMapControls.Enable();
            EntityManager.CreateSingleton<CameraPanInput>();
        }

        protected override void OnUpdate()
        {
            var currentPanInput = _planetaryMapControls.PlanetaryMap.Pan.ReadValue<Vector2>();
            var currentCameraOrbit = _planetaryMapControls.PlanetaryMap.CameraOrbit.ReadValue<Vector2>();
            var cameraOrbitActive = _planetaryMapControls.PlanetaryMap.CameraOrbitActivation.IsPressed();
            var currentZoomInput = _planetaryMapControls.PlanetaryMap.CameraZoom.ReadValue<Vector2>();
            
            // Write to pan input component
            SystemAPI.SetSingleton(new CameraPanInput()
            {
                PanInputValue = currentPanInput,
                OrbitInputValue = currentCameraOrbit,
                OrbitActive = cameraOrbitActive,
                ZoomInputValue = currentZoomInput
            });
        }

        protected override void OnStopRunning()
        {
            _planetaryMapControls.Disable();
        }
    }
}

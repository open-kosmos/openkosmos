using Kosmos.Prototype.Character;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Camera
{
    /// <summary>
    /// System responsible for getting player input and writing it to a singleton component.
    /// </summary>
    [UpdateBefore(typeof(CameraOrbitUpdateSystem))]
    [UpdateBefore(typeof(PlayableCharacterMovementSystem))]
    public partial class GetPlayerInputSystem : SystemBase
    {
        private PannableCameraControls playerInputControls;

        protected override void OnCreate()
        {
            playerInputControls = new PannableCameraControls();
        }

        protected override void OnStartRunning()
        {
            playerInputControls.Enable();
            EntityManager.CreateSingleton<PlayerInput>();
        }

        protected override void OnUpdate()
        {
            var translationInput = playerInputControls.PlanetaryMap.Pan.ReadValue<Vector2>();
            var currentCameraOrbit = playerInputControls.PlanetaryMap.CameraOrbit.ReadValue<Vector2>();
            var cameraOrbitActive = playerInputControls.PlanetaryMap.CameraOrbitActivation.IsPressed();
            var currentZoomInput = playerInputControls.PlanetaryMap.CameraZoom.ReadValue<Vector2>();
            
            // Write to pan input component
            SystemAPI.SetSingleton(new PlayerInput()
            {
                TranslationValue = translationInput,
                OrbitInputValue = currentCameraOrbit,
                OrbitActive = cameraOrbitActive,
                ZoomInputValue = currentZoomInput
            });
        }

        protected override void OnStopRunning()
        {
            playerInputControls.Disable();
        }
    }
}

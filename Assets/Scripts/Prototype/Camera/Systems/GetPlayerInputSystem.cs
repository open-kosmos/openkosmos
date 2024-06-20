using Kosmos.Prototype.Character;
using Kosmos.Time;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

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
            
            playerInputControls.PlanetaryMap.TimeSpeedPauseToggle.performed += OnTimeSpeedPauseToggle;
            playerInputControls.PlanetaryMap.TimeSpeed1.performed += OnTimeSpeedOne;
            playerInputControls.PlanetaryMap.TimeSpeed2.performed += OnTimeSpeedTwo;
            playerInputControls.PlanetaryMap.TimeSpeed3.performed += OnTimeSpeedThree;
            playerInputControls.PlanetaryMap.TimeSpeed4.performed += OnTimeSpeedFour;
            playerInputControls.PlanetaryMap.TimeSpeed5.performed += OnTimeSpeedFive;
            playerInputControls.PlanetaryMap.TimeSpeed6.performed += OnTimeSpeedSix;
            playerInputControls.PlanetaryMap.TimeSpeed7.performed += OnTimeSpeedSeven;
        }

        protected override void OnUpdate()
        {
            var translationInput = playerInputControls.PlanetaryMap.Pan.ReadValue<Vector2>();
            var currentCameraOrbit = playerInputControls.PlanetaryMap.CameraOrbit.ReadValue<Vector2>();
            var cameraOrbitActive = playerInputControls.PlanetaryMap.CameraOrbitActivation.IsPressed();
            var currentZoomInput = playerInputControls.PlanetaryMap.CameraZoom.ReadValue<Vector2>();
            var scaleZoomActive = playerInputControls.PlanetaryMap.ScaleZoomModifier.IsPressed();
            
            // Write to pan input component
            SystemAPI.SetSingleton(new PlayerInput()
            {
                TranslationValue = translationInput,
                OrbitInputValue = currentCameraOrbit,
                OrbitActive = cameraOrbitActive,
                ZoomInputValue = currentZoomInput,
                ScaleZoomActive = scaleZoomActive
            });
        }

        protected override void OnStopRunning()
        {
            playerInputControls.PlanetaryMap.TimeSpeedPauseToggle.performed -= OnTimeSpeedPauseToggle;
            playerInputControls.PlanetaryMap.TimeSpeed1.performed -= OnTimeSpeedOne;
            playerInputControls.PlanetaryMap.TimeSpeed2.performed -= OnTimeSpeedTwo;
            playerInputControls.PlanetaryMap.TimeSpeed3.performed -= OnTimeSpeedThree;
            playerInputControls.PlanetaryMap.TimeSpeed4.performed -= OnTimeSpeedFour;
            playerInputControls.PlanetaryMap.TimeSpeed5.performed -= OnTimeSpeedFive;
            playerInputControls.PlanetaryMap.TimeSpeed6.performed -= OnTimeSpeedSix;
            playerInputControls.PlanetaryMap.TimeSpeed7.performed -= OnTimeSpeedSeven;
            
            playerInputControls.Disable();
        }
        
        private void OnTimeSpeedPauseToggle(InputAction.CallbackContext obj)
        {
            var timeEntity = SystemAPI.GetSingletonEntity<UniversalTime>();
            var currentPaused = EntityManager.GetComponentData<UniversalTimePaused>(timeEntity).Value;
            
            var universalTimePaused = new UniversalTimePaused()
            {
                Value = !currentPaused
            };
            
            EntityManager.SetComponentData(timeEntity, universalTimePaused);
        }
        
        private void OnTimeSpeedOne(InputAction.CallbackContext obj)
        {
            OnTimeSpeedChanged(1.0);
        }
        
        private void OnTimeSpeedTwo(InputAction.CallbackContext obj)
        {
            OnTimeSpeedChanged(3.0);
        }
        
        private void OnTimeSpeedThree(InputAction.CallbackContext obj)
        {
            OnTimeSpeedChanged(10.0);
        }
        
        private void OnTimeSpeedFour(InputAction.CallbackContext obj)
        {
            OnTimeSpeedChanged(500.0);
        }
        
        private void OnTimeSpeedFive(InputAction.CallbackContext obj)
        {
            OnTimeSpeedChanged(50000.0);
        }
        
        private void OnTimeSpeedSix(InputAction.CallbackContext obj)
        {
            OnTimeSpeedChanged(500000.0);
        }
        
        private void OnTimeSpeedSeven(InputAction.CallbackContext obj)
        {
            OnTimeSpeedChanged(5000000.0);
        }
        
        private void OnTimeSpeedChanged(double newTimeSpeed)
        {
            var timeEntity = SystemAPI.GetSingletonEntity<UniversalTime>();
            
            var universalTimeModifier = new UniversalTimeModifier()
            {
                Value = newTimeSpeed
            };
            
            EntityManager.SetComponentData<UniversalTimeModifier>(timeEntity, universalTimeModifier);
        }
    }
}

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Kosmos.FloatingOrigin;
using Kosmos.Math;
using UnityEngine;

namespace Kosmos.Camera
{
    [UpdateBefore(typeof(FloatingOriginSnapCheckSystem))]
    public partial class CameraOrbitUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerInput>();
            RequireForUpdate<OrbitingCameraData>();
            RequireForUpdate<FloatingFocusData>();
        }

        protected override void OnUpdate()
        {
            var camera = EntityManager.GetComponentData<OrbitingCameraData>(SystemHandle);
            
            var input = SystemAPI.GetSingleton<PlayerInput>();
            
            var focusEntity = SystemAPI.GetSingletonEntity<FloatingFocusData>();
            var focusPosition = EntityManager.GetComponentData<LocalTransform>(focusEntity).Position;

            if (input.OrbitActive)
            {
                var newPitchAngle = camera.CurrentPitchAngle 
                                    - input.OrbitInputValue.y 
                                    * camera.CameraOrbitSpeed
                                    * SystemAPI.Time.DeltaTime;
                var newYawAngle = camera.CurrentYawAngle 
                                  + input.OrbitInputValue.x 
                                  * camera.CameraOrbitSpeed
                                  * SystemAPI.Time.DeltaTime;
                
                camera.CurrentPitchAngle = math.clamp(newPitchAngle, -89.9f, 89.9f);
                camera.CurrentYawAngle = newYawAngle;
                
                EntityManager.SetComponentData(SystemHandle, camera);
            }
            
            var cameraTransform = camera.Camera.transform;
            
            var pitchRotation = quaternion.Euler(math.radians(camera.CurrentPitchAngle), 0, 0);
            var yawRotation = quaternion.Euler(0, math.radians(camera.CurrentYawAngle), 0);
            
            var rotation = math.mul(pitchRotation, yawRotation);
            var cameraOffset = camera.CurrentOffset;
            var offset = math.mul(rotation, cameraOffset);
            
            cameraTransform.position = focusPosition + offset;
            
            cameraTransform.LookAt(focusPosition);
        }
    }
}

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Kosmos.FloatingOrigin;
using Kosmos.Math;
using UnityEngine;

namespace Kosmos.Camera
{
    /// <summary>
    /// System responsible for updating the camera's orbit values based on player input.
    /// </summary>
    [UpdateBefore(typeof(FloatingOriginSnapCheckSystem))]
    public partial class CameraOrbitUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerInput>();
            RequireForUpdate<OrbitingCameraData>();
            RequireForUpdate<FloatingFocusTag>();
        }

        protected override void OnUpdate()
        {
            var camera = EntityManager.GetComponentData<OrbitingCameraData>(SystemHandle);
            var input = SystemAPI.GetSingleton<PlayerInput>();
            
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
            
            var rotation = quaternion.Euler(
                math.radians(camera.CurrentPitchAngle), 
                math.radians(camera.CurrentYawAngle), 
                0);
            
            var baseOffset = camera.BaseOffset;
            var offset = math.mul(rotation, baseOffset);

            camera.CurrentOffset = offset;
        }
    }
}

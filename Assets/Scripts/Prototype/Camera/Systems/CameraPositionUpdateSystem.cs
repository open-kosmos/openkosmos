using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Kosmos.FloatingOrigin;
using Kosmos.Math;
using UnityEngine;

namespace Kosmos.Camera
{
    [UpdateBefore(typeof(FloatingOriginSnapCheckSystem))]
    public partial class CameraPositionUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<CameraPanInput>();
            RequireForUpdate<PannableCameraData>();
            RequireForUpdate<FloatingOriginData>();
            RequireForUpdate<FloatingFocusData>();
        }

        protected override void OnUpdate()
        {
            var camera = EntityManager.GetComponentData<PannableCameraData>(SystemHandle);
            
            var input = SystemAPI.GetSingleton<CameraPanInput>();
            
            var floatingOriginData = SystemAPI.GetSingleton<FloatingOriginData>();

            var focusData = SystemAPI.GetSingletonEntity<FloatingFocusData>();
            var focusEntityPosition = EntityManager.GetComponentData<FloatingPositionData>(focusData);
            var focusTransformPosition = EntityManager.GetComponentData<LocalTransform>(focusData).Position;
            
            focusTransformPosition.x += (input.PanInputValue.x * camera.CameraPanSpeed * SystemAPI.Time.DeltaTime);
            focusTransformPosition.z += (input.PanInputValue.y * camera.CameraPanSpeed * SystemAPI.Time.DeltaTime);

            focusEntityPosition = FloatingOriginMath.PositionDataFromCurrentWorldSpace(
                floatingOriginData,
                focusTransformPosition, 
                focusEntityPosition.Scale);
            
            EntityManager.SetComponentData(focusData, focusEntityPosition);

            if (input.OrbitActive)
            {
                var newPitchAngle = camera.CurrentPitchAngle + input.OrbitInputValue.y * SystemAPI.Time.DeltaTime;
                var newYawAngle = camera.CurrentYawAngle + input.OrbitInputValue.x * SystemAPI.Time.DeltaTime;
                
                camera.CurrentPitchAngle = math.clamp(newPitchAngle, -89.9f, 89.9f);
                camera.CurrentYawAngle = newYawAngle;
                
                EntityManager.SetComponentData(SystemHandle, camera);
            }
            
            if (input.ZoomInputValue.magnitude > 0.1f)
            {
                floatingOriginData.Scale -= input.ZoomInputValue.y 
                                            * SystemAPI.Time.DeltaTime 
                                            * (floatingOriginData.Scale / 10.0) 
                                            * camera.CameraZoomSpeed;
                
                SystemAPI.SetSingleton<FloatingOriginData>(floatingOriginData);
            }
            
            var cameraTransform = camera.Camera.transform;
            var focusWorldPosition = (float3) FloatingOriginMath.WorldSpaceFromPosition(
                floatingOriginData, focusEntityPosition);
            
            cameraTransform.position = focusWorldPosition
                                       + KMath.GetSphericalDirectionVector(camera.CurrentPitchAngle, camera.CurrentYawAngle)
                                       * camera.CurrentDistance;
            
            cameraTransform.LookAt(focusWorldPosition);
        }
    }
}

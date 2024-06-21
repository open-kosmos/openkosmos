using Kosmos.FloatingOrigin;
using Kosmos.Prototype.Character;
using Unity.Entities;
using Unity.Transforms;

namespace Kosmos.Camera
{
    /// <summary>
    /// System responsible for updating the camera's position based on the focus entity's
    /// position and current orbit values.
    /// </summary>
    [UpdateAfter(typeof(FloatingPositionToWorldPositionUpdateSystem))]
    public partial class CameraPositionUpdateSystem : SystemBase
    {
        private SystemHandle _cameraOrbitUpdateSystem;
        
        protected override void OnCreate()
        {
            RequireForUpdate<FloatingFocusTag>();
            _cameraOrbitUpdateSystem = World.GetExistingSystem<CameraOrbitUpdateSystem>();
        }

        protected override void OnUpdate()
        {
            var cameraData = EntityManager.GetComponentData<OrbitingCameraData>(_cameraOrbitUpdateSystem);
            var cameraTransform = cameraData.Camera.transform;

            Entities
                .WithAll<FloatingFocusTag>()
                .ForEach((in LocalTransform transform) =>
                {
                    cameraTransform.position = transform.Position + cameraData.CurrentOffset;
                    
                    // Get offset slightly above the focus entity
                    var lookAtPosition = transform.Position + new Unity.Mathematics.float3(0, 1, 0);
                    
                    cameraTransform.LookAt(lookAtPosition);
                })
                .WithoutBurst()
                .Run();
        }
    }
}
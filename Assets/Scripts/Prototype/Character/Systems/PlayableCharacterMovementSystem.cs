using Kosmos.Camera;
using Kosmos.FloatingOrigin;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Kosmos.Prototype.Character
{
    /// <summary>
    /// System responsible for moving a player character based on input.
    /// </summary>
    [UpdateAfter(typeof(CameraOrbitUpdateSystem))]
    public partial class PlayableCharacterMovementSystem : SystemBase
    {
        private SystemHandle _cameraOrbitUpdateSystem;
        protected override void OnCreate()
        {
            RequireForUpdate<PlayableCharacterData>();
            RequireForUpdate<PlayerInput>();
            RequireForUpdate<OrbitingCameraData>();
            
            _cameraOrbitUpdateSystem = World.GetExistingSystem<CameraOrbitUpdateSystem>();
        }

        protected override void OnUpdate()
        {
            var cameraData = EntityManager.GetComponentData<OrbitingCameraData>(_cameraOrbitUpdateSystem);
            var cameraPosition = (float3) cameraData.Camera.transform.position;

            var input = SystemAPI.GetSingleton<PlayerInput>();
            
            Entities
                .ForEach((ref LocalTransform transform, 
                    ref TargetRotation targetRotation, 
                    ref PlayableCharacterData characterData) =>
                {
                    var playerPosition = transform.Position;

                    characterData.MoveSpeed += input.ZoomInputValue.y * 10f;
                    
                    // Forward input moves the player in the directionaway from the camera
                    var forward = math.normalize(playerPosition - cameraPosition);
                    var right = math.cross(forward, new float3(0, 1, 0));
                    
                    var translation = input.TranslationValue.y * forward - input.TranslationValue.x * right;
                    
                    transform.Position += translation * characterData.MoveSpeed * SystemAPI.Time.DeltaTime;
                    
                    targetRotation.Value = quaternion.LookRotation(forward, new float3(0, 1, 0));
                    
                    transform.Rotation = math.slerp(transform.Rotation, targetRotation.Value, characterData.RotationSpeed * SystemAPI.Time.DeltaTime);
                })
                .Schedule();
        }
    }
}
using Kosmos.Camera;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Kosmos.Prototype.Character
{
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
                .ForEach((ref LocalTransform transform, in PlayableCharacterData characterData) =>
                {
                    var playerPosition = transform.Position;
                    
                    // Forward input moves the player in the directionaway from the camera
                    var forward = math.normalize(playerPosition - cameraPosition);
                    var right = math.cross(forward, new float3(0, 1, 0));
                    
                    var translation = input.TranslationValue.y * forward - input.TranslationValue.x * right;
                    
                    transform.Position += translation * characterData.MoveSpeed * SystemAPI.Time.DeltaTime;
                    transform.Rotation = quaternion.LookRotation(forward, new float3(0, 1, 0));
                })
                .Schedule();
        }
    }
}
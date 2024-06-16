using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Kosmos.FloatingOrigin
{
    /// <summary>
    /// System responsible for checking how distant each physics object is from
    /// the current origin, and toggling physics activity based on that distance.
    /// </summary>
    public partial struct DistanceBasedPhysicsActivitySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FloatingOriginData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var floatingOrigin = SystemAPI.GetSingleton<FloatingOriginData>();
            
            new CheckDistanceBasedPhysicsActivityJob()
            {
                FloatingOrigin = floatingOrigin
            }.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }
    }

    [BurstCompile]
    public partial struct CheckDistanceBasedPhysicsActivityJob : IJobEntity
    {
        public FloatingOriginData FloatingOrigin;

        private void Execute(
            Entity entity,
            [ChunkIndexInQuery] int sortKey,
            in FloatingPositionData floatingPosition,
            ref PhysicsMassOverride physicsMassOverride
        )
        {
            // Get distance from floating origin
            var distance = FloatingOriginMath.VectorFromFloatingOrigin(FloatingOrigin, floatingPosition);
            
            // If distance is greater than threshold, deactivate physics
            if (math.length(distance) > 20000f)
            {
                physicsMassOverride.IsKinematic = 1;
                physicsMassOverride.SetVelocityToZero = 1;
            }
            else
            {
                physicsMassOverride.IsKinematic = 0;
                physicsMassOverride.SetVelocityToZero = 0;
            }
        }
    }
}
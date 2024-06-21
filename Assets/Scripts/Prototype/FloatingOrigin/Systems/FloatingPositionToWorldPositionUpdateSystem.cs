using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Kosmos.FloatingOrigin
{
    /// <summary>
    /// System responsible for resetting the floating origin and updating all
    /// world positions to their current floating positions.
    /// </summary>
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct FloatingPositionToWorldPositionUpdateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FloatingOriginData>();
            state.RequireForUpdate<FloatingFocusTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var floatingOrigin = SystemAPI.GetSingleton<FloatingOriginData>();

            if (!floatingOrigin.ShouldSnap)
            {
                return;
            }
            
            var focusEntity = SystemAPI.GetSingletonEntity<FloatingFocusTag>();
            var focusEntityTransform = state.EntityManager.GetComponentData<LocalTransform>(focusEntity);
            
            // Get focus entity's current world space position
            var focusPosition = focusEntityTransform.Position;
            
            // Reset focus entity's position to origin
            focusEntityTransform.Position -= focusPosition;
            state.EntityManager.SetComponentData(focusEntity, focusEntityTransform);
            
            // Set all floating positions to current world positions
            new WorldPositionToFloatingPositionUpdateJob()
            {
                FloatingOrigin = floatingOrigin
            }.ScheduleParallel();
            
            // Adjust floating origin
            floatingOrigin = FloatingOriginMath.Add(floatingOrigin, focusPosition);
            //floatingOrigin.Local += focusPosition;
            
            // Set all world positions to floating positions relative to new origin
            new FloatingPositionToWorldPositionUpdateJob()
            {
                FloatingOrigin = floatingOrigin
            }.ScheduleParallel();
            
            floatingOrigin.ShouldSnap = false;
            SystemAPI.SetSingleton(floatingOrigin);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }
    }

    [BurstCompile]
    public partial struct WorldPositionToFloatingPositionUpdateJob : IJobEntity
    {
        public FloatingOriginData FloatingOrigin;
        
        private void Execute(
            in LocalTransform localTransform,
            ref FloatingPositionData floatingPositionData)
        {
            // Get world space position currently pointed at by floating position
            var floatingPosWorldSpace = FloatingOriginMath.VectorFromFloatingOrigin(
                FloatingOrigin, floatingPositionData);
            
            // Get actual world space position
            var worldSpacePos = localTransform.Position;
            
            // Get vector from floating WS pos to actual WS pos
            var relativePos = worldSpacePos - floatingPosWorldSpace;
            
            // Add this vector to the floating position
            floatingPositionData = FloatingOriginMath.Add(floatingPositionData, relativePos);
        }
    }
    
    [BurstCompile]
    public partial struct FloatingPositionToWorldPositionUpdateJob : IJobEntity
    {
        public FloatingOriginData FloatingOrigin;
        
        private void Execute(
            in FloatingPositionData floatingPositionData,
            ref LocalTransform localTransform)
        {
            var vectorFromOrigin = FloatingOriginMath.VectorFromFloatingOrigin(
                FloatingOrigin, floatingPositionData);
            
            localTransform.Position = new float3(
                (float)(vectorFromOrigin.x),
                (float)(vectorFromOrigin.y),
                (float)(vectorFromOrigin.z));
        }
    }
}

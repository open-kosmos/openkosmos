using Kosmos.Prototype.OrbitalPhysics;
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
    [UpdateAfter(typeof(OrbitToFloatingPositionUpdateSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [DisableAutoCreation]
    public partial struct DISABLED_FORPHYSICSREF_FloatingPositionToWorldPositionUpdateSystem : ISystem
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

            if (floatingOrigin.ShouldSnap)
            {
                floatingOrigin.ShouldSnap = false;

                var focusEntity = SystemAPI.GetSingletonEntity<FloatingFocusTag>();
                var focusEntityTransform = state.EntityManager.GetComponentData<LocalTransform>(focusEntity);

                // Get focus entity's current world space position
                // TODO: USE FLOATING POSITION RATHER THAN FLOAT WORLD POSITION
                var focusPosition = focusEntityTransform.Position;

                // Reset focus entity's position to origin
                focusEntityTransform.Position -= focusPosition;
                state.EntityManager.SetComponentData(focusEntity, focusEntityTransform);

                // Set all floating positions to current world positions
                new OLD_WorldPositionToFloatingPositionUpdateJob()
                {
                    FloatingOrigin = floatingOrigin
                }.ScheduleParallel();

                // Adjust floating origin
                var scaledFocusPosition = focusPosition * (float)floatingOrigin.Scale;
                floatingOrigin = FloatingOriginMath.Add(floatingOrigin, scaledFocusPosition);

                // Set all world positions to floating positions relative to new origin
                new OLD_FloatingPositionToWorldPositionUpdateJob()
                {
                    FloatingOrigin = floatingOrigin
                }.ScheduleParallel();
                
                SystemAPI.SetSingleton(floatingOrigin);
            }

            // Set all transform scales to floating scales relative to new origin
            // NOTE: This currently happens every frame because the scale can currently change very frame
            new OLD_FloatingScaleToWorldScaleUpdateJob()
            {
                FloatingOrigin = floatingOrigin
            }.ScheduleParallel();
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }
    }

    [BurstCompile]
    public partial struct OLD_WorldPositionToFloatingPositionUpdateJob : IJobEntity
    {
        public FloatingOriginData FloatingOrigin;
        
        private void Execute(
            in LocalTransform localTransform,
            ref FloatingPositionData floatingPositionData)
        {
            // Get world space position currently pointed at by floating position
            var floatingPosWorldSpace = FloatingOriginMath.VectorFromFloatingOrigin(
                FloatingOrigin, floatingPositionData) / FloatingOrigin.Scale;
            
            // Get actual world space position
            var worldSpacePos = localTransform.Position;
            
            // Get vector from floating WS pos to actual WS pos
            var relativePos = (worldSpacePos - floatingPosWorldSpace) * FloatingOrigin.Scale;
            
            // Add this vector to the floating position
            floatingPositionData = FloatingOriginMath.Add(floatingPositionData, relativePos);
        }
    }
    
    [BurstCompile]
    public partial struct OLD_FloatingPositionToWorldPositionUpdateJob : IJobEntity
    {
        public FloatingOriginData FloatingOrigin;
        
        private void Execute(
            in FloatingPositionData floatingPositionData,
            ref LocalTransform localTransform)
        {
            var vectorFromOrigin = (float3)FloatingOriginMath.VectorFromFloatingOrigin(
                FloatingOrigin, floatingPositionData);

            var scale = (float)FloatingOrigin.Scale;

            localTransform.Position = vectorFromOrigin / scale;
        }
    }

    [BurstCompile]
    public partial struct OLD_FloatingScaleToWorldScaleUpdateJob : IJobEntity
    {
        public FloatingOriginData FloatingOrigin;
        
        private void Execute(
            in FloatingScaleData floatingScaleData,
            ref LocalTransform localTransform)
        {
            localTransform.Scale = (float)(floatingScaleData.Value / FloatingOrigin.Scale);
        }
    }
}

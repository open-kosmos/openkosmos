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

            if (floatingOrigin.ShouldSnap)
            {
                floatingOrigin.ShouldSnap = false;

                var focusEntity = SystemAPI.GetSingletonEntity<FloatingFocusTag>();
                var focusEntityFloatingPosition = state.EntityManager.GetComponentData<FloatingPositionData>(focusEntity);
                
                // Adjust floating origin
                floatingOrigin = FloatingOriginMath.ConvertPositionToOrigin(
                    focusEntityFloatingPosition, floatingOrigin.Scale);
                
                SystemAPI.SetSingleton(floatingOrigin);
            }

            // Set all world positions to floating positions relative to new origin
            new FloatingPositionToWorldPositionUpdateJob()
            {
                FloatingOrigin = floatingOrigin
            }.ScheduleParallel();
            
            // Set all transform scales to floating scales relative to new origin
            new FloatingScaleToWorldScaleUpdateJob()
            {
                FloatingOrigin = floatingOrigin
            }.ScheduleParallel();
            
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
    public partial struct FloatingPositionToWorldPositionUpdateJob : IJobEntity
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
    public partial struct FloatingScaleToWorldScaleUpdateJob : IJobEntity
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

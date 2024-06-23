using Kosmos.Prototype.OrbitalPhysics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
            
            var focusEntity = SystemAPI.GetSingletonEntity<FloatingFocusTag>();
            var focusEntityFloatingPosition = state.EntityManager.GetComponentData<FloatingPositionData>(focusEntity);
                
            // Adjust floating origin
            floatingOrigin = FloatingOriginMath.ConvertPositionToOrigin(
                focusEntityFloatingPosition, floatingOrigin.Scale);
                
            SystemAPI.SetSingleton(floatingOrigin);

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

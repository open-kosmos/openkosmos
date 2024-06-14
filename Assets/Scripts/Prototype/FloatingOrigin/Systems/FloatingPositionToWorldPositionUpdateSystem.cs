using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Kosmos.FloatingOrigin
{
    //[DisableAutoCreation]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct FloatingPositionToWorldPositionUpdateSystem : ISystem
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
            new FloatingPositionToWorldPositionUpdateJob()
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
        
        void Execute(
            in FloatingPositionData floatingPositionData,
            ref LocalTransform localTransform)
        {
            var vectorFromOrigin = FloatingOriginMath.VectorFromFloatingOrigin(
                FloatingOrigin, floatingPositionData);
                    
            var floatingScale = (float)(floatingPositionData.Scale / FloatingOrigin.Scale);

            localTransform.Scale = floatingScale;

            localTransform.Position = new float3(
                (float)(vectorFromOrigin.x / FloatingOrigin.Scale),
                (float)(vectorFromOrigin.y / FloatingOrigin.Scale),
                (float)(vectorFromOrigin.z / FloatingOrigin.Scale));
        }
    }
}

using Kosmos.FloatingOrigin;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Kosmos.Prototype.OrbitalPhysics
{
    /// <summary>
    /// System responsible for updating the floating position of orbital bodies based on their orbital elements and
    /// parent body. Bodies are updated from the parent body outwards according to their update order.
    /// </summary>
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct OrbitToFloatingPositionUpdateSystem : ISystem
    {
        private EntityQuery _firstOrderParentQuery;
        private EntityQuery _secondOrderParentQuery;
        private EntityQuery _thirdOrderParentQuery;
        private EntityQuery _fourthOrderParentQuery;
        private EntityQuery _fifthOrderParentQuery;

        private EntityQuery _firstOrderQuery;
        private EntityQuery _secondOrderQuery;
        private EntityQuery _thirdOrderQuery;
        private EntityQuery _fourthOrderQuery;
        private EntityQuery _fifthOrderQuery;
        
        [ReadOnly] private ComponentLookup<FloatingPositionData> _floatingPositionLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FloatingOriginData>();
            
            _floatingPositionLookup = state.GetComponentLookup<FloatingPositionData>(true);

            _firstOrderParentQuery = SystemAPI.QueryBuilder()
                .WithAll<BodyUpdateOrderTag1,
                    BodyParentData,
                    ParentFloatingPositionData>()
                .Build();

            _secondOrderParentQuery = SystemAPI.QueryBuilder()
                .WithAll<BodyUpdateOrderTag2,
                    BodyParentData,
                    ParentFloatingPositionData>()
                .Build();

            _thirdOrderParentQuery = SystemAPI.QueryBuilder()
                .WithAll<BodyUpdateOrderTag3,
                    BodyParentData,
                    ParentFloatingPositionData>()
                .Build();

            _fourthOrderParentQuery = SystemAPI.QueryBuilder()
                .WithAll<BodyUpdateOrderTag4,
                    BodyParentData,
                    ParentFloatingPositionData>()
                .Build();

            _fifthOrderParentQuery = SystemAPI.QueryBuilder()
                .WithAll<BodyUpdateOrderTag5,
                    BodyParentData,
                    ParentFloatingPositionData>()
                .Build();

            _firstOrderQuery = SystemAPI.QueryBuilder()
                .WithAll<BodyUpdateOrderTag1,
                    KeplerElements,
                    MeanAnomaly,
                    Mass,
                    BodyParentData,
                    FloatingPositionData,
                    ParentFloatingPositionData>()
                .Build();

            _secondOrderQuery = SystemAPI.QueryBuilder()
                .WithAll<BodyUpdateOrderTag2,
                    KeplerElements,
                    MeanAnomaly,
                    Mass,
                    BodyParentData,
                    FloatingPositionData,
                    ParentFloatingPositionData>()
                .Build();

            _thirdOrderQuery = SystemAPI.QueryBuilder()
                .WithAll<BodyUpdateOrderTag3,
                    KeplerElements,
                    MeanAnomaly,
                    Mass,
                    BodyParentData,
                    FloatingPositionData,
                    ParentFloatingPositionData>()
                .Build();

            _fourthOrderQuery = SystemAPI.QueryBuilder()
                .WithAll<BodyUpdateOrderTag4,
                    KeplerElements,
                    MeanAnomaly,
                    Mass,
                    BodyParentData,
                    FloatingPositionData,
                    ParentFloatingPositionData>()
                .Build();

            _fifthOrderQuery = SystemAPI.QueryBuilder()
                .WithAll<BodyUpdateOrderTag5,
                    KeplerElements,
                    MeanAnomaly,
                    Mass,
                    BodyParentData,
                    FloatingPositionData,
                    ParentFloatingPositionData>()
                .Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _floatingPositionLookup.Update(ref state);
            
            var floatingOrigin = SystemAPI.GetSingleton<FloatingOriginData>();

            var firstOrderJob = new OrbitToFloatingPositionUpdateJob()
            {
                FloatingOrigin = floatingOrigin
            };

            var secondOrderJob = new OrbitToFloatingPositionUpdateJob()
            {
                FloatingOrigin = floatingOrigin
            };

            var thirdOrderJob = new OrbitToFloatingPositionUpdateJob()
            {
                FloatingOrigin = floatingOrigin
            };

            var fourthOrderJob = new OrbitToFloatingPositionUpdateJob()
            {
                FloatingOrigin = floatingOrigin
            };

            var fifthOrderJob = new OrbitToFloatingPositionUpdateJob()
            {
                FloatingOrigin = floatingOrigin
            };

            new UpdateParentFloatingPositionJob()
            {
                FloatingPositionLookup = _floatingPositionLookup
            }.ScheduleParallel(_firstOrderParentQuery);

            firstOrderJob.ScheduleParallel(_firstOrderQuery);

            new UpdateParentFloatingPositionJob()
            {
                FloatingPositionLookup = _floatingPositionLookup
            }.ScheduleParallel(_secondOrderParentQuery);

            secondOrderJob.ScheduleParallel(_secondOrderQuery);

            new UpdateParentFloatingPositionJob()
            {
                FloatingPositionLookup = _floatingPositionLookup
            }.ScheduleParallel(_thirdOrderParentQuery);

            thirdOrderJob.ScheduleParallel(_thirdOrderQuery);

            new UpdateParentFloatingPositionJob()
            {
                FloatingPositionLookup = _floatingPositionLookup
            }.ScheduleParallel(_fourthOrderParentQuery);

            fourthOrderJob.ScheduleParallel(_fourthOrderQuery);

            new UpdateParentFloatingPositionJob()
            {
                FloatingPositionLookup = _floatingPositionLookup
            }.ScheduleParallel(_fifthOrderParentQuery);

            fifthOrderJob.ScheduleParallel(_fifthOrderQuery);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
    
    [BurstCompile]
    public partial struct OrbitToFloatingPositionUpdateJob : IJobEntity
    {
        public FloatingOriginData FloatingOrigin;
        public void Execute(
            ref FloatingPositionData floatingPosition,
            in KeplerElements keplerElements,
            in MeanAnomaly meanAnomaly,
            in BodyParentData parentData,
            in ParentFloatingPositionData parentFloatingPositionData)
        {
            var positionInOrbit = Kosmos.ThirdParty.Math.OrbitMath.RelativePositionFromKeplerElements(
                keplerElements.SemiMajorAxisMeters,
                keplerElements.Eccentricity,
                keplerElements.EclipticInclinationRadians,
                keplerElements.LongitudeOfAscendingNodeRadians,
                keplerElements.ArgumentOfPeriapsisRadians,
                meanAnomaly.MeanAnomalyRadians,
                parentData.ParentMassKg,
                out var velocity);

            floatingPosition = FloatingOriginMath.Add(parentFloatingPositionData, positionInOrbit);
        }
    }
    
    [BurstCompile]
    public partial struct UpdateParentFloatingPositionJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<FloatingPositionData> FloatingPositionLookup;

        private void Execute(
            ref ParentFloatingPositionData parentFloatingPositionData,
            in BodyParentData parentData)
        {
            var parentPosition = FloatingPositionLookup[parentData.ParentEntity];
            parentFloatingPositionData.Local = parentPosition.Local;
            parentFloatingPositionData.GlobalX = parentPosition.GlobalX;
            parentFloatingPositionData.GlobalY = parentPosition.GlobalY;
            parentFloatingPositionData.GlobalZ = parentPosition.GlobalZ;
        }
    }
}

using Kosmos.Time;
using Unity.Burst;
using Unity.Entities;

namespace Kosmos.Prototype.OrbitalPhysics
{
    /// <summary>
    /// System responsible for updating the mean anomaly of all orbital bodies based on their orbital elements.
    /// </summary>
    [UpdateBefore(typeof(OrbitToFloatingPositionUpdateSystem))]
    public partial struct OrbitEvolutionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UniversalTime>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var universalTime = SystemAPI.GetSingleton<UniversalTime>().Value;
            new OrbitEvolutionJob() { UniversalTime = universalTime }.ScheduleParallel();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }

    [BurstCompile]
    public partial struct OrbitEvolutionJob : IJobEntity
    {
        public double UniversalTime;

        private void Execute(
            in KeplerElements keplerElements,
            ref MeanAnomaly meanAnomaly
        )
        {
            meanAnomaly.MeanAnomalyRadians = Kosmos.Math.OrbitMath.ComputeMeanAnomalyAtTime(
                meanAnomaly.MeanAnomalyAtEpoch,
                keplerElements.OrbitalPeriodInSeconds,
                UniversalTime
                );
        }
    }
}

using Unity.Entities;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public struct MeanAnomaly : IComponentData
    {
        public double MeanAnomalyRadians;
        public double MeanAnomalyAtEpoch;
    }
}
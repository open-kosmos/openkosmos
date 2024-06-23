using Unity.Entities;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public struct BodyRadius : IComponentData
    {
        public double EquatorialRadiusMeters;
        public double PolarRadiusMeters;
    }
}
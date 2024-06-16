using Unity.Entities;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public struct KeplerElements : IComponentData
    {
        public double SemiMajorAxisMeters;
        public double Eccentricity;
        public double EclipticInclinationRadians;
        public double LongitudeOfAscendingNodeRadians;
        public double ArgumentOfPeriapsisRadians;
        public double OrbitalPeriodInSeconds;
    }
}
using Unity.Mathematics;

namespace Kosmos.Math
{
    /// <summary>
    /// Static methods for orbital physics calculations.
    /// </summary>
    public static partial class OrbitMath
    {
        public static readonly double GRAV_CONSTANT = 0.0000000000667408;
        public static readonly double STD_GRAV_PARAM_EARTH = 398600441800000;

        public static readonly float ONE_EARTH_MASS_KG = 5972000000000000000000000f;
        public static readonly float ONE_EARTH_RADIUS_KM = 6371f;
        
        public static double ComputeOrbitalPeriodSeconds(double semimajorAxisMeters, double parentBodyMass)
        {
            return math.sqrt((4.0 * math.pow(math.PI, 2.0) * math.pow(semimajorAxisMeters, 3.0)) /
                             (STD_GRAV_PARAM_EARTH * (parentBodyMass / ONE_EARTH_MASS_KG)));
        }
        
        public static double ComputeMeanAnomalyAtTime(
            double meanAnomalyAtEpoch,
            double orbitalPeriodSeconds,
            double currentUniversalTime
        )
        {
            var meanAnomaly = (meanAnomalyAtEpoch + (KMath.TWO_PI * (currentUniversalTime / orbitalPeriodSeconds))) % KMath.TWO_PI;
                    
            if (meanAnomaly < 0)
            {
                meanAnomaly += KMath.TWO_PI;
            }

            return meanAnomaly;
        }
    }
}

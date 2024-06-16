using Kosmos.Math;
using Unity.Collections;
using Unity.Mathematics;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public static class OrbitMath
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
        
        public static double3 RelativePositionFromKeplerElements(
                double a,
                double e,
                double i,
                double W,
                double w,
                double MA,
                double parentMassKg,
                out double3 v0)
            {
                double b, n, xper, yper, xdotper, ydotper;
                
                var R = new NativeArray<NativeArray<double>>(3, Allocator.Temp);

                for (int j = 0; j < R.Length; j++)
                {
                    R[j] = new NativeArray<double>(3, Allocator.Temp);
                }
                
                double cosomg, cosomp, sinomg, sinomp, cosi, sini;

                var mu = parentMassKg * GRAV_CONSTANT;
                
                //Compute eccentric anomaly
                double delta = math.pow(10, -10);

                double EA; //Eccentric Anomaly
                int maxIter = 100;

                if (e < 1)
                {
                    if (e < 0.8f) EA = MA;
                    else EA = math.PI;

                    double F = EA - e * math.sin(MA) - MA;

                    int idx = 0;

                    while ((math.abs(F) > delta) && (idx < maxIter))
                    {
                        EA -= F / (1f - e * math.cos(EA));
                        F = EA - e * math.sin(EA) - MA;

                        idx++;
                    }
                }
                else
                {
                    EA = MA/e;  // Initial guess // 
                    int iteration = 0;

                    while (iteration < maxIter)
                    {
                        double f = e * math.sinh(EA) - EA - MA; // Function value
                        double fPrime = e * math.cosh(EA) - 1; // Derivative of the function

                        var deltaH = f / fPrime;
                        EA -= deltaH;
                        
                        // Check for convergence
                        if (math.abs(deltaH) < delta)
                            break;
                        
                        iteration++;
                    }
                }

                // semi-major axis is assumed to be positive here we apply the convention of having it negative as for
                // computations to result in higher elegance
                if (e >= 1.0)
                {
                    a = -a;
                }

                // 1 - We start by evaluating position and velocity in the perifocal reference system
                if (e < 1.0) // EA is the eccentric anomaly
                {
                    b = a * math.sqrt(1 - e * e);
                    n = math.sqrt(mu / (a * a * a));

                    xper = a * (math.cos(EA) - e);
                    yper = b * math.sin(EA);
                    xdotper = -(a * n * math.sin(EA)) / (1 - e * math.cos(EA));
                    ydotper = (b * n * math.cos(EA)) / (1 - e * math.cos(EA));
                } else // EA is Hyperbolic anomaly
                {
                    b = a * math.sqrt(e * e - 1); // Semi-minor axis
                    n = math.sqrt(mu / (a * a * a)); // Mean motion

                    xper = a * (e - math.cosh(EA));
                    yper = b * math.sinh(EA);
                    
                    xdotper = -(a * n * math.sinh(EA)) / (e * math.cosh(EA) - 1);
                    ydotper = (b * n * math.cosh(EA)) / (e * math.cosh(EA) - 1);
                }

                // 2 - We then built the rotation matrix from perifocal reference frame to inertial

                cosomg = math.cos(W);
                cosomp = math.cos(w);
                sinomg = math.sin(W);
                sinomp = math.sin(w);
                cosi = math.cos(i);
                sini = math.sin(i);

                var R0 = R[0];
                var R1 = R[1];
                var R2 = R[2];

                R0[0] = cosomg * cosomp - sinomg * sinomp * cosi;
                R0[1] = -cosomg * sinomp - sinomg * cosomp * cosi;
                R0[2] = sinomg * sini;
                R1[0] = sinomg * cosomp + cosomg * sinomp * cosi;
                R1[1] = -sinomg * sinomp + cosomg * cosomp * cosi;
                R1[2] = -cosomg * sini;
                R2[0] = sinomp * sini;
                R2[1] = cosomp * sini;
                R2[2] = cosi;

                // 3 - We end by transforming according to this rotation matrix

                var temp = new NativeArray<double>(3, Allocator.Temp);
                temp[0] = xper;
                temp[1] = yper;
                temp[2] = 0.0;

                var temp2 = new NativeArray<double>(3, Allocator.Temp);
                temp2[0] = xdotper;
                temp2[1] = ydotper;
                temp2[2] = 0.0;

                var r0 = new double3();
                v0 = new double3();
                
                for (int j = 0; j < 3; j++) {
                    r0[j] = 0.0;
                    v0[j] = 0.0;
                    for (int k = 0; k < 3; k++) {
                        r0[j] += R[j][k] * temp[k];
                        v0[j] += R[j][k] * temp2[k];
                    }
                }

                var r0Y = r0[1];
                r0[1] = r0[2];
                r0[2] = r0Y;
                
                var v0Y = v0[1];
                v0[1] = v0[2];
                v0[2] = v0Y;

                for (int idx2 = 0; idx2 < R.Length; idx2++)
                {
                    R[idx2].Dispose();
                }

                R.Dispose();
                
                temp.Dispose();
                temp2.Dispose();
                
                return r0;
            }
    }
}
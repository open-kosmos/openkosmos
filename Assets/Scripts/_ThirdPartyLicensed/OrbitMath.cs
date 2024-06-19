using Unity.Mathematics;

namespace Kosmos.ThirdParty.Math
{
    public static class OrbitMath
    {
        private static readonly double GRAV_CONSTANT = 0.0000000000667408;
        
        
        /// <summary>
        /// Calculates the position of a body relative to its parent body given its Keplerian orbital elements.
        /// </summary>
        /// <license>
        /// This method is licensed under the <a href="https://github.com/esa/pykep/blob/master/LICENSE.txt">GNU General Public License v3.0</a>.
        /// </license>
        /// <remarks>
        /// Based heavily on work from the European Space Agency found <a href="https://github.com/esa/pykep/blob/403a7dfe8ed3ff19b43bcbd6e6856de7f820cf55/include/keplerian_toolbox/core_functions/par2ic.hpp">here</a>.
        /// </remarks>
        public static double3 RelativePositionFromKeplerElements(
                double semiMajorAxis,
                double eccentricity,
                double inclination,
                double longitudeAscendingNode,
                double argumentPeriapsis,
                double meanAnomaly,
                double parentMassKg,
                out double3 velocity)
            {
                double b, 
                    meanMotion, 
                    perifocalX, 
                    perifocalY, 
                    perifocalVelocityX, 
                    perifocalVelocityY, 
                    longAscNodeCos, 
                    argPeriapsisCos, 
                    longAscNodeSin, 
                    argPeriapsisSin, 
                    inclinationCos, 
                    inclinationSin;

                var rotationMatrix = new double3x3();
                
                var mu = parentMassKg * GRAV_CONSTANT;
                
                // Compute eccentric anomaly using Newton-Raphson method
                // https://en.wikipedia.org/wiki/Newton%27s_method
                double tolerance = math.pow(10, -10);

                double eccentricAnomaly;
                int maxIterations = 100;

                if (eccentricity < 1)
                {
                    eccentricAnomaly = eccentricity < 0.8f ? meanAnomaly : math.PI;

                    var error = eccentricAnomaly - eccentricity * math.sin(meanAnomaly) - meanAnomaly;

                    var idx = 0;

                    while ((math.abs(error) > tolerance) && (idx < maxIterations))
                    {
                        eccentricAnomaly -= error / (1f - eccentricity * math.cos(eccentricAnomaly));
                        error = eccentricAnomaly - eccentricity * math.sin(eccentricAnomaly) - meanAnomaly;

                        idx++;
                    }
                }
                else
                {
                    eccentricAnomaly = meanAnomaly/eccentricity;  // Initial guess
                    int iteration = 0;

                    while (iteration < maxIterations)
                    {
                        var f = eccentricity * math.sinh(eccentricAnomaly) - eccentricAnomaly - meanAnomaly; // Function value
                        var fPrime = eccentricity * math.cosh(eccentricAnomaly) - 1; // Derivative of the function

                        var deltaH = f / fPrime;
                        eccentricAnomaly -= deltaH;
                        
                        // Check for convergence
                        if (math.abs(deltaH) < tolerance)
                            break;
                        
                        iteration++;
                    }
                }

                if (eccentricity >= 1.0)
                {
                    semiMajorAxis = -semiMajorAxis;
                }

                // Evaluate position and velocity in the perifocal reference frame
                // https://orbital-mechanics.space/classical-orbital-elements/perifocal-frame.html
                if (eccentricity < 1.0)
                {
                    b = semiMajorAxis * math.sqrt(1 - eccentricity * eccentricity);
                    meanMotion = math.sqrt(mu / (semiMajorAxis * semiMajorAxis * semiMajorAxis));

                    perifocalX = semiMajorAxis * (math.cos(eccentricAnomaly) - eccentricity);
                    perifocalY = b * math.sin(eccentricAnomaly);
                    perifocalVelocityX = -(semiMajorAxis * meanMotion * math.sin(eccentricAnomaly)) / (1 - eccentricity * math.cos(eccentricAnomaly));
                    perifocalVelocityY = (b * meanMotion * math.cos(eccentricAnomaly)) / (1 - eccentricity * math.cos(eccentricAnomaly));
                } else
                {
                    // Eccentric Anomaly becomes Hyperbolic Anomaly
                    b = semiMajorAxis * math.sqrt(eccentricity * eccentricity - 1); // Semi-minor axis
                    meanMotion = math.sqrt(mu / (semiMajorAxis * semiMajorAxis * semiMajorAxis)); // Mean motion

                    perifocalX = semiMajorAxis * (eccentricity - math.cosh(eccentricAnomaly));
                    perifocalY = b * math.sinh(eccentricAnomaly);
                    
                    perifocalVelocityX = -(semiMajorAxis * meanMotion * math.sinh(eccentricAnomaly)) / (eccentricity * math.cosh(eccentricAnomaly) - 1);
                    perifocalVelocityY = (b * meanMotion * math.cosh(eccentricAnomaly)) / (eccentricity * math.cosh(eccentricAnomaly) - 1);
                }

                // Build the rotation matrix to transform from perifocal to inertial frame
                longAscNodeCos = math.cos(longitudeAscendingNode);
                longAscNodeSin = math.sin(longitudeAscendingNode);
                argPeriapsisCos = math.cos(argumentPeriapsis);
                argPeriapsisSin = math.sin(argumentPeriapsis);
                inclinationCos = math.cos(inclination);
                inclinationSin = math.sin(inclination);
                
                rotationMatrix[0][0] = longAscNodeCos * argPeriapsisCos - longAscNodeSin * argPeriapsisSin * inclinationCos;
                rotationMatrix[0][1] = longAscNodeSin * inclinationSin;
                rotationMatrix[0][2] = -longAscNodeCos * argPeriapsisSin - longAscNodeSin * argPeriapsisCos * inclinationCos;

                rotationMatrix[1][0] = argPeriapsisSin * inclinationSin;
                rotationMatrix[1][1] = inclinationCos;
                rotationMatrix[1][2] = argPeriapsisCos * inclinationSin;
                
                rotationMatrix[2][0] = longAscNodeSin * argPeriapsisCos + longAscNodeCos * argPeriapsisSin * inclinationCos;
                rotationMatrix[2][1] = -longAscNodeCos * inclinationSin;
                rotationMatrix[2][2] = -longAscNodeSin * argPeriapsisSin + longAscNodeCos * argPeriapsisCos * inclinationCos;

                // Apply the rotation matrix to the position and velocity vectors
                var tempPos = new double3(perifocalX, 0.0, perifocalY);
                var tempVel = new double3(perifocalVelocityX, 0.0, perifocalVelocityY);

                var position = new double3();
                velocity = new double3();
                
                for (var j = 0; j < 3; j++) {
                    position[j] = 0.0;
                    velocity[j] = 0.0;
                    for (var k = 0; k < 3; k++) {
                        position[j] += rotationMatrix[j][k] * tempPos[k];
                        velocity[j] += rotationMatrix[j][k] * tempVel[k];
                    }
                }
                
                return position;
            }
    }
}

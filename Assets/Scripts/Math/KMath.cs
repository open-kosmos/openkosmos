using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Kosmos.Math
{
    public static class KMath
    {
        public static readonly double PI = 3.14159265358979323846;
        public static readonly float PI_DIV_TWO = 1.57079632679f;
        public static readonly float THREE_PI_DIV_TWO = 4.71238898038f;
        public static readonly double PI_DIV_FOUR = 0.785398163397448309616;
        public static readonly double TWO_PI = 6.28318530717958647692;

        public static readonly double LN_TWO = 0.693147180559945309417232121458176568075500134360255254120680009;
        public static readonly double LOG_MAX_VALUE = 709.782712893384;

        public static double MapValue(double value, double originalStart, double originalEnd, double newStart,
            double newEnd)
        {
            double scale = (newEnd - newStart) / (originalEnd - originalStart);
            return newStart + ((value - originalStart) * scale);
        }

        public static float3 GetSphericalDirectionVector(float pitch, float yaw)
        {
            float x = math.cos(yaw) * math.cos(pitch);
            float y = math.sin(pitch);
            float z = math.sin(yaw) * math.cos(pitch);

            return new float3(x, y, z);
        }

        public static double Norm(ref double3 v1)
        {
            return math.sqrt(v1[0] * v1[0] + v1[1] * v1[1] + v1[2] * v1[2]);
        }

        public static void Vers(ref double3 output, ref double3 input)
        {
            double c = Norm(ref input);
            for (int i = 0; i < 3; ++i)
            {
                output[i] = input[i] / c;
            }
        }

        public static void Cross(ref double3 output, ref double3 v1, ref double3 v2)
        {
            output.x = v1[1] * v2[2] - v1[2] * v2[1];
            output.y = v1[2] * v2[0] - v1[0] * v2[2];
            output.z = v1[0] * v2[1] - v1[1] * v2[0];
        }

        public static double Atanh(double x)
        {
            return 0.5 * math.log((1 + x) / (1 - x));
        }

        public static double Acosh(double x)
        {
            return math.log(x + math.sqrt(x * x - 1));
        }

        public static double Asinh(double x)
        {
            return math.log(x + math.sqrt(x * x + 1));
        }

        public static double SqrMagnitude(ref double3 input)
        {
            return input[0] * input[0] + input[1] * input[1] + input[2] * input[2];
        }
    }
}
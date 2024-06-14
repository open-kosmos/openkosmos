using Kosmos.FloatingOrigin;
using Unity.Mathematics;

namespace Kosmos.FloatingOrigin
{
    public static class FloatingOriginMath
    {
        public static readonly double CELL_SIZE = 10.0;
            
        public static double3 VectorFromFloatingOrigin(FloatingOriginData a, FloatingPositionData b)
        {
            var x = (b.GlobalX - a.GlobalX) * CELL_SIZE + b.LocalX - a.LocalX;
            var y = (b.GlobalY - a.GlobalY) * CELL_SIZE + b.LocalY - a.LocalY;
            var z = (b.GlobalZ - a.GlobalZ) * CELL_SIZE + b.LocalZ - a.LocalZ;

            return new double3(x, y, z);
        }
            
        public static double3 VectorFromPosition(FloatingPositionData from, FloatingPositionData to)
        {
            var x = (to.GlobalX - from.GlobalX) * CELL_SIZE + to.LocalX - from.LocalX;
            var y = (to.GlobalY - from.GlobalY) * CELL_SIZE + to.LocalY - from.LocalY;
            var z = (to.GlobalZ - from.GlobalZ) * CELL_SIZE + to.LocalZ - from.LocalZ;

            return new double3(x, y, z);
        }

        public static double3 VectorFromRelativePosition(double3 from, double3 to)
        {
            var x = to.x - from.x;
            var y = to.y - from.y;
            var z = to.z - from.z;

            return new double3(x, y, z);
        }
        
        public static double3 WorldSpaceFromPosition(FloatingOriginData floatingOrigin, FloatingPositionData floatingPosition)
        {
            var vectorFromOrigin = VectorFromFloatingOrigin(floatingOrigin, floatingPosition);
            return new double3(
                vectorFromOrigin.x / floatingOrigin.Scale,
                vectorFromOrigin.y / floatingOrigin.Scale,
                vectorFromOrigin.z / floatingOrigin.Scale);
        }
        
        public static FloatingPositionData PositionDataFromCurrentWorldSpace(
            FloatingOriginData floatingOriginData, 
            float3 worldSpacePosition, 
            float scale)
        {
            var originPos = new double3(floatingOriginData.LocalX, floatingOriginData.LocalY, floatingOriginData.LocalZ);
            var pos = originPos 
                      + new double3(worldSpacePosition.x, worldSpacePosition.y, worldSpacePosition.z) 
                      * floatingOriginData.Scale;

            return new FloatingPositionData()
            {
                LocalX = pos.x,
                LocalY = pos.y,
                LocalZ = pos.z,
                Scale = scale
            };
        }
    }
}
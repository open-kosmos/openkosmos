using Kosmos.FloatingOrigin;
using Unity.Mathematics;

namespace Kosmos.FloatingOrigin
{
    /// <summary>
    /// Static math functions for floating origin calculations.
    /// </summary>
    public static class FloatingOriginMath
    {
        public static readonly double CELL_SIZE = 5000000.0;
            
        public static double3 VectorFromFloatingOrigin(FloatingOriginData a, FloatingPositionData b)
        {
            var x = (b.GlobalX - a.GlobalX) * CELL_SIZE + b.Local.x - a.Local.x;
            var y = (b.GlobalY - a.GlobalY) * CELL_SIZE + b.Local.y - a.Local.y;
            var z = (b.GlobalZ - a.GlobalZ) * CELL_SIZE + b.Local.z - a.Local.z;

            return new double3(x, y, z);
        }
            
        public static double3 VectorFromPosition(FloatingPositionData from, FloatingPositionData to)
        {
            var x = (to.GlobalX - from.GlobalX) * CELL_SIZE + to.Local.x - from.Local.x;
            var y = (to.GlobalY - from.GlobalZ) * CELL_SIZE + to.Local.y - from.Local.y;
            var z = (to.GlobalZ - from.GlobalZ) * CELL_SIZE + to.Local.z - from.Local.z;

            return new double3(x, y, z);
        }

        /// <summary>
        /// Calculates the floating position of a given world space position relative to a NON-FLOATING origin.
        /// </summary>
        public static FloatingPositionData InitializeLocal(double3 originalLocal, float scale)
        {
            long globalX = 0;
            long globalY = 0;
            long globalZ = 0;
            
            var local = new double3();
            
            // Bounds check
            local.x = originalLocal.x % CELL_SIZE;
            globalX = (long) (originalLocal.x / CELL_SIZE);
            
            local.y = originalLocal.y % CELL_SIZE;
            globalY = (long) (originalLocal.y / CELL_SIZE);
            
            local.z = originalLocal.z % CELL_SIZE;
            globalZ = (long) (originalLocal.z / CELL_SIZE);
            
            return new FloatingPositionData()
            {
                Local = local,
                GlobalX = globalX,
                GlobalY = globalY,
                GlobalZ = globalZ,
                Scale = scale
            };
        }
        
        public static FloatingPositionData Add(FloatingPositionData a, double3 b)
        {
            var newLocal = a.Local + b;
            var local = new double3();
            
            // Bounds check
            var globalX = a.GlobalX;
            var globalY = a.GlobalY;
            var globalZ = a.GlobalZ;
            
            local.x = newLocal.x % CELL_SIZE;
            globalX += (long) (newLocal.x / CELL_SIZE);
            
            local.y = newLocal.y % CELL_SIZE;
            globalY += (long) (newLocal.y / CELL_SIZE);
            
            local.z = newLocal.z % CELL_SIZE;
            globalZ += (long) (newLocal.z / CELL_SIZE);
            
            return new FloatingPositionData()
            {
                Local = local,
                GlobalX = globalX,
                GlobalY = globalY,
                GlobalZ = globalZ,
                Scale = a.Scale
            };
        }
        
        public static FloatingOriginData Add(FloatingOriginData a, double3 b)
        {
            var local = a.Local + b;
            
            // Bounds check
            var globalX = a.GlobalX;
            var globalY = a.GlobalY;
            var globalZ = a.GlobalZ;
            
            if (local.x >= CELL_SIZE)
            {
                local.x -= CELL_SIZE;
                globalX++;
            }
            else if (local.x < 0)
            {
                local.x += CELL_SIZE;
                globalX--;
            }
            
            if (local.y >= CELL_SIZE)
            {
                local.y -= CELL_SIZE;
                globalY++;
            }
            else if (local.y < 0)
            {
                local.y += CELL_SIZE;
                globalY--;
            }
            
            if (local.z >= CELL_SIZE)
            {
                local.z -= CELL_SIZE;
                globalZ++;
            }
            else if (local.z < 0)
            {
                local.z += CELL_SIZE;
                globalZ--;
            }
            
            return new FloatingOriginData()
            {
                Local = local,
                GlobalX = globalX,
                GlobalY = globalY,
                GlobalZ = globalZ,
                Scale = a.Scale
            };
        }

        
        public static FloatingPositionData PositionDataFromCurrentWorldSpace(
            FloatingOriginData floatingOriginData, 
            float3 worldSpacePosition, 
            float scale)
        {
            var originPos = floatingOriginData.Local;
            var pos = originPos 
                      + new double3(worldSpacePosition.x, worldSpacePosition.y, worldSpacePosition.z) 
                      * floatingOriginData.Scale;

            return new FloatingPositionData()
            {
                Local = pos,
                Scale = scale
            };
        }
    }
}
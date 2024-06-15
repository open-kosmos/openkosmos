using Unity.Entities;
using Unity.Mathematics;

namespace Kosmos.FloatingOrigin
{
    public struct FloatingPositionData : IComponentData
    {
        public double3 Local;
        public long GlobalX;
        public long GlobalY;
        public long GlobalZ;
        public float Scale;
    }
}

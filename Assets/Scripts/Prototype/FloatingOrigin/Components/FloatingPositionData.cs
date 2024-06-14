using Unity.Entities;
using Unity.Mathematics;

namespace Kosmos.FloatingOrigin
{
    public struct FloatingPositionData : IComponentData
    {
        public double LocalX;
        public double LocalY;
        public double LocalZ;
        public long GlobalX;
        public long GlobalY;
        public long GlobalZ;
        public float Scale;
    }
}

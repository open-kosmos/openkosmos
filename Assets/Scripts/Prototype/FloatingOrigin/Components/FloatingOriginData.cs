using Unity.Entities;
using Unity.Mathematics;

namespace Kosmos.FloatingOrigin
{
    public struct FloatingOriginData : IComponentData
    {
        public double3 Local;
        public long GlobalX;
        public long GlobalY;
        public long GlobalZ;
        public double Scale;

        public bool ShouldSnap;
    }
}

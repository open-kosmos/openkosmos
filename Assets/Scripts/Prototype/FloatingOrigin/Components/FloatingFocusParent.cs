using Unity.Entities;
using Unity.Mathematics;

namespace Kosmos.FloatingOrigin
{
    public struct FloatingFocusParent : IComponentData
    {
        public Entity ParentEntity;
        public double3 Offset;
    }
}
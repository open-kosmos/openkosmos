using Unity.Collections;
using Unity.Entities;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public struct BodyId : IComponentData
    {
        public FixedString64Bytes Value;
    }
}
using Unity.Entities;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public struct OrbitalBodyUpdateOrder : IComponentData
    {
        public int UpdateOrder;
    }
}
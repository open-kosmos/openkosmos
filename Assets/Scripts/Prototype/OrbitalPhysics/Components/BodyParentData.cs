using Unity.Entities;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public struct BodyParentData : IComponentData
    {
        public double ParentMassKg;
        public Entity ParentEntity;
    }
}
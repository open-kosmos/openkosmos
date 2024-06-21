using Unity.Entities;

namespace Prototype.FloatingOrigin.Components
{
    public struct SineMover : IComponentData
    {
        public float Speed;
        public float Amplitude;
    }
}
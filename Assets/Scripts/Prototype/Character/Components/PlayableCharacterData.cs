using Unity.Entities;
using Unity.Mathematics;

namespace Kosmos.Prototype.Character
{
    public struct PlayableCharacterData : IComponentData
    {
        public float MoveSpeed;
        public float RotationSpeed;
    }
    
    public struct TargetRotation : IComponentData
    {
        public quaternion Value;
    }
}
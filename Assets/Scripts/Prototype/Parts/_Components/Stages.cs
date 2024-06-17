using Unity.Collections;
using Unity.Entities;

namespace Kosmos.Prototype.Parts.Components
{
    public struct Stage : IBufferElementData
    {
        public NativeArray<StagePart> Parts;
    }
    
    public struct StagePart
    {
        public Entity Value;
    }
}
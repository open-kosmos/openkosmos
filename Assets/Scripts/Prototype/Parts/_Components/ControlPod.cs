using Unity.Entities;

namespace Kosmos.Prototype.Parts.Components
{
    public struct ControlPod : IComponentData
    {
        public int CurrentStageIndex;
    }
}
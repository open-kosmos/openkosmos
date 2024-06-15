using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototype.Parts
{
    public class StageablePart : PartBase
    {
        [Tweakable]
        [SerializeField]
        protected int StageIndex;

        private class StageablePartBaker : Baker<StageablePart>
        {
            public override void Bake(StageablePart authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new StageablePartData()
                {
                    StageIndex = authoring.StageIndex
                });
            }
        }
    }
}

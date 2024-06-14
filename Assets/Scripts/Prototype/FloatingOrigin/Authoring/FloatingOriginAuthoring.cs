using Kosmos.FloatingOrigin;
using Unity.Entities;
using UnityEngine;

namespace Prototype.FloatingOrigin.Components
{
    public class FloatingOriginAuthoring : MonoBehaviour
    {
        private class FloatingOriginAuthoringBaker : Baker<FloatingOriginAuthoring>
        {
            public override void Bake(FloatingOriginAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FloatingOriginData()
                {
                    Scale = 1f
                });
            }
        }
    }
}
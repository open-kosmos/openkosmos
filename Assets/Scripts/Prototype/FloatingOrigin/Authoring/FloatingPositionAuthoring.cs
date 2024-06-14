using Kosmos.FloatingOrigin;
using Unity.Entities;
using UnityEngine;

namespace Prototype.FloatingOrigin.Components
{
    public class FloatingPositionAuthoring : MonoBehaviour
    {
        private class FloatingPositionAuthoringBaker : Baker<FloatingPositionAuthoring>
        {
            public override void Bake(FloatingPositionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var floatingOrigin = new FloatingOriginData()
                {
                    Scale = 1f
                };
                var floatingPosition = FloatingOriginMath.PositionDataFromCurrentWorldSpace(
                    floatingOrigin,
                    authoring.transform.position,
                    authoring.transform.localScale.x);
                AddComponent(entity, floatingPosition);
            }
        }
    }
}
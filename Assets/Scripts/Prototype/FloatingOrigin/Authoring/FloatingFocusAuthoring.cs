using Kosmos.FloatingOrigin;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Prototype.FloatingOrigin.Components
{
    public class FloatingFocusAuthoring : MonoBehaviour
    {
        private class FloatingFocusAuthoringBaker : Baker<FloatingFocusAuthoring>
        {
            public override void Bake(FloatingFocusAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var focusData = new FloatingFocusTag();
                AddComponent(entity, focusData);
                var floatingPosition = FloatingOriginMath.InitializeLocal(
                    (float3)authoring.transform.position);
                AddComponent(entity, floatingPosition);
            }
        }
    }
}
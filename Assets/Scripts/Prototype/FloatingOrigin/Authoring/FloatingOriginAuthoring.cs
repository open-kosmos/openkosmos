using Kosmos.FloatingOrigin;
using Unity.Entities;
using UnityEngine;

namespace Prototype.FloatingOrigin.Components
{
    public class FloatingOriginAuthoring : MonoBehaviour
    {
        [SerializeField] private double _initialScale = 1.0;
        
        private class FloatingOriginAuthoringBaker : Baker<FloatingOriginAuthoring>
        {
            public override void Bake(FloatingOriginAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FloatingOriginData()
                {
                    Scale = authoring._initialScale
                });
            }
        }
    }
}
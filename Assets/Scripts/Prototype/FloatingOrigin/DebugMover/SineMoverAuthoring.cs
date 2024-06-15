using Kosmos.FloatingOrigin;
using Unity.Entities;
using UnityEngine;

namespace Prototype.FloatingOrigin.Components
{
    [UpdateBefore(typeof(FloatingOriginSnapCheckSystem))]
    public class SineMoverAuthoring : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _amplitude = 1f;
        
        private class SineMoverAuthoringBaker : Baker<SineMoverAuthoring>
        {
            public override void Bake(SineMoverAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SineMover()
                {
                    Speed = authoring._speed,
                    Amplitude = authoring._amplitude
                });
            }
        }
    }
}
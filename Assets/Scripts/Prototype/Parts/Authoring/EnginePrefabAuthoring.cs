using System.Collections.Generic;
using Kosmos.Prototype.Parts;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public struct EnginePrefabEntry : IBufferElementData
    {
        public FixedString64Bytes PartId;
        public Entity PrefabEntity;
    }

    public class EnginePrefabAuthoring : MonoBehaviour
    {
        public List<RocketPart> EnginePrefabs;

        private class EntityPrefabAuthoringBaker : Baker<EnginePrefabAuthoring>
        {
            public override void Bake(EnginePrefabAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                var buffer = AddBuffer<EnginePrefabEntry>(entity);

                foreach (var prefab in authoring.EnginePrefabs)
                {
                    var id = prefab.PartId;
                    var entityPrefab = GetEntity(prefab, TransformUsageFlags.Dynamic);
                    buffer.Add(new EnginePrefabEntry()
                    {
                        PartId = id,
                        PrefabEntity = entityPrefab
                    });
                }
            }
        }
    }
}

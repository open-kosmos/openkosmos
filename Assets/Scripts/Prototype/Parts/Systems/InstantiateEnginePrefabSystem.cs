using Kosmos.Prototype.OrbitalPhysics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kosmos.Prototype.Prefabs
{
    public partial class InstantiateEnginePrefabSystem : SystemBase
    {
        private const string PREFAB_ID = "Engine_M";

        protected override void OnUpdate()
        {
            var spacebarInput = Input.GetKeyDown(KeyCode.Space);

            if (!spacebarInput)
            {
                return;
            }

            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            var prefabId = new FixedString64Bytes(PREFAB_ID);

            Entities
                .ForEach((ref DynamicBuffer<EnginePrefabEntry> enginePrefabs) =>
                {
                    foreach (var prefab in enginePrefabs)
                    {
                        if (prefab.PartId.Equals(prefabId))
                        {
                            var entity = ecb.Instantiate(prefab.PrefabEntity);

                            // Set transform to random range
                            var pos = new float3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
                            ecb.SetComponent(entity, new LocalTransform()
                            {
                                Position = pos,
                                Scale = 1f
                            });

                            ecb.SetName(entity, PREFAB_ID);
                        }
                    }
                })
                .Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}
using GLTFast;
using Kosmos.Prototype.Parts;
using Kosmos.Prototype.Parts.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;
using System.IO;
using UnityEngine;
using GLTFast.Schema;
using Mesh = UnityEngine.Mesh;
using Material = UnityEngine.Material;

namespace Assets.Scripts.Prototype.VAB
{
    public static class PartsToEcsManager
    {

        private static Dictionary<int, List<Entity>> _stages = new();

        private static bool _controllComponentAdded = false;
        private static Entity _playerControlledControlPod;

        public static async Task ConstructPlayableVehicle(PartCollection vehicleRoot)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var parts = vehicleRoot.AllParts;


            foreach (var part in parts)
            {
                var entity = entityManager.CreateEntity();
                SetStage(part, entity);
                AddComponentsToEntity(part, entity, entityManager);
                var modelPath = Path.Combine(Application.streamingAssetsPath, "Parts", $"{part.PartId}.glb");
                await CreateMeshForEntity(entity, entityManager, modelPath, part);
            }

            AddStageBuffer(parts, entityManager);
        }

        private static void SetStage(PartBase part, Entity entity)
        {

            if (part is StageablePart)
            {
                var stageIndex = (part as StageablePart).GetStageIndex();
                if (!_stages.ContainsKey(stageIndex))
                {
                    _stages.Add(stageIndex, new());
                }

                _stages[stageIndex].Add(entity);
            }
        }

        private static void AddStageBuffer(IReadOnlyCollection<PartBase> parts, EntityManager entityManager)
        {
            if (_playerControlledControlPod != Entity.Null)
            {
                var stagesBuffer = entityManager.AddBuffer<Stage>(_playerControlledControlPod);
                foreach (var stageParts in _stages.OrderBy(s => s.Key))
                {
                    NativeArray<StagePart> stage = new NativeArray<StagePart>(stageParts.Value.Count, Allocator.Persistent);
                    for (int i = 0; i < stageParts.Value.Count; i++)
                    {
                        stage[i] = new StagePart { Value = stageParts.Value[i] };
                    }

                    stagesBuffer.Add(new Stage { Parts = stage });
                }
            }
        }

        private static void AddComponentsToEntity(PartBase part, Entity entity, EntityManager entityManager)
        {
            //TODO: Probably a better way to add components to certain parts.
            switch (part.PartId)
            {
                case "Engine_M":
                case "Engine_S":
                    entityManager.AddComponentData(entity, new Engine { MaxThrust = (part as RocketPart).GetMaxThrust() });
                    break;
                case "Capsule_M":
                case "Capsule_S":
                    var controlPod = new ControlPod { CurrentStageIndex = 0 };
                    entityManager.AddComponentData(entity, controlPod);

                    if (!_controllComponentAdded)
                    {
                        _playerControlledControlPod = entity;
                        entityManager.AddComponentData(entity, new PlayerControlledTag());
                        _controllComponentAdded = true;
                    }
                    break;
            }
        }

        private static async Task CreateMeshForEntity(Entity entity, EntityManager entityManager, string modelPath, PartBase part)
        {
            var gltf = new GltfImport();
            var importSettings = new ImportSettings()
            {
                AnimationMethod = AnimationMethod.Mecanim,
                AnisotropicFilterLevel = 16,
                DefaultMagFilterMode = Sampler.MagFilterMode.Nearest,
                DefaultMinFilterMode = Sampler.MinFilterMode.Nearest,
                GenerateMipMaps = true,
                NodeNameMethod = NameImportMethod.Original
            };
            var bytes = await File.ReadAllBytesAsync(modelPath);
            await gltf.LoadGltfBinary(bytes, new Uri(modelPath), importSettings);

            var meshes = gltf.GetMeshes();
            var meshRefs = new UnityObjectRef<Mesh>[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                meshRefs[i] = new UnityObjectRef<Mesh>
                {
                    Value = meshes[i]
                };
            }

            var materials = gltf.GetMaterial();
            var materialRefs = new UnityObjectRef<Material>[1];
            materialRefs[0] = new UnityObjectRef<Material>
            {
                Value = materials
            };

            var materialMeshIndices = new MaterialMeshIndex[1]
            {
                    new MaterialMeshIndex()
            };

            var renderMeshDescription = new RenderMeshDescription()
            {
                FilterSettings = RenderFilterSettings.Default,
                LightProbeUsage = LightProbeUsage.Off
            };

            var renderMeshArray = new RenderMeshArray(
                new ReadOnlySpan<UnityObjectRef<Material>>(materialRefs),
                new ReadOnlySpan<UnityObjectRef<Mesh>>(meshRefs),
                new ReadOnlySpan<MaterialMeshIndex>(materialMeshIndices)
                );

            RenderMeshUtility.AddComponents(
                entity,
                entityManager,
                renderMeshDescription,
                renderMeshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

            entityManager.AddComponentData(entity, new LocalTransform()
            {
                Position = part.transform.position,
                Scale = 1,
            });
        }
    }
}

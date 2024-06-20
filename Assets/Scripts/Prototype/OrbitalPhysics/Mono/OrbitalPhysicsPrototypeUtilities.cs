using System;
using Kosmos.FloatingOrigin;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public static class OrbitalPhysicsPrototypeUtilities
    {
        public static void AddBodyGeometryComponents(
            EntityManager entityManager, 
            Entity entity, 
            Mesh mesh, 
            Material material, 
            double equatorialRadiusM)
        {
            var renderMeshDescription = new RenderMeshDescription()
            {
                FilterSettings = RenderFilterSettings.Default,
                LightProbeUsage = LightProbeUsage.Off
            };
            
            var meshRefs = new UnityObjectRef<Mesh>[1];
            meshRefs[0] = new UnityObjectRef<Mesh>()
            {
                Value = mesh
            };
            
            var materialRefs = new UnityObjectRef<Material>[1];
            materialRefs[0] = new UnityObjectRef<Material>()
            {
                Value = material
            };

            var materialIndices = new MaterialMeshIndex[]
            {
                new MaterialMeshIndex()
                {
                    MeshIndex = 0,
                    MaterialIndex = 0,
                    SubMeshIndex = 0
                }
            };

            var meshSpan = new ReadOnlySpan<UnityObjectRef<Mesh>>(meshRefs);
            var materialSpan = new ReadOnlySpan<UnityObjectRef<Material>>(materialRefs);
            var materialMeshIndexSpan = new ReadOnlySpan<MaterialMeshIndex>(materialIndices);

            var renderMeshArray = new RenderMeshArray(materialSpan, meshSpan, materialMeshIndexSpan);
            
            RenderMeshUtility.AddComponents(
                entity, 
                entityManager, 
                renderMeshDescription,
                renderMeshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0)
            );
            
            // Floating scale
            var floatingScale = new FloatingScaleData()
            {
                Value = equatorialRadiusM
            };
            
            entityManager.AddComponentData(entity, floatingScale);
            
            entityManager.AddComponentData(entity, new LocalTransform()
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = (float)equatorialRadiusM
            });
        }
    }
}
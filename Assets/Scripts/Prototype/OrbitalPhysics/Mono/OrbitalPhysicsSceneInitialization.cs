using System;
using System.Collections.Generic;
using System.IO;
using Kosmos.FloatingOrigin;
using Kosmos.Time;
using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public class OrbitalPhysicsSceneInitialization : MonoBehaviour
    {
        [SerializeField] private Mesh _sphereMesh;
        [SerializeField] private Material _sphereMaterial;
        
        private Dictionary<StarSystemFileBodyEntry, Entity> _orbitalEntities = 
            new Dictionary<StarSystemFileBodyEntry, Entity>();

        private Dictionary<string, StarSystemFileBodyEntry> _idMap = 
            new Dictionary<string, StarSystemFileBodyEntry>();
        
        private void Start()
        {
            CreateTime();
            var starData = DeserializeStarFile();
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            foreach (var body in starData.CelestialBodies)
            {
                // Create the root transform entity
                var rootEntity = entityManager.CreateEntity();
                entityManager.SetName(rootEntity, body.BodyData.FormalName);

                AddFloatingOriginComponents(entityManager, rootEntity, body);

                _orbitalEntities.Add(body, rootEntity);
                _idMap.Add(body.Id, body);

                // Create the entity that holds the body's geometry
                // -- Since each body will have separate mesh data, moving those shared components to
                // a separate entity keeps all the orbit calculation entities (rootEntity) in the same chunk.
                var geometryEntity = entityManager.CreateEntity();
                entityManager.SetName(geometryEntity, $"{body.BodyData.FormalName}_geo");
                
                // Parent the geometry entity to the root entity
                entityManager.AddComponentData(geometryEntity, new Parent()
                {
                    Value = rootEntity
                });
                
                // Create new material with specified color
                ColorUtility.TryParseHtmlString(body.BodyData.ColorCode, out var color);
                var mat = new Material(_sphereMaterial);
                mat.color = color;
                
                // Add the body's geometry to the geometry entity
                AddBodyGeometryComponents(entityManager, geometryEntity, _sphereMesh, mat, body);
            }
            
            ResolveBodyUpdateOrder();

            foreach (var orbitalEntity in _orbitalEntities)
            {
                AddCommonBodyComponents(entityManager, orbitalEntity.Value, orbitalEntity.Key);
                
                switch (orbitalEntity.Key.Type)
                {
                    case "star":
                    {
                        break;
                    }
                    case "planet":
                    {
                        AddOrbitalBodyComponents(entityManager, orbitalEntity.Value, orbitalEntity.Key);
                        break;
                    }
                }
            }
        }

        private void ResolveBodyUpdateOrder()
        {
            var bodiesLeftToResolve = _idMap.Count;
            
            // Determine the order in which each body needs to be updated to ensure that its parent body
            // is always updated first
            while (bodiesLeftToResolve > 0)
            {
                foreach (var entry in _idMap)
                {
                    var body = entry.Value;
                    
                    if (body.UpdateOrder != -1)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(body.ParentId))
                    {
                        body.UpdateOrder = 0;
                        bodiesLeftToResolve--;
                        continue;
                    }

                    if (_idMap.TryGetValue(body.ParentId, out var parentBody))
                    {
                        if (parentBody.UpdateOrder != -1)
                        {
                            body.UpdateOrder = parentBody.UpdateOrder + 1;
                            bodiesLeftToResolve--;
                        }
                    }
                }
            }
        }

        private StarSystemFile DeserializeStarFile()
        {
            var dirPath = Path.Combine(
                Application.streamingAssetsPath,
                "CelestialBodies"
            );
            
            var starFilePath = Path.Combine(
                dirPath,
                "protopia.star"
            );
            
            var starSystemDeserializer = new StarSystemFileDeserializer();
            var starSystemFile = starSystemDeserializer.DeserializeStarSystemFile(starFilePath);

            foreach (var body in starSystemFile.CelestialBodies)
            {
                var bodyFilePath = Path.Combine(dirPath, body.BodyFile);
                body.BodyData = starSystemDeserializer.DeserializeCelestialBodyData(bodyFilePath);
            }

            return starSystemFile;
        }

        private void CreateTime()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new UniversalTime()
            {
                Value = 0.0
            });
            entityManager.AddComponentData(entity, new UniversalTimeModifier()
            {
                Value = 1.0
            });
            entityManager.AddComponentData(entity, new UniversalTimePaused()
            {
                Value = false
            });
            entityManager.AddComponentData(entity, new IsCurrentPlayerTimelineTag());
        }

        private void AddCommonBodyComponents(
            EntityManager entityManager,
            Entity entity,
            StarSystemFileBodyEntry body)
        {
            entityManager.AddComponentData(entity, new Mass()
            {
                ValueKg = body.BodyData.MassKg
            });

            entityManager.AddComponentData(entity, new BodyRadius()
            {
                EquatorialRadiusMeters = body.BodyData.EquatorialRadiusM
            });
            
            OrbitalBodyEntityUtilities.AddUpdateOrderTagToEntity(entityManager, entity, body.UpdateOrder);
        }

        private void AddOrbitalBodyComponents(
            EntityManager entityManager,
            Entity entity,
            StarSystemFileBodyEntry body)
        {
            _idMap.TryGetValue(body.ParentId, out var parentData);

            if (parentData == null)
            {
                Debug.LogError($"[OrbitalPhysicsSceneInitialization] Failed to find parent body with id {body.ParentId}");
                return;
            }
            
            _orbitalEntities.TryGetValue(parentData, out var parentEntity);

            if (parentEntity == Entity.Null)
            {
                Debug.LogError($"[OrbitalPhysicsSceneInitialization] Failed to find parent entity with id {body.ParentId}");
                return;
            }
            
            var parentMass = parentData.BodyData.MassKg;
            
            var orbitalPeriod = OrbitMath.ComputeOrbitalPeriodSeconds(
                body.Orbit.SemiMajorAxisM,
                parentMass
            );
            
            entityManager.AddComponentData(entity, new KeplerElements()
            {
                SemiMajorAxisMeters = body.Orbit.SemiMajorAxisM,
                Eccentricity = body.Orbit.Eccentricity,
                EclipticInclinationRadians = math.radians(body.Orbit.InclinationDeg),
                LongitudeOfAscendingNodeRadians = math.radians(body.Orbit.LongitudeAscNodeDeg),
                ArgumentOfPeriapsisRadians = math.radians(body.Orbit.ArgPeriapsisDeg),
                OrbitalPeriodInSeconds = orbitalPeriod
            });

            entityManager.AddComponentData(entity, new MeanAnomaly()
            {
                MeanAnomalyAtEpoch = math.radians(body.Orbit.MeanAnomalyAtEpochDeg)
            });

            entityManager.AddComponentData(entity, new BodyParentData()
            {
                ParentMassKg = parentMass,
                ParentEntity = parentEntity
            });
            
            entityManager.AddComponentData(entity, new ParentFloatingPositionData());
        }
        
        private void AddFloatingOriginComponents(
            EntityManager entityManager, 
            Entity entity,
            StarSystemFileBodyEntry body)
        {
            entityManager.AddComponentData(entity, new LocalToWorld());
            entityManager.AddComponentData(entity, new LocalTransform()
            {
                Scale = 1f
            });
            
            entityManager.AddComponentData(entity, new FloatingPositionData());
        }
        
        private void AddBodyGeometryComponents(
            EntityManager entityManager, 
            Entity entity, 
            Mesh mesh, 
            Material material,
            StarSystemFileBodyEntry body)
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
                Value = body.BodyData.EquatorialRadiusM
            };
            
            entityManager.AddComponentData(entity, floatingScale);
            
            entityManager.AddComponentData(entity, new LocalTransform()
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = (float)body.BodyData.EquatorialRadiusM
            });
        }
    }
}

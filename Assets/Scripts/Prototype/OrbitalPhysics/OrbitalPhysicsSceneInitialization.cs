using System;
using Kosmos.Time;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public class OrbitalPhysicsSceneInitialization : MonoBehaviour
    {
        private void Start()
        {
            // Create time
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
    }
}

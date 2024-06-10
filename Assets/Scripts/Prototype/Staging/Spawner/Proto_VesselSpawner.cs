using System;
using Kosmos.Prototype.Staging.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Prototype.Staging.Spawner
{
    public class Proto_VesselSpawner : MonoBehaviour
    {
        private void Start()
        {
            SpawnVessel();
        }
        
        private void SpawnVessel()
        {
            Debug.Log("Spawning vessel...");
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            // Create control pod entity
            var controlPod = entityManager.CreateEntity();
            entityManager.AddComponentData(controlPod, new ControlPod()
            {
                StageIndex = 0
            });
            entityManager.AddComponentData(controlPod, new PlayerControlledTag());
            
            
            // Create parachute entity
            var parachute = entityManager.CreateEntity();
            entityManager.AddComponentData(parachute, new Parachute());
            
            
            // Create two engine entities
            var engine1 = entityManager.CreateEntity();
            entityManager.AddComponentData(engine1, new Engine());
            
            var engine2 = entityManager.CreateEntity();
            entityManager.AddComponentData(engine2, new Engine());

            
            // Create a buffer to store the list of stages
            var stagesBuffer = entityManager.AddBuffer<Stage>(controlPod);
            
            
            // Create array of parts belonging to stage 0
            var stageZeroPartsArray = new NativeArray<StagePart>(2, Allocator.Persistent);
            stageZeroPartsArray[0] = new StagePart() { Value = engine1 };
            stageZeroPartsArray[1] = new StagePart() { Value = engine2 };
            
            
            // Create array of parts belonging to stage 1
            var stageOnePartsArray = new NativeArray<StagePart>(1, Allocator.Persistent);
            stageOnePartsArray[0] = new StagePart() { Value = parachute };
            
            
            // Add the stages to the buffer
            stagesBuffer.Add(new Stage() { Parts = stageZeroPartsArray });
            stagesBuffer.Add(new Stage() { Parts = stageOnePartsArray });
            
            
            Debug.Log("Vessel spawned.");
        }
    }
}
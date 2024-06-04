using Kosmos.Prototype.Staging.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototype.Staging.Systems
{
    public partial class StagingSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<ShouldStageTag>();
        }

        protected override void OnUpdate()
        {
            // Get a command buffer to handle structural changes.
            // Structural changes occur when adding or removing components from entities.
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            // Loop through all ControlPod entities with the ShouldStage tag
            Entities
                .WithAll<ShouldStageTag>()
                .ForEach((ref Entity entity, ref ControlPod controlPod, ref DynamicBuffer<Stage> stagesBuffer) =>
            {
                
                // Remove the ShouldStage tag from the control pod
                ecb.RemoveComponent<ShouldStageTag>(entity);
                
                // Get the pod's stage index
                var stageIndex = controlPod.StageIndex;

                if (stageIndex < 0 || stageIndex >= stagesBuffer.Length)
                {
                    Debug.Log($"No more stages to activate. Hope you have enough Delta-V!");
                    return;
                }
                
                // Get the array of entities to stage
                var stages = stagesBuffer[stageIndex];
                
                // Loop through the array of entities to stage
                for (int i = 0; i < stages.Parts.Length; i++)
                {
                    // Add the ShouldStage tag to each entity that needs to be staged
                    var entityToStage = stages.Parts[i].Value;
                    ecb.AddComponent(entityToStage, new ShouldStageTag());
                }
                
                // Increment the stage index
                controlPod.StageIndex++;

            })
            .Run();
            
            // Execute the command buffer
            ecb.Playback(EntityManager);
        }
    }
}
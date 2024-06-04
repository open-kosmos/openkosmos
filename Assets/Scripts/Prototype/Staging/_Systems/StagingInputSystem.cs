using Kosmos.Prototype.Staging.Components;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototype.Staging.Systems
{
    [BurstCompile]
    public partial class StagingInputSystem : SystemBase
    {
        
        [BurstCompile]
        protected override void OnUpdate()
        {
            // Get the staging key down state
            var stagingInputActive = Input.GetKeyDown(KeyCode.Space);
            
            // Set the ShouldStage flag on the player's control pod
            Entities.ForEach((ref Entity entity, ref ControlPod controlPod, in PlayerControlledTag playerControlledTag) =>
            {
                if (stagingInputActive)
                {
                    EntityManager.AddComponent<ShouldStageTag>(entity);
                }
            })
            .WithStructuralChanges()
            .Run();
        }
    }
}
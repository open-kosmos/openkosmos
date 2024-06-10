using Kosmos.Prototype.Staging.Components;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototype.Staging.Systems
{
    public partial class ParachuteStagingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<ShouldStageTag>()
                .ForEach((ref Entity entity, ref Parachute parachute) =>
                {
                    
                    Debug.Log("Parachute deployed!");
                    
                    EntityManager.RemoveComponent<ShouldStageTag>(entity);
                    
                })
                .WithStructuralChanges()
                .Run();
        }
    }
}
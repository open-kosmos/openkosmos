using Kosmos.Prototype.Staging.Components;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototype.Staging.Systems
{
    public partial class EngineStagingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<ShouldStageTag>()
                .ForEach((ref Entity entity, ref Engine engine) =>
                {
                    
                    Debug.Log("Engine ignited!");
                    
                    EntityManager.RemoveComponent<ShouldStageTag>(entity);
                    
                })
                .WithStructuralChanges()
                .Run();
        }
    }
}
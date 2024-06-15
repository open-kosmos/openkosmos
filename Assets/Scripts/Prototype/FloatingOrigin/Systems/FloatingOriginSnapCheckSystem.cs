using Kosmos.Prototype.Character;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Kosmos.FloatingOrigin
{
    [UpdateAfter(typeof(PlayableCharacterMovementSystem))]
    [UpdateBefore(typeof(FloatingPositionToWorldPositionUpdateSystem))]
    public partial class FloatingOriginSnapCheckSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<FloatingOriginData>();
            RequireForUpdate<FloatingFocusTag>();
        }

        protected override void OnUpdate()
        {
            var focusEntity = SystemAPI.GetSingletonEntity<FloatingFocusTag>();

            var focusEntityTransform =
                EntityManager.GetComponentData<LocalTransform>(focusEntity);
            
            var focusPosition = focusEntityTransform.Position;

            if (math.length(focusPosition) > 1000f)
            {
                var floatingOriginData = SystemAPI.GetSingleton<FloatingOriginData>();
                
                floatingOriginData.ShouldSnap = true;
                SystemAPI.SetSingleton(floatingOriginData);
            }
        }
    }
}

using Kosmos.Prototype.Character;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Kosmos.FloatingOrigin
{
    /// <summary>
    /// System responsible for checking if the floating origin currently exceeds the snap threshold
    /// and should be snapped to the focus entity's position.
    /// </summary>
    [UpdateAfter(typeof(PlayableCharacterMovementSystem))]
    [UpdateBefore(typeof(FloatingPositionToWorldPositionUpdateSystem))]
    [DisableAutoCreation] // DISABLED -- Let's see how far a continuous floating origin system gets us.
    public partial class DISABLED_FloatingOriginSnapCheckSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<FloatingOriginData>();
            RequireForUpdate<FloatingFocusTag>();
        }

        protected override void OnUpdate()
        {
            var floatingOriginData = SystemAPI.GetSingleton<FloatingOriginData>();
            var floatingFocusEntity = SystemAPI.GetSingletonEntity<FloatingFocusTag>();
            var floatingFocusPosition = EntityManager.GetComponentData<FloatingPositionData>(floatingFocusEntity);

            var vectorFromOrigin = FloatingOriginMath.VectorFromFloatingOrigin(
                floatingOriginData, floatingFocusPosition);
            
            floatingOriginData.ShouldSnap = true;
            SystemAPI.SetSingleton(floatingOriginData);
        }
    }
}

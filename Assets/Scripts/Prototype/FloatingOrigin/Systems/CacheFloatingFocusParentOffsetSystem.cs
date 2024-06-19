using Kosmos.Prototype.Character;
using Kosmos.Prototype.OrbitalPhysics;
using Unity.Entities;

namespace Kosmos.FloatingOrigin
{
    /// <summary>
    /// System responsible for CACHING the offset between the parent entity and the floating focus entity
    /// AFTER movement input has been applied, but BEFORE the parent's orbital position is updated.
    /// </summary>
    [UpdateAfter(typeof(PlayableCharacterMovementSystem))]
    [UpdateBefore(typeof(OrbitToFloatingPositionUpdateSystem))]
    public partial class CacheFloatingFocusParentOffsetSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<FloatingFocusParent>();
        }

        protected override void OnUpdate()
        {
            Entities
                .ForEach((ref FloatingFocusParent parentData,
                    in FloatingPositionData floatingPositionData) =>
                {
                    var parentFloatingPosition =
                        EntityManager.GetComponentData<FloatingPositionData>(parentData.ParentEntity);

                    var offset = FloatingOriginMath.VectorFromPosition(parentFloatingPosition, floatingPositionData);
                    
                    parentData.Offset = offset;
                })
                .WithoutBurst()
                .Run();
        }
    }
}
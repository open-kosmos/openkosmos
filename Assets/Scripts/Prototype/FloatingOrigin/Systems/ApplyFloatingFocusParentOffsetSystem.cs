using Kosmos.Prototype.OrbitalPhysics;
using Unity.Entities;

namespace Kosmos.FloatingOrigin
{
    /// <summary>
    /// System responsible for APPLYING the focus entity's offset from their parent AFTER the
    /// parent's orbital position has been updated.
    ///
    /// The Floating Focus Parent is a parent entity to which the Floating Focus entity is spatially "affixed."
    /// </summary>
    [UpdateAfter(typeof(OrbitToFloatingPositionUpdateSystem))]
    [UpdateBefore(typeof(FloatingPositionToWorldPositionUpdateSystem))]
    public partial class ApplyFloatingFocusParentOffsetSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<FloatingFocusParent>();
        }
        
        protected override void OnUpdate()
        {
            Dependency.Complete();
            
            Entities
                .ForEach((ref FloatingPositionData floatingPositionData,
                        in FloatingFocusParent parentData) =>
                {
                    var parentFloatingPosition =
                        EntityManager.GetComponentData<FloatingPositionData>(parentData.ParentEntity);
                    
                    var floatingPosition = FloatingOriginMath.Add(parentFloatingPosition, parentData.Offset);
                    
                    floatingPositionData = floatingPosition;
                })
                .WithoutBurst()
                .Run();
        }
    }
}

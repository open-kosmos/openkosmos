using Unity.Entities;
using Unity.Mathematics;

namespace Kosmos.FloatingOrigin
{
    [UpdateBefore(typeof(FloatingPositionToWorldPositionUpdateSystem))]
    public partial class FloatingOriginSnapCheckSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<FloatingOriginData>();
            RequireForUpdate<FloatingFocusData>();
        }

        protected override void OnUpdate()
        {
            var focusEntity = SystemAPI.GetSingletonEntity<FloatingFocusData>();
            var floatingOrigin = SystemAPI.GetSingleton<FloatingOriginData>();

            var focusEntityPosition =
                EntityManager.GetComponentData<FloatingPositionData>(focusEntity);

            var focusPosition = FloatingOriginMath.VectorFromFloatingOrigin(
                floatingOrigin, focusEntityPosition);

            var scaledPosition = focusPosition / floatingOrigin.Scale;

            if (math.length(scaledPosition) > 10f)
            {
                var floatingOriginData = SystemAPI.GetSingleton<FloatingOriginData>();

                // TODO: Account for global grid
                floatingOriginData.LocalX += focusPosition.x;
                floatingOriginData.LocalY += focusPosition.y;
                floatingOriginData.LocalZ += focusPosition.z;

                SystemAPI.SetSingleton<FloatingOriginData>(floatingOriginData);
            }
        }
    }
}

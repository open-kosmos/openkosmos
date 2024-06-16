using Kosmos.Camera;
using Unity.Entities;
using Unity.Mathematics;

namespace Kosmos.FloatingOrigin
{
    public partial class FloatingOriginScaleUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<FloatingOriginData>();
            RequireForUpdate<PlayerInput>();
        }

        protected override void OnUpdate()
        {
            var input = SystemAPI.GetSingleton<PlayerInput>();

            if (!input.ScaleZoomActive)
            {
                return;
            }
            
            var floatingOrigin = SystemAPI.GetSingleton<FloatingOriginData>();

            var scale = floatingOrigin.Scale;
            
            if (input.ZoomInputValue.magnitude < 0.01)
            {
                return;
            }
            
            scale += input.ZoomInputValue.y * scale * SystemAPI.Time.DeltaTime;

            floatingOrigin.Scale = math.clamp(scale, 1.0, double.MaxValue);
            floatingOrigin.ShouldSnap = true;
            
            SystemAPI.SetSingleton(floatingOrigin);
        }
    }
}
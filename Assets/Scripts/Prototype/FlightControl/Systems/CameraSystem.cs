using Assets.Scripts.Prototype.FlightControl.Components;
using Kosmos.Prototype.Parts.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class CameraSystem : SystemBase
{

    protected override void OnCreate()
    {
        RequireForUpdate<PlayerControlledTag>();
    }

    protected override void OnUpdate()
    {
        var cameraData = EntityManager.GetComponentData<FlightControlCameraData>(SystemHandle);
        var cameraTransform = cameraData.Camera.transform;

        Entities.ForEach((ref Entity entity, ref ControlPod controlPod, ref LocalTransform localTransform, in PlayerControlledTag playerControlledTag) =>
        {
            cameraTransform.position =
                new Vector3(localTransform.Position.x, localTransform.Position.y, localTransform.Position.z) + cameraData.Offset;
        })
        .WithoutBurst()
        .Run();
    }
}

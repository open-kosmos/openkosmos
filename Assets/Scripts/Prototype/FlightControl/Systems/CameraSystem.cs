using Kosmos.Prototype.Parts.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class CameraSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var camera = GameObject.FindGameObjectWithTag("MainCamera");
        Vector3 cameraOffset = new Vector3(0, 12, -60);

        Entities.ForEach((ref Entity entity, ref ControlPod controlPod, ref LocalTransform localTransform, in PlayerControlledTag playerControlledTag) =>
        {
            camera.transform.position =
                new Vector3(localTransform.Position.x, localTransform.Position.y, localTransform.Position.z) + cameraOffset;
        })
        .WithoutBurst()
        .Run();
    }
}

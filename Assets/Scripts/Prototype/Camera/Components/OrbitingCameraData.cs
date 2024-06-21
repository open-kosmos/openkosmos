using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Kosmos.Camera
{
    public class OrbitingCameraData : IComponentData
    {
        public UnityEngine.Camera Camera;
        public float CameraPanSpeed;
        public float CameraOrbitSpeed;
        public float CurrentPitchAngle;
        public float CurrentYawAngle;
        public float3 BaseOffset;
        public float CameraZoomSpeed;
        public float3 CurrentOffset;
    }
}

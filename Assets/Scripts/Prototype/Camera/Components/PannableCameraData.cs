using Unity.Entities;
using UnityEngine;

namespace Kosmos.Camera
{
    public class PannableCameraData : IComponentData
    {
        public UnityEngine.Camera Camera;
        public float CameraPanSpeed;
        public float CurrentPitchAngle;
        public float CurrentYawAngle;
        public float CurrentDistance;
        public float CameraZoomSpeed;
    }
}

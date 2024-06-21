using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Prototype.FlightControl.Components
{
    public class FlightControlCameraData : IComponentData
    {
        public Camera Camera;
        public Vector3 Offset = new Vector3(0, 12, -60);
    }
}

using Unity.Entities;
using UnityEngine;

namespace Kosmos.Camera
{
    public struct CameraPanInput : IComponentData
    {
        public Vector2 PanInputValue;
        public Vector2 OrbitInputValue;
        public bool OrbitActive;
        public Vector2 ZoomInputValue;
    }
}

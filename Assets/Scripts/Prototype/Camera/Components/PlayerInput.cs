using Unity.Entities;
using UnityEngine;

namespace Kosmos.Camera
{
    public struct PlayerInput : IComponentData
    {
        public Vector2 TranslationValue;
        public Vector2 OrbitInputValue;
        public bool OrbitActive;
        public Vector2 ZoomInputValue;
    }
}

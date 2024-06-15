using UnityEngine;

namespace Kosmos.Prototype.Parts
{
    public class RocketPart : StageablePart
    {
        [SerializeField] private float _maxThrust;

        [Tweakable] private float _currentThrust;
    }
}

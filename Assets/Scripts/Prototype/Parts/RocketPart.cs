using UnityEngine;
using UnityEngine.Serialization;

namespace Kosmos.Prototype.Parts
{
    public class RocketPart : PartBase
    {
        [SerializeField] private float _maxThrust;

        [Tweakable] private float _currentThrust;

    }
}

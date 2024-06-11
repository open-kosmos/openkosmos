using UnityEngine;
using UnityEngine.Serialization;

namespace Kosmos.Prototype.Parts
{
    public class FuelTankPart : PartBase
    {
        [FormerlySerializedAs("MaxFuel")]
        [SerializeField] private float _maxFuel;

        [Tweakable]
        private float _currentFuel = 1.0f;

        public override float GetMass()
        {
            return _currentFuel * _maxFuel;
        }
    }
}
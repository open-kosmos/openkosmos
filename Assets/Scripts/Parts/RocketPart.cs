using UnityEngine;
using UnityEngine.Serialization;

namespace Arkship.Parts
{
    public class RocketPart : PartBase
    {
        //TODO - this should come from somewhere else
        private const float FUEL_DENSITY = 1.0f;
        
        [FormerlySerializedAs("MaxFuel")]
        [SerializeField] private float _maxFuel;

        [FormerlySerializedAs("MaxThrust")]
        [SerializeField] private float _maxThrust;

        [Tweakable]
        private float _currentFuel = 1.0f;

        public override float GetMass()
        {
            return _currentFuel * _maxFuel * FUEL_DENSITY;
        }
    }
}

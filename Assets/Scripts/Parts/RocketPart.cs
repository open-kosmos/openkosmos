using UnityEngine;

namespace Arkship.Parts
{
    public class RocketPart : PartBase
    {
        //TODO - this should come from somewhere else
        private const float FUEL_DENSITY = 1.0f;
        
        [SerializeField] public float MaxFuel;
        [SerializeField] public float MaxThrust;

        //Some sort of attribure here for "Tweakable"
        private float CurrentFuel;

        public override float GetMass()
        {
            return CurrentFuel * FUEL_DENSITY;
        }
    }
}

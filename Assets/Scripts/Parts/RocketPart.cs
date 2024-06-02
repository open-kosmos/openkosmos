using UnityEngine;

namespace Arkship.Parts
{
    public class RocketPart : PartBase
    {
        //TODO - this should come from somewhere else
        private const float FUEL_DENSITY = 1.0f;
        
        [SerializeField] public float MaxFuel;
        [SerializeField] public float MaxThrust;

        [Tweakable]
        private float CurrentFuel = 1.0f;

        public override float GetMass()
        {
            return CurrentFuel * FUEL_DENSITY;
        }
    }
}

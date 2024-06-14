using Kosmos.FloatingOrigin;
using Unity.Entities;
using UnityEngine;

namespace Kosmos.Prototype.Character
{
    public class PlayerCharacterAuthoring : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 20f;
        
        private class PlayerCharacterAuthoringBaker : Baker<PlayerCharacterAuthoring>
        {
            public override void Bake(PlayerCharacterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var playerCharacter = new PlayableCharacterData()
                {
                    MoveSpeed = authoring._moveSpeed
                };
                
                AddComponent(entity, playerCharacter);
            }
        }
    }
}
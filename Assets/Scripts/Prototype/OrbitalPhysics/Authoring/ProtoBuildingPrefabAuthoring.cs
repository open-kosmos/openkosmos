using Unity.Entities;
using UnityEngine;

namespace Prototype.OrbitalPhysics.Authoring
{
    public struct BuildingPrefab : IComponentData
    {
        public Entity Prefab;
    }
    
    public class ProtoBuildingPrefabAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        
        private class ProtoBuildingPrefabAuthoringBaker : Baker<ProtoBuildingPrefabAuthoring>
        {
            public override void Bake(ProtoBuildingPrefabAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var prefabEntity = GetEntity(authoring._prefab, TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingPrefab()
                {
                    Prefab = prefabEntity
                });
            }
        }
    }
}
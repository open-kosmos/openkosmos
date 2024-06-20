using Unity.Entities;

namespace Kosmos.Prototype.OrbitalPhysics
{
    /// <summary>
    /// Static methods to assist with the creation and management of orbital body entities.
    /// </summary>
    public static class OrbitalBodyEntityUtilities
    {
        public static void AddUpdateOrderTagToEntity(EntityManager entityManager, Entity entity, int order)
        {
            switch (order)
            {
                case 1:
                    entityManager.AddComponentData(entity, new BodyUpdateOrderTag1());
                    break;
                case 2:
                    entityManager.AddComponentData(entity, new BodyUpdateOrderTag2());
                    break;
                case 3:
                    entityManager.AddComponentData(entity, new BodyUpdateOrderTag3());
                    break;
                case 4:
                    entityManager.AddComponentData(entity, new BodyUpdateOrderTag4());
                    break;
                case 5:
                    entityManager.AddComponentData(entity, new BodyUpdateOrderTag5());
                    break;
            }

            entityManager.AddComponentData(entity, new OrbitalBodyUpdateOrder()
            {
                UpdateOrder = order
            });
        }
    }
}

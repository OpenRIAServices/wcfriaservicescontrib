using System.ServiceModel.DomainServices.Client;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> where TEntity : Entity
    {
        #region DetachEntityGraph
        /// <summary>
        /// CCC Extension method that detaches a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by associations which are marked with an entity graph attribute of type 'GraphType'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="entity"></param>
        public void DetachEntityGraph(EntitySet<TEntity> entitySet)
        {
            GraphOperation(e => DetachAction(e, entitySet.EntityContainer));
        }
        private Entity DetachAction(Entity entity, EntityContainer entityContainer)
        {
            var entitySetToDetachFrom = entityContainer.GetEntitySet(entity.GetType());
            entitySetToDetachFrom.Detach(entity);
            return entity;
        }
        #endregion
        #region RemoveEntityGraph
        /// <summary>
        /// CCC Extension method that deletes a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by the 'EntityGraphAttribute' attribute.
        /// </summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitySet"></param>
        /// <param name="entity"></param>
        public void RemoveEntityGraph(EntitySet<TEntity> entitySet)
        {
            GraphOperation(e => RemoveAction(e, entitySet.EntityContainer));
        }
        private static Entity RemoveAction(Entity entity, EntityContainer entityContainer)
        {
            var entitySetToDetachFrom = entityContainer.GetEntitySet(entity.GetType());
            entitySetToDetachFrom.Remove(entity);
            return entity;
        }
        #endregion
    }
}

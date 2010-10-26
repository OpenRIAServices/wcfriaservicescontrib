using System.ServiceModel.DomainServices.Client;

namespace EntityGraph.RIA
{
    public static class EntityGraphProxies
    {
        /// <summary>
        /// Extension method that returns an entity graph object that represents the entity graph this entity is part of.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static EntityGraph<TEntity> EntityGraph<TEntity>(this TEntity entity) where TEntity : Entity
        {
            return new EntityGraph<TEntity>(entity);
        }
        /// <summary>
        /// Extension method that returns an entity graph object that represents the entity graph with given name this entity is part of.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="graphName"></param>
        /// <returns></returns>
        public static EntityGraph<TEntity> EntityGraph<TEntity>(this TEntity entity, string graphName) where TEntity : Entity
        {
            return new EntityGraph<TEntity>(entity, graphName);
        }
        /// <summary>
        /// Extension method that returns an entity graph object, defined by the provided collection of paths
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static EntityGraph<TEntity> EntityGraph<TEntity>(this TEntity entity, string[] paths) where TEntity : Entity
        {
            return new EntityGraph<TEntity>(entity, paths);
        }

        /// <summary>
        /// Extension method that clones the given entity and all associated entities marked with the GraphAttribute attribute. 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TEntity Clone<TEntity>(this TEntity entity) where TEntity : Entity
        {
            return entity.EntityGraph().Clone();
        }
        /// <summary>
        /// Extension method that clones the given entity and all associated entities marked with the GraphAttribute attribute and 
        /// which have the same gaph name. 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="graphName"></param>
        /// <returns></returns>
        public static TEntity Clone<TEntity>(this TEntity entity, string graphName) where TEntity : Entity
        {
            return entity.EntityGraph(graphName).Clone();
        }
        /// <summary>
        /// Extension method that clones the given entity and all associated entities as indicated by the given collection of paths
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static TEntity Clone<TEntity>(this TEntity entity, string[] paths) where TEntity : Entity
        {
            return entity.EntityGraph(paths).Clone();
        }
    }
}

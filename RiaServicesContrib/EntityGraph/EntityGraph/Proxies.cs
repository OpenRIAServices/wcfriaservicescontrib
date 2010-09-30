using System.ServiceModel.DomainServices.Client;

namespace RIA.EntityGraph
{
    public static class EntityGraphProxies
    {
        public static EntityGraph<T> EntityGraph<T>(this T entity) where T : Entity
        {
            return new EntityGraph<T>(entity);
        }
        public static EntityGraph<T> EntityGraph<T>(this T entity, string graphName) where T : Entity
        {
            return new EntityGraph<T>(entity, graphName);
        }

        public static TEntity Clone<TEntity>(this TEntity entity) where TEntity : Entity
        {
            return entity.EntityGraph().Clone();
        }
    }
}

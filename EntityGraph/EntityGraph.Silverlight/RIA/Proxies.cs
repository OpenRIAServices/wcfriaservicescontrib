using System.ServiceModel.DomainServices.Client;
using System;
using System.Collections.Generic;

namespace RiaServicesContrib.DomainServices.Client
{
    public static class EntityGraphProxies
    {
        /// <summary>
        /// Extension method that returns an entity graph object as defined by the provided entity graph shape object.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static EntityGraph EntityGraph<TEntity>(this TEntity entity, IEntityGraphShape shape) where TEntity : Entity
        {
            return EntityGraphFactory.Get(entity, shape);
        }
        /// <summary>
        /// Extension method that copies the given entity and all associated entities in the entity graph defined by the given 
        /// entity graph shape.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static TEntity Copy<TEntity>(this TEntity entity, IEntityGraphShape shape) where TEntity : Entity
        {
            return (TEntity)entity.EntityGraph(shape).Copy();
        }
        /// <summary>
        /// Extension method that clones the given entity and all associated entities in the entity graph defined by the given 
        /// entity graph shape.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static TEntity Clone<TEntity>(this TEntity entity, IEntityGraphShape shape) where TEntity : Entity
        {
            return (TEntity)entity.EntityGraph(shape).Clone();
        }
    }
}

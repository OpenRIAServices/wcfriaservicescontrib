using System.ServiceModel.DomainServices.Client;
using System;
using System.Collections.Generic;

namespace RiaServicesContrib.DomainServices.Client
{
    public static class EntityGraphProxies
    {
        /// <summary>
        /// Extension method that returns an entity graph object as defined by the provided entity graph attribute shape object.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static EntityGraph EntityGraph<TEntity>(this TEntity entity, EntityGraphAttributeShape shape) where TEntity : Entity
        {
            return GraphFactory.GetEntityGraph(entity, shape);
        }
        /// <summary>
        /// Extension method that returns an entity graph object as defined by the provided entity graph shape object.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static EntityGraph EntityGraph<TEntity>(this TEntity entity, EntityGraphShape shape) where TEntity : Entity
        {
            return GraphFactory.GetEntityGraph(entity, shape);
        }
        /// <summary>
        /// Extension method that copies the given entity and all associated entities in the entity graph defined by the given 
        /// entity graph attribute shape.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static TEntity Copy<TEntity>(this TEntity entity, EntityGraphAttributeShape shape) where TEntity : Entity
        {
            return (TEntity)entity.EntityGraph(shape).Copy();
        }
        /// <summary>
        /// Extension method that copies the given entity and all associated entities in the entity graph defined by the given 
        /// entity graph shape.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static TEntity Copy<TEntity>(this TEntity entity, EntityGraphShape shape) where TEntity : Entity
        {
            return (TEntity)entity.EntityGraph(shape).Copy();
        }
        /// <summary>
        /// Extension method that clones the given entity and all associated entities in the entity graph defined by the given 
        /// entity graph attribute shape.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static TEntity Clone<TEntity>(this TEntity entity, EntityGraphAttributeShape shape) where TEntity : Entity
        {
            return (TEntity)entity.EntityGraph(shape).Clone();
        }
        /// <summary>
        /// Extension method that clones the given entity and all associated entities in the entity graph defined by the given 
        /// entity graph shape.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static TEntity Clone<TEntity>(this TEntity entity, EntityGraphShape shape) where TEntity : Entity
        {
            return (TEntity)entity.EntityGraph(shape).Clone();
        }

        #region Factory
        private static class GraphFactory
        {
            private static Dictionary<int, EntityGraph> EntityGraphs
                = new Dictionary<int, EntityGraph>();

            public static EntityGraph GetEntityGraph(Entity entity, EntityGraphAttributeShape shape)
            {
                int key = entity.GetHashCode() ^ shape.GetHashCode();
                if(EntityGraphs.ContainsKey(key) == false)
                {
                    var gr = new EntityGraph(entity, shape);
                    EntityGraphs.Add(key, gr);
                    return gr;
                }
                return EntityGraphs[key];
            }
            public static EntityGraph GetEntityGraph(Entity entity, EntityGraphShape shape)
            {
                int key = entity.GetHashCode() ^ shape.GetHashCode();
                if(EntityGraphs.ContainsKey(key) == false)
                {
                    var gr = new EntityGraph(entity, shape);
                    EntityGraphs.Add(key, gr);
                    return gr;
                }
                return EntityGraphs[key];
            }
        }
        #endregion
    }
}

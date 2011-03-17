using System.ServiceModel.DomainServices.Client;
using System;
using System.Collections.Generic;

namespace RiaServicesContrib.DomainServices.Client
{
    public static class EntityGraphProxies
    {
        /// <summary>
        /// Extension method that returns an entity graph object that represents the entity graph this entity is part of.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static EntityGraph EntityGraph<TEntity>(this TEntity entity) where TEntity : Entity
        {
            return GraphFactory.GetAttributedEntityGraph(entity, null);
        }
        /// <summary>
        /// Extension method that returns an entity graph object that represents the entity graph with given name this entity is part of.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="graphName"></param>
        /// <returns></returns>
        public static EntityGraph EntityGraph<TEntity>(this TEntity entity, string graphName) where TEntity : Entity
        {
            return GraphFactory.GetAttributedEntityGraph(entity, graphName);
        }
        public static EntityGraph EntityGraph<TEntity>(this TEntity entity, EntityGraphShape shape) where TEntity : Entity
        {
            return GraphFactory.GetGraphShapeEntityGraph(entity, shape);
        }

        /// <summary>
        /// Extension method that copies the given entity and all associated entities marked with the GraphAttribute attribute. 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TEntity Copy<TEntity>(this TEntity entity) where TEntity : Entity
        {
            return (TEntity)entity.EntityGraph().Copy();
        }
        /// <summary>
        /// Extension method that copies the given entity and all associated entities marked with the GraphAttribute attribute and 
        /// which have the same graph name. 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="graphName"></param>
        /// <returns></returns>
        public static TEntity Copy<TEntity>(this TEntity entity, string graphName) where TEntity : Entity
        {
            return (TEntity)entity.EntityGraph(graphName).Copy();
        }
        /// <summary>
        /// Extension method that copies the given entity and all associated entities in the entity graph defined by the given EntityGraphShape.
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
        /// Extension method that clones the given entity and all associated entities marked with the GraphAttribute attribute. 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TEntity Clone<TEntity>(this TEntity entity) where TEntity : Entity
        {
            return (TEntity)entity.EntityGraph().Clone();
        }
        /// <summary>
        /// Extension method that clones the given entity and all associated entities marked with the GraphAttribute attribute and 
        /// which have the same graph name. 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="graphName"></param>
        /// <returns></returns>
        public static TEntity Clone<TEntity>(this TEntity entity, string graphName) where TEntity : Entity
        {
            return (TEntity)entity.EntityGraph(graphName).Clone();
        }
        /// <summary>
        /// Extension method that clones the given entity and all associated entities in the entity graph defined by the given EntityGraphShape.
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
            private static Dictionary<Tuple<int, string>, EntityGraph> AttributedEntityGraphs
                = new Dictionary<Tuple<int, string>, EntityGraph>();
            private static Dictionary<Tuple<int, EntityGraphShape>, EntityGraph> GraphShapeEntityGraphs
                = new Dictionary<Tuple<int, EntityGraphShape>, EntityGraph>();

            public static EntityGraph GetAttributedEntityGraph(Entity entity, string graphName)
            {
                var key = new Tuple<int, string>(entity.GetHashCode(), graphName);
                if(AttributedEntityGraphs.ContainsKey(key) == false)
                {
                    var gr = new EntityGraph(entity, graphName);
                    AttributedEntityGraphs.Add(key, gr);
                    return gr;
                }
                return AttributedEntityGraphs[key];
            }
            public static EntityGraph GetGraphShapeEntityGraph(Entity entity, EntityGraphShape shape)
            {
                var key = new Tuple<int, EntityGraphShape>(entity.GetHashCode(), shape);
                if(GraphShapeEntityGraphs.ContainsKey(key) == false)
                {
                    var gr = new EntityGraph(entity, shape);
                    GraphShapeEntityGraphs.Add(key, gr);
                    return gr;
                }
                return GraphShapeEntityGraphs[key];
            }
        }
        #endregion
    }
}

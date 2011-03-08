using System.ServiceModel.DomainServices.Client;
using System;
using System.Collections.Generic;

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
            return GraphFactory<TEntity>.GetAttributedEntityGraph(entity, null);
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
            return GraphFactory<TEntity>.GetAttributedEntityGraph(entity, graphName);
        }
        public static EntityGraph<TEntity> EntityGraph<TEntity>(this TEntity entity, EntityGraphShape shape) where TEntity : Entity
        {
            return GraphFactory<TEntity>.GetGraphShapeEntityGraph(entity, shape);
        }

        /// <summary>
        /// Extension method that copies the given entity and all associated entities marked with the GraphAttribute attribute. 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TEntity Copy<TEntity>(this TEntity entity) where TEntity : Entity
        {
            return entity.EntityGraph().Copy();
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
            return entity.EntityGraph(graphName).Copy();
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
            return new EntityGraph<TEntity>(entity, shape).Copy();
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
        /// which have the same graph name. 
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
        /// Extension method that clones the given entity and all associated entities in the entity graph defined by the given EntityGraphShape.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static TEntity Clone<TEntity>(this TEntity entity, EntityGraphShape shape) where TEntity : Entity
        {
            return new EntityGraph<TEntity>(entity, shape).Clone();
        }

        #region Factory
        private static class GraphFactory<TEntity> where TEntity : Entity
        {
            private static Dictionary<Tuple<int, string>, EntityGraph<TEntity>> AttributedEntityGraphs
                = new Dictionary<Tuple<int, string>, EntityGraph<TEntity>>();
            private static Dictionary<Tuple<int, EntityGraphShape>, EntityGraph<TEntity>> GraphShapeEntityGraphs
                = new Dictionary<Tuple<int, EntityGraphShape>, EntityGraph<TEntity>>();

            public static EntityGraph<TEntity> GetAttributedEntityGraph(TEntity entity, string graphName)
            {
                var key = new Tuple<int, string>(entity.GetHashCode(), graphName);
                if(AttributedEntityGraphs.ContainsKey(key) == false)
                {
                    var gr = new EntityGraph<TEntity>(entity, graphName);
                    AttributedEntityGraphs.Add(key, gr);
                    return gr;
                }
                return AttributedEntityGraphs[key];
            }
            public static EntityGraph<TEntity> GetGraphShapeEntityGraph(TEntity entity, EntityGraphShape shape)
            {
                var key = new Tuple<int, EntityGraphShape>(entity.GetHashCode(), shape);
                if(GraphShapeEntityGraphs.ContainsKey(key) == false)
                {
                    var gr = new EntityGraph<TEntity>(entity, shape);
                    GraphShapeEntityGraphs.Add(key, gr);
                    return gr;
                }
                return GraphShapeEntityGraphs[key];
            }
        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RiaServicesContrib
{
    public class EntityGraphEdge
    {
        public Type From { get; set; }
        public PropertyInfo To { get; set; }
    }

    public class EntityGraphShape : IEntityGraphShape, IEnumerable<EntityGraphEdge>
    {
        public delegate TTo EdgeType<in TFrom, out TTo>(TFrom from);
        public delegate IEnumerable<TTo> EdgeEnumType<in TFrom, TTo>(TFrom from);

        private List<EntityGraphEdge> edges = new List<EntityGraphEdge>();
        public EntityGraphShape Edge<TLHS, TRHS>(Expression<EdgeType<TLHS, TRHS>> edge)
        {
            var entityType = edge.Parameters.Single().Type;
            if(edge.Body is MemberExpression == false)
            {
                var msg = String.Format("Edge expression '{0}' is invalid; it should have the form 'A => A.B'", edge.ToString());
                throw new Exception(msg);
            }
            var mexpr = (MemberExpression)edge.Body;
            if(mexpr.Expression is ParameterExpression == false)
            {
                var msg = String.Format("Edge expression '{0}' is invalid; it should have the form 'A => A.B'", edge.ToString());
                throw new Exception(msg);
            }
            var propInfo = mexpr.Member as PropertyInfo;
            if(entityType != null && propInfo != null)
                edges.Add(new EntityGraphEdge { From = entityType, To = propInfo });
            return this;
        }
        // We can't use TEntity as the return type of EdgeEnumType, because IEnumerable<T> is not 
        // covariant in Silverlight!!. Therefore a second type parameter TRHS is needed.
        public EntityGraphShape Edge<TLHS, TRHS>(Expression<EdgeEnumType<TLHS, TRHS>> edge)
        {
            var entityType = edge.Parameters.Single().Type;
            if(edge.Body is MemberExpression == false)
            {
                var msg = String.Format("Edge expression '{0}' is invalid; it should have the form 'A => A.B'", edge.ToString());
                throw new Exception(msg);
            }
            var mexpr = (MemberExpression)edge.Body;
            if(mexpr.Expression is ParameterExpression == false)
            {
                var msg = String.Format("Edge expression '{0}' is invalid; it should have the form 'A => A.B'", edge.ToString());
                throw new Exception(msg);
            }
            var propInfo = mexpr.Member as PropertyInfo;
            if(entityType != null && propInfo != null)
                edges.Add(new EntityGraphEdge { From = entityType, To = propInfo });
            return this;
        }
        /// <summary>
        /// Returns an IEnumerable that iterates over the out edges of the given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<PropertyInfo> OutEdges(object entity)
        {
            var entityType = entity.GetType();
            return this.Where(edge => edge.From.IsAssignableFrom(entityType)).Select(edge => edge.To).Distinct();
        }

        public IEnumerator<EntityGraphEdge> GetEnumerator()
        {
            return edges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Indicates of the given property info represents an edge in this graph shape object.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool IsEdge(PropertyInfo edge)
        {
            return edges.Any(e => e.To.Name == edge.Name && e.To.PropertyType.IsAssignableFrom(edge.PropertyType));
        }
        /// <summary>
        /// Returns the object that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public object GetNode(object entity, PropertyInfo edge)
        {
            return edge.GetValue(entity, null);
        }
        /// <summary>
        /// Returns the collection og objects that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public IEnumerable GetNodes(object entity, PropertyInfo edge)
        {
            return (IEnumerable)edge.GetValue(entity, null);
        }
    }
}

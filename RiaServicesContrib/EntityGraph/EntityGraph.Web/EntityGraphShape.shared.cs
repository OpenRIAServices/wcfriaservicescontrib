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
            if(edge.Body is MemberExpression)
            {
                var mexpr = (MemberExpression)edge.Body;
                var propInfo = mexpr.Member as PropertyInfo;
                if(entityType != null && propInfo != null)
                    edges.Add(new EntityGraphEdge { From = entityType, To = propInfo });
            }
            return this;
        }
        // We can't use TEntity as the return type of EdgeEnumType, because IEnumerable<T> is not 
        // covariant in Silverlight!!. Therefore a second type parameter TRHS is needed.
        public EntityGraphShape Edge<TLHS, TRHS>(Expression<EdgeEnumType<TLHS, TRHS>> edge)
        {
            var entityType = edge.Parameters.Single().Type;
            if(edge.Body is MemberExpression)
            {
                var mexpr = (MemberExpression)edge.Body;
                var propInfo = mexpr.Member as PropertyInfo;
                if(entityType != null && propInfo != null)
                    edges.Add(new EntityGraphEdge { From = entityType, To = propInfo });
            }
            return this;
        }
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


        public bool IsEdge(PropertyInfo edge)
        {
            return edges.Any(e => e.To.Name == edge.Name && e.To.PropertyType.IsAssignableFrom(edge.PropertyType));
        }
    }
}

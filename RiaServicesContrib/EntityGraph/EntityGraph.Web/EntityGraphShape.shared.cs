using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityGraph
{
    public class EntityGraphShape : IEnumerable<Tuple<Type, PropertyInfo>> 
    {
        public delegate TTo EdgeType<in TFrom, out TTo>(TFrom from);
        public delegate IEnumerable<TTo> EdgeEnumType<in TFrom, TTo>(TFrom from);

        private List<Tuple<Type, PropertyInfo>> edges = new List<Tuple<Type, PropertyInfo>>();

        public EntityGraphShape Edge<TLHS, TRHS>(Expression<EdgeType<TLHS, TRHS>> edge)
        {
            var entityType = edge.Parameters.Single().Type;
            if(edge.Body is MemberExpression)
            {
                var mexpr = (MemberExpression)edge.Body;
                var propInfo = mexpr.Member as PropertyInfo;
                if(entityType != null && propInfo != null)
                    edges.Add(new Tuple<Type, PropertyInfo>(entityType, propInfo));
            }
            return this;
        }
        // We can't use TBase as the return type of EdgeEnumType, because IEnumerable<T> is not 
        // covariant in Silverlight!!. Therefore a second type parameter TRHS is needed.
        public EntityGraphShape Edge<TLHS, TRHS>(Expression<EdgeEnumType<TLHS, TRHS>> edge)
        {
            var entityType = edge.Parameters.Single().Type;
            if(edge.Body is MemberExpression)
            {
                var mexpr = (MemberExpression)edge.Body;
                var propInfo = mexpr.Member as PropertyInfo;
                if(entityType != null && propInfo != null)
                    edges.Add(new Tuple<Type, PropertyInfo>(entityType, propInfo));
            }
            return this; 
        }
        public IEnumerable<PropertyInfo> GetAssociations(object entity)
        {
            var entityType = entity.GetType();
            return this.Where(edge => edge.Item1 == entityType).Select(edge => edge.Item2).Distinct();
        }

        public IEnumerator<Tuple<Type, PropertyInfo>> GetEnumerator()
        {
            return edges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

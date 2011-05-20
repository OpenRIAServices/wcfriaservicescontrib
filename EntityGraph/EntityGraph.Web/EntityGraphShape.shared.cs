﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RiaServicesContrib
{
    class EntityGraphEdgeComparer : IEqualityComparer<EntityGraphEdge>
    {
        public bool Equals(EntityGraphEdge x, EntityGraphEdge y)
        {
            if(x == y)
                return true;
            return
                x.FromType.Equals(y.FromType) &&
                x.EdgeInfo.Equals(y.EdgeInfo);
        }

        public int GetHashCode(EntityGraphEdge obj)
        {
            return obj.FromType.GetHashCode() ^ obj.EdgeInfo.GetHashCode();
        }
    }

    public class EntityGraphEdge
    {
        public Type FromType { get; set; }
        private PropertyInfo _edgeInfo;
        public PropertyInfo EdgeInfo
        {
            get
            {
                return _edgeInfo;
            }
            set
            {
                if(_edgeInfo != value)
                {
                    _edgeInfo = value;
                    if(typeof(IEnumerable).IsAssignableFrom(_edgeInfo.PropertyType) && _edgeInfo.PropertyType.IsGenericType)
                    {
                        ToType = _edgeInfo.PropertyType.GetGenericArguments()[0];
                    }
                    else
                    {
                        ToType = _edgeInfo.PropertyType;
                    }
                }
            }
        }
        public Type ToType { get; private set; }
        public override string ToString()
        {
            return String.Format("{0}->{1}", FromType.Name, ToType.Name);
        }
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
                edges.Add(new EntityGraphEdge { FromType = entityType, EdgeInfo = propInfo });
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
                edges.Add(new EntityGraphEdge { FromType = entityType, EdgeInfo = propInfo });
            return this;
        }
        /// <summary>
        /// Produces the set union of two entity graph shapes by using the default equality comparer.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public EntityGraphShape Union(EntityGraphShape shape)
        {
            var union = new EntityGraphShape();
            var edges = ((IEnumerable<EntityGraphEdge>)this).Union(shape, new EntityGraphEdgeComparer());

            foreach(var edge in edges)
            {
                union.edges.Add(edge);
            }
            return union;
        }
        /// <summary>
        /// Returns an IEnumerable that iterates over the out edges of the given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<PropertyInfo> OutEdges(object entity)
        {
            if(entity == null)
            {
                return new List<PropertyInfo>();
            }
            var entityType = entity.GetType();
            return this.Where(edge => edge.FromType.IsAssignableFrom(entityType)).Select(edge => edge.EdgeInfo).Distinct();
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
            return edges.Any(e => e.EdgeInfo.Name == edge.Name && e.EdgeInfo.PropertyType.IsAssignableFrom(edge.PropertyType));
        }
        /// <summary>
        /// Returns the object that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public virtual object GetNode(object entity, PropertyInfo edge)
        {
            return edge.GetValue(entity, null);
        }
        /// <summary>
        /// Returns the collection og objects that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public virtual IEnumerable GetNodes(object entity, PropertyInfo edge)
        {
            return (IEnumerable)edge.GetValue(entity, null);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace RiaServicesContrib.DomainServices.Client
{
    public class FullEntityGraphShape : IEntityGraphShape
    {
        /// <summary>
        /// Returns an IEnumerable that iterates over the out edges of the given entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<PropertyInfo> OutEdges(object entity)
        {
            return from propInfo in entity.GetType().GetProperties() 
                   where propInfo.IsDefined(typeof(AssociationAttribute), true)
                   select propInfo;
        }
        /// <summary>
        /// Indicates of the given property info represents an edge in this graph shape object.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool IsEdge(PropertyInfo edge)
        {
            throw new NotImplementedException();
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
        /// Returns the collection of objects that is reachable from entity via the given edge.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public IEnumerable GetNodes(object entity, PropertyInfo edge)
        {
            var nodes = (IEnumerable)edge.GetValue(entity, null);
            return nodes == null ? new List<object> { } : nodes;
        }
    }
}

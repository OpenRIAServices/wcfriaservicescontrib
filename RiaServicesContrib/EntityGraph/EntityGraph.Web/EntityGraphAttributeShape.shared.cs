using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace RiaServicesContrib
{
    public class EntityGraphAttributeShape : IEntityGraphShape
    {
        /// <summary>
        /// Initializes a new instance of the EntityGraphAttributeShape class. 
        /// </summary>
        public EntityGraphAttributeShape() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the EntityGraphAttributeShape class. 
        /// </summary>
        /// <param name="Name"></param>
        public EntityGraphAttributeShape(string Name)
        {
            this.Name = Name;
        }
        /// <summary>
        /// Gets the name of this entity graph shape.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Returns an IEnumerable of PropertyInfo objects for properties which have the "RiaServicesContrib.EntityGraph" attribute. 
        /// If Name is not null, the name of the entity graph attribute shape should match the name of the attribute.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IEnumerable<System.Reflection.PropertyInfo> OutEdges(object entity)
        {
            Func<EntityGraphAttribute, bool> match =
                entityGraph => entityGraph is EntityGraphAttribute && (Name == null || Name == entityGraph.Name);
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in entity.GetType().GetProperties(bindingAttr)
                      where p.GetCustomAttributes(true).OfType<EntityGraphAttribute>().Any(match)
                      select p;
            return qry;
        }
        /// <summary>
        /// Indicates of the given property info represents an edge in this graph shape object.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool IsEdge(PropertyInfo edge)
        {
            Func<EntityGraphAttribute, bool> match =
                entityGraph => entityGraph is EntityGraphAttribute && (Name == null || Name == entityGraph.Name);

            return edge.GetCustomAttributes(true).OfType<EntityGraphAttribute>().Any(match);
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

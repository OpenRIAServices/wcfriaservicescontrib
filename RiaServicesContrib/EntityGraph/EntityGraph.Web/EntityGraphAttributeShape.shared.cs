using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace EntityGraph
{
    public class EntityGraphAttributeShape : IEntityGraphShape
    {
        public EntityGraphAttributeShape() : this(null) { }
        public EntityGraphAttributeShape(string Name)
        {
            this.Name = Name;
        }
        public string Name { get; set; }
        /// <summary>
        /// Returns an IEnumerable of PropertyInfo objects for properties which have the "EntityGraph". If entityGraphname
        /// is not null, the name of the entity graph should match entityGraphname.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="entityGraphName"></param>
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
        /// Returns true if the property has the "EntityGraphAttribute" (or a subclass), false otherwise.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool IsEdge(PropertyInfo edge)
        {
            Func<EntityGraphAttribute, bool> match =
                entityGraph => entityGraph is EntityGraphAttribute && (Name == null || Name == entityGraph.Name);

            return edge.GetCustomAttributes(true).OfType<EntityGraphAttribute>().Any(match);
        }
    }
}

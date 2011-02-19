using System;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;

namespace EntityGraph.RIA
{
    public partial class EntityGraph<TEntity> 
    {
        /// <summary>
        /// Method that clones an entity and, recursively, all its associations that are included in the entity graph
        /// </summary>
        /// <returns></returns>
        public TEntity Clone() 
        {
            return GraphMap(CloneDataMembers);
        }

        private TClone CloneDataMembers<TClone>(TClone entity) where TClone: Entity
        {
            // Create new object of type T (or subtype) using reflection and inspecting the concrete 
            // type of the entity to copy.
            TClone clone = (TClone)Activator.CreateInstance(entity.GetType());

            // Copy DataMember properties
            foreach (PropertyInfo currentPropertyInfo in GetDataMembers(entity, true))
            {
                object currentObject = currentPropertyInfo.GetValue(entity, null);
                currentPropertyInfo.SetValue(clone, currentObject, null);
            }
            return clone;
        }
    }
}

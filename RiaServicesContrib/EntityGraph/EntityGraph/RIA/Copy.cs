using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Client;

namespace EntityGraph.RIA
{
    public partial class EntityGraph<TEntity> 
    {
        /// <summary>
        /// Method that copies an entity and, recursively, all its associations that are included in the entity graph
        /// </summary>
        /// <returns></returns>
        public TEntity Copy() 
        {
            return GraphMap(CopyDataMembers);
        }

        private TCopy CopyDataMembers<TCopy>(TCopy entity) where TCopy : Entity
        {
            // Create new object of type T (or subtype) using reflection and inspecting the concrete 
            // type of the entity to copy.
            TCopy copy = (TCopy)Activator.CreateInstance(entity.GetType());

            // Copy DataMember properties
            foreach (PropertyInfo currentPropertyInfo in GetDataMembers(entity, false))
            {
                object currentObject = currentPropertyInfo.GetValue(entity, null);
                currentPropertyInfo.SetValue(copy, currentObject, null);
            }
            return copy;
        }
        /// <summary>
        /// Returns an array of PropertyInfo objects for properties which have the "DataMemberAttribute"
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="includeKeys">Indicates whether key properties should also be included</param>
        /// <returns></returns>
        private static PropertyInfo[] GetDataMembers(object obj, bool includeKeys)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in obj.GetType().GetProperties(bindingAttr)
                      where
                        p.IsDefined(typeof(DataMemberAttribute), true)
                      && (includeKeys || p.IsDefined(typeof(KeyAttribute), true) == false)
                      && p.GetSetMethod() != null
                      select p;
            return qry.ToArray();
        }
    }
}

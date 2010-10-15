using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Client;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> where TEntity : Entity
    {
        /// <summary>
        /// Method that clones an entity and, recursively, all its associations that are marked with 
        /// the 'EntityGraphAttribute' attribute
        /// </summary>
        /// <returns></returns>
        public TEntity Clone() 
        {
            return GraphOperation(CloneDataMembers);
        }

        private TClone CloneDataMembers<TClone>(TClone entity) where TClone : Entity
        {
            // Create new object of type T (or subtype) using reflection and inspecting the concrete 
            // type of the entity to clone.
            TClone clone = (TClone)Activator.CreateInstance(entity.GetType());

            // Clone DataMember properties
            foreach (PropertyInfo currentPropertyInfo in GetDataMembers(entity))
            {
                object currentObject = currentPropertyInfo.GetValue(entity, null);
                currentPropertyInfo.SetValue(clone, currentObject, null);
            }
            return clone;
        }
        /// <summary>
        /// Returns an array of PropertyInfo  objects for properties which have the "DataMemberAttribute"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected static PropertyInfo[] GetDataMembers(object obj)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var qry = from p in obj.GetType().GetProperties(bindingAttr)
                      where
                        p.IsDefined(typeof(DataMemberAttribute), true)
                      && p.IsDefined(typeof(KeyAttribute), true) == false
                      && p.GetSetMethod() != null
                      select p;
            return qry.ToArray();
        }
    }
}
        
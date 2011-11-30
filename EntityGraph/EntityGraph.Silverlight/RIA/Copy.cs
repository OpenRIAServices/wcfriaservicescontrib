using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph 
    {
        /// <summary>
        /// Method that makes a copy of an entitygraph by copying entities that are included in the entity graph.
        /// </summary>
        /// <returns></returns>
        public EntityGraph Copy() 
        {
            var copiedEntity = GraphMap(CopyDataMembers);
            return new EntityGraph(copiedEntity, GraphShape);
        }

        private TCopy CopyDataMembers<TCopy>(TCopy source) where TCopy : Entity
        {
            // Create new object of type T (or subtype) using reflection and inspecting the concrete 
            // type of the entity to copy.
            TCopy copy = (TCopy)Activator.CreateInstance(source.GetType());
            var dataMembers = GetDataMembers(source, false);
            // Copy DataMember properties
            ApplyState(copy, source, dataMembers);
            return copy;
        }
        /// <summary>
        /// Copies the values form the properties in dataMembers from sourceEntity to targetEntity.
        /// </summary>
        /// <param name="sourceEntity"></param>
        /// <param name="targetObject"></param>
        /// <param name="dataMembers"></param>
        private static void ApplyState(object targetObject, object sourceEntity, IEnumerable<PropertyInfo> dataMembers)
        {
            if(targetObject == null)
            {
                return;
            }
            StreamingContext dummy = new StreamingContext();
            Entity entityObject = targetObject as Entity;

            //Call OnDeserializing to temporarily disable validation
            if(entityObject != null)
            {
                entityObject.OnDeserializing(dummy);
            }

            // Copy DataMember properties            
            foreach(PropertyInfo currentPropertyInfo in dataMembers)
            {
                object currentObject = currentPropertyInfo.GetValue(sourceEntity, null);
                currentPropertyInfo.SetValue(targetObject, currentObject, null);
            }

            //Call OnDeserializaed to enable validation
            if(entityObject != null)
            {
                entityObject.OnDeserialized(dummy);
            }
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
                      && p.CanWrite
                      select p;
            return qry.ToArray();
        }
    }
}

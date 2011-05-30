﻿using System;
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
        /// Method that copies an entity and, recursively, all its associations that are included in the entity graph
        /// </summary>
        /// <returns></returns>
        public Entity Copy() 
        {
            return GraphMap(CopyDataMembers);
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
        /// <param name="targetEntity"></param>
        /// <param name="dataMembers"></param>
        private static void ApplyState(object targetEntity, object sourceEntity, IEnumerable<PropertyInfo> dataMembers)
        {
            // Copy DataMember properties
            foreach(PropertyInfo currentPropertyInfo in dataMembers)
            {
                object currentObject = currentPropertyInfo.GetValue(sourceEntity, null);
                currentPropertyInfo.SetValue(targetEntity, currentObject, null);
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
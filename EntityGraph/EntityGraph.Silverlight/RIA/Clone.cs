using System;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph 
    {
        /// <summary>
        /// Method that clones an entity and, recursively, all its associations that are included in the entity graph
        /// </summary>
        /// <returns></returns>
        public Entity Clone()
        {
            return GraphMap(CloneDataMembers);
        }

        private TClone CloneDataMembers<TClone>(TClone source) where TClone: Entity
        {
            // Create new object of type T (or subtype) using reflection and inspecting the concrete 
            // type of the entity to copy.
            TClone clone = (TClone)Activator.CreateInstance(source.GetType());
            var dataMembers = GetDataMembers(source, true);
            // Copy DataMember properties
            ApplyState(clone, source, dataMembers);
            return clone;
        }
    }
}

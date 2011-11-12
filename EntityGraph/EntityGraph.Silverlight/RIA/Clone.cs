﻿using System;
using System.ServiceModel.DomainServices.Client;
using RiaServicesContrib.Extensions;

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
        /// <summary>
        /// Method that clones an entity and, recursively, all its associations that are included in the entity graph.
        /// The cloned entities are added/attached to the given domain context, which will give them
        /// the same EntityState as the original entities.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Entity Clone(DomainContext context)
        {
            foreach(var entity in context.EntityContainer.GetEntitySet(Source.GetType()))
            {
                if(entity == Source)
                {
                    throw new Exception("Cannot call CloneInto for the same domain context as the source entity graph.");
                }
            }
            return GraphMap(entity => CloneDataMembersIntoDomainContext(context, entity));
        }
        private TClone CloneDataMembersIntoDomainContext<TClone>(DomainContext context, TClone source) where TClone : Entity
        {
            // Create new object of type T (or subtype) using reflection and inspecting the concrete
            // type of the entity to copy.
            TClone clone = (TClone)Activator.CreateInstance(source.GetType());
            var entitySet = context.EntityContainer.GetEntitySet(source.GetType());
            if(source.EntityState == EntityState.Unmodified || source.EntityState == EntityState.Modified)
            {
                entitySet.Attach(clone);
            }
            var originalState = source.ExtractState(ExtractType.OriginalState);
            var modifiedState = source.ExtractState(ExtractType.ModifiedState);

            clone.ApplyState(originalState, modifiedState);

            if(source.EntityState == EntityState.New)
            {
                entitySet.Add(clone);
            }
            return clone;
        }
        private TClone CloneDataMembers<TClone>(TClone source) where TClone : Entity
        {
            // Create new object of type T (or subtype) using reflection and inspecting the concrete 
            // type of the entity to copy.
            TClone clone = (TClone)Activator.CreateInstance(source.GetType());

            var originalState = source.ExtractState(ExtractType.OriginalState);
            var modifiedState = source.ExtractState(ExtractType.ModifiedState);

            clone.ApplyState(originalState, modifiedState);
            return clone;
        }
    }
}

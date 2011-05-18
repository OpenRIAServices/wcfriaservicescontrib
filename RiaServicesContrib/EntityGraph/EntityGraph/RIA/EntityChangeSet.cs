using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph
    {
        public EntityGraphChangeSet GetChanges()
        {
            List<Entity> addedEntities = new List<Entity>();
            List<Entity> modifiedEntities = new List<Entity>();
            List<Entity> removedEntities = new List<Entity>();

            EntityGraphChangeSet changeSet = new EntityGraphChangeSet();
            changeSet.AddedEntities = new ReadOnlyCollection<Entity>(addedEntities);
            changeSet.ModifiedEntities = new ReadOnlyCollection<Entity>(modifiedEntities);
            changeSet.RemovedEntities = new ReadOnlyCollection<Entity>(removedEntities);

            foreach (var node in EntityRelationGraph)
            {
                switch (node.EntityState)
                {
                    case EntityState.Deleted:
                        removedEntities.Add(node); break;
                    case EntityState.Modified:
                        modifiedEntities.Add(node); break;
                    case EntityState.New:
                        addedEntities.Add(node); break;
                    default: break;
                }
            }
            return changeSet;
        }
    }
}

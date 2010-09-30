using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> where TEntity : Entity
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

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
using System.Collections.Generic;
using System.Collections;
using System.ServiceModel.DomainServices.Client;
using System.Collections.ObjectModel;

namespace RIA.EntityGraph
{
    public class EntityGraphChangeSet : IEnumerable<Entity>, IEnumerable
    {
        public EntityGraphChangeSet()
        {
        }
        public ReadOnlyCollection<Entity> AddedEntities { get; internal set; }
        public bool IsEmpty
        {
            get
            {
                if( ModifiedEntities == null && RemovedEntities == null && AddedEntities == null)
                    return true;
                return 
                    ModifiedEntities.Count > 0 && RemovedEntities.Count > 0 && AddedEntities.Count > 0;
            }
        }
        public ReadOnlyCollection<Entity> ModifiedEntities { get; internal set; }
        public ReadOnlyCollection<Entity> RemovedEntities { get; internal set; }
        public IEnumerable<ChangeSetEntry> GetChangeSetEntries()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

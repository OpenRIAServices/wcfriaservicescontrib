using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel.DomainServices.Client;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EntityGraphTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100515056df58c296225211a73ce8a2631970838aeb80071df3457991c8cd8585f531be1e22f166f63d6462be361ad473ad8175d6e7173129ed4e3861b0018f499e0a6376dd52c8067c25e05fdc8b9cb9782203c7d1635587038d7e79d0a55895d75a5e181307ab8d31956ae08ca85dba415ada31605ce4bf1a8974cb33e4f74896")]
namespace RiaServicesContrib.DomainServices.Client
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
                if(ModifiedEntities != null && ModifiedEntities.Count > 0)
                {
                    return false;
                }
                if(AddedEntities != null && AddedEntities.Count > 0)
                {
                    return false;
                }
                if(RemovedEntities != null && RemovedEntities.Count > 0)
                {
                    return false;
                }
                return true;
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
            foreach(var entity in AddedEntities)
            {
                yield return entity;
            }
            foreach(var entity in ModifiedEntities)
            {
                yield return entity;
            }
            foreach(var entity in RemovedEntities)
            {
                yield return entity;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public override string ToString()
        {
            return string.Format("Added = {0}, Modified = {1}, Removed = {2}",
                AddedEntities.Count,
                ModifiedEntities.Count,
                RemovedEntities.Count);
        }
    }
}

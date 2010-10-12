using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> : IEnumerable<Entity> where TEntity : Entity
    {
        public IEnumerator<Entity> GetEnumerator() {
            return EntityRelationGraph.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}

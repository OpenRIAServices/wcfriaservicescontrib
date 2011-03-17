using System.Collections.Generic;

namespace RiaServicesContrib
{
    public partial class EntityGraph<TEntity, TValidationResult> : IEnumerable<TEntity>
    {
        public IEnumerator<TEntity> GetEnumerator() {
            return EntityRelationGraph.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return EntityRelationGraph.GetEnumerator();
        }
    }
}

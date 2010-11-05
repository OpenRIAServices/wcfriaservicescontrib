using System.Collections.Generic;

namespace EntityGraph
{
    public partial class EntityGraph<TEntity, TBase, TValidationResult> : IEnumerable<TBase>
    {
        public IEnumerator<TBase> GetEnumerator() {
            return EntityRelationGraph.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}

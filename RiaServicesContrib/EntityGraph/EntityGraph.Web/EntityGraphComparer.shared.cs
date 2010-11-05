using System;
using System.Linq;

namespace EntityGraph
{
    public partial class EntityGraph<TEntity, TBase, TValidationResult>
    {
        /// <summary>
        /// Determines whether two entity graphs are equal using the given comparer function.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public bool EntityGraphEqual<T>(EntityGraph<T, TBase, TValidationResult> graph, Func<TBase, TBase, bool> comparer) where T : class, TBase
        {
            if(this.Count() != graph.Count())
                return false;

            var zipList = this.Zip(graph, (e1, e2) => new { e1, e2 });
            return zipList.All(zipElem => comparer(zipElem.e1, zipElem.e2));
        }
        /// <summary>
        /// Determines whether two entity graphs are equal using a default equality comparison of all entities
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public bool EntityGraphEqual<T>(EntityGraph<T, TBase, TValidationResult> graph) where T : class, TBase
        {
            return EntityGraphEqual(graph, (e1, e2) => e1 == e2);
        }
    }
}

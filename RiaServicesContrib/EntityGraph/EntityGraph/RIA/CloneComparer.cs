using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using RiaServicesContrib;
using RiaServicesContrib.Extensions;
using System.Collections.Generic;

namespace EntityGraph.RIA
{
    public partial class EntityGraph<TEntity>
    {
        /// <summary>
        /// Determines whether two entity graphs are clones of eachother by member-wise comparing all entities in both graphs.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public bool IsCloneOf<T>(EntityGraph<T> graph) where T : Entity
        {
            return EntityGraphEqual(graph, (e1, e2) => e1 != e2 && MemberwiseCompare(e1, e2, true));
        }
    }
}

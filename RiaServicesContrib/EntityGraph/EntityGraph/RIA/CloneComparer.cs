using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph
    {
        /// <summary>
        /// Determines whether two entity graphs are clones of eachother by member-wise comparing all entities in both graphs.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public bool IsCloneOf(EntityGraph graph)
        {
            return EntityGraphEqual(graph, (e1, e2) => e1 != e2 && MemberwiseCompare(e1, e2, true));
        }
    }
}

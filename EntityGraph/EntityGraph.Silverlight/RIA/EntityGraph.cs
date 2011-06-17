using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph : EntityGraph<Entity>
    {
        /// <summary>
        /// Extension method that returns an entity graph object, defined by the provided entity graph shape
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="shape"></param>
        public EntityGraph(Entity Source, IEntityGraphShape shape) : base(Source, shape) { }
    }
}

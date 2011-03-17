using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph : EntityGraph<Entity, ValidationResult>
    {
        /// <summary>
        /// Extension method that returns an entity graph object that represents the entity graph this entity is part of.
        /// </summary>
        /// <param name="Source"></param>
        public EntityGraph(Entity Source) : base(Source, new EntityGraphAttributeShape()) { }
        /// <summary>
        /// Extension method that returns an entity graph object that represents the entity graph with given name this entity is part of.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Name"></param>
        public EntityGraph(Entity Source, string Name) : base(Source, new EntityGraphAttributeShape(Name)) { }
        /// <summary>
        /// Extension method that returns an entity graph object, defined by the provided graph shape
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="shape"></param>
        public EntityGraph(Entity Source, EntityGraphShape shape) : base(Source, shape) { }
    }
}

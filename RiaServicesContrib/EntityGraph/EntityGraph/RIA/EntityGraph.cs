using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;

namespace EntityGraph.RIA
{
    public partial class EntityGraph<TEntity> : EntityGraph<TEntity, Entity, ValidationResult> where TEntity : Entity
    {
        /// <summary>
        /// Extension method that returns an entity graph object that represents the entity graph this entity is part of.
        /// </summary>
        /// <param name="Source"></param>
        public EntityGraph(TEntity Source) : base(Source, new EntityGraphAttributeShape()) { }
        /// <summary>
        /// Extension method that returns an entity graph object that represents the entity graph with given name this entity is part of.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Name"></param>
        public EntityGraph(TEntity Source, string Name) : base(Source, new EntityGraphAttributeShape(Name)) { }
        /// <summary>
        /// Extension method that returns an entity graph object, defined by the provided graph shape
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="shape"></param>
        public EntityGraph(TEntity Source, EntityGraphShape shape) : base(Source, shape) { }
    }
}

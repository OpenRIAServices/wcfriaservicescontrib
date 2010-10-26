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
        public EntityGraph(TEntity Source) : base (Source) { }
        /// <summary>
        /// Extension method that returns an entity graph object that represents the entity graph with given name this entity is part of.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Name"></param>
        public EntityGraph(TEntity Source, string Name) : base(Source, Name) { }
        /// <summary>
        /// Extension method that returns an entity graph object, defined by the provided collection of paths
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Paths"></param>
        public EntityGraph(TEntity Source, string[] Paths) : base(Source, Paths) { }
    }
}

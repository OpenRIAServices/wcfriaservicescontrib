using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;

namespace EntityGraph.RIA
{
    public partial class EntityGraph<TEntity> : EntityGraph<TEntity, Entity, ValidationResult> where TEntity : Entity
    {
        public EntityGraph(TEntity Source) : base (Source) { }

        public EntityGraph(TEntity Source, string Name) : base(Source, Name) { }
    }
}

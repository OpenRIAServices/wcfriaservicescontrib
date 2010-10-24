using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;

namespace EntityGraph.RIA
{
    public abstract class GraphValidationRule<TEntity> : GraphValidationRule<TEntity, Entity, ValidationResult>
        where TEntity : Entity
    {
    }
}

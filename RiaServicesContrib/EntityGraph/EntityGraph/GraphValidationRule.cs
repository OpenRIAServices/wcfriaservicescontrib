using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using RIA.EntityValidator;


namespace RIA.EntityGraph
{
    public abstract class GraphValidationRule<TEntity> : ValidationRule<EntityGraph<TEntity>, ValidationResult> where TEntity : Entity
    {
    }
}

using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using RIA.EntityValidator;

namespace EntityGraph.EntityValidator.RIA
{
    internal class MEFValidationRulesProvider<TEntity> : MEFValidationRulesProvider<TEntity, Entity, ValidationResult>
        where TEntity : Entity
    {
    }
}

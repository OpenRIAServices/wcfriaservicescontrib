using System.ServiceModel.DomainServices.Client;

namespace EntityGraph.EntityValidator.RIA
{
    public class MEFEntityValidator<TEntity> : EntityValidator<TEntity>
        where TEntity : Entity
    {
        public MEFEntityValidator(TEntity entity)
            : base(new MEFValidationRulesProvider<TEntity>(), entity)
        {
        }
    }
}

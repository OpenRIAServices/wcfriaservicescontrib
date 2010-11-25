using System.ServiceModel.DomainServices.Client;

namespace EntityGraph.EntityValidator.RIA
{
    public class ClassEntityValidator : EntityValidator<Entity>
    {
        public ClassEntityValidator(Entity entity)
            : base(new ClassValidationRulesProvider(), entity)
        {
        }
    }
}

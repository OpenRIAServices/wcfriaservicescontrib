using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using RiaServicesContrib.DataValidation;

namespace RiaServicesContrib.DomainServices.Client.DataValidation
{
    /// <summary>
    /// WCF DomainServices.Client services-specific instantiation of the RiaServicesContrib.EntityGraph.Validation.EntityValidator class.
    /// </summary>
    public class EntityValidator : EntityValidator<Entity, ValidationResult>
    {
        /// <summary>
        /// Initializes a new instance of the EntityValidator class.
        /// </summary>
        /// <param name="entity"></param>
        public EntityValidator(Entity entity)
            : base(entity)
        {
            var rulesProvider= new ClassValidationRulesProvider<ValidationResult>(entity.GetType());
            this.Validator = new ValidationEngine { RulesProvider = rulesProvider };
        }
    }
}

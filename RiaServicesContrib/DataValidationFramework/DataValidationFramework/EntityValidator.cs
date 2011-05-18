using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using RiaServicesContrib.DataValidation;

namespace RiaServicesContrib.DomainServices.Client.DataValidation
{
    /// <summary>
    /// Class that implements signature-based validation for single entities.
    /// This is a WCF DomainServices.Client services-specific instantiation of the 
    /// RiaServicesContrib.DataValidation.EntityValidator class.
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

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using EntityGraph.Validation;

namespace EntityGraph.RIA.Validation
{
    /// <summary>
    /// WCF RIA services-specific instantiation of the EntityGraph.Validation.EntityValidator class.
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
        }
        /// <summary>
        /// Method that clears the validation result of the given entity, for the given members.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        protected override void ClearValidationResult(Entity entity, string[] membersInError, ValidationResult validationResult)
        {
            if(validationResult != ValidationResult.Success)
            {
                var validationError = entity.ValidationErrors.SingleOrDefault(ve => ve.ErrorMessage == validationResult.ErrorMessage && ve.MemberNames.SequenceEqual(membersInError));
                if(validationError != null)
                {
                    entity.ValidationErrors.Remove(validationError);
                }
            }
        }
        /// <summary>
        /// Method that sets a validation error for the given memebrs of the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        protected override void SetValidationResult(Entity entity, string[] membersInError, ValidationResult validationResult)
        {
            if(validationResult != ValidationResult.Success)
            {
                ValidationResult vResult = new ValidationResult(validationResult.ErrorMessage, membersInError);
                entity.ValidationErrors.Add(vResult);
            }
        }
        /// <summary>
        /// Method that checks if the entity has a validation error for the given set of members.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        protected override bool HasValidationResult(Entity entity, string[] membersInError, ValidationResult validationResult)
        {
            if(validationResult == ValidationResult.Success)
                return false;
            var validationError = entity.ValidationErrors.SingleOrDefault(ve => ve.ErrorMessage == validationResult.ErrorMessage && ve.MemberNames.SequenceEqual(membersInError));
            return validationError != null;
        }
    }
}

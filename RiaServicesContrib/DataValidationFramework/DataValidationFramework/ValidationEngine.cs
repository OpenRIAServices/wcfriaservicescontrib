using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
using RiaServicesContrib.DataValidation;

namespace RiaServicesContrib.DomainServices.Client.DataValidation
{
    /// <summary>
    /// Class that implements cross-entity validation.
    /// This is a WCF DomainServices.Client services-specific instantiation of the 
    /// RiaServicesContrib.DataValidation.ValidationEngine class.
    /// </summary>
    public class ValidationEngine : ValidationEngine<Entity, ValidationResult>
    {
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
        /// <summary>
        /// Method that checks if the given validation result indicates a successful validation.
        /// </summary>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        protected override bool IsValidationSuccess(ValidationResult validationResult)
        {
            return validationResult == ValidationResult.Success;
        }
    }
}

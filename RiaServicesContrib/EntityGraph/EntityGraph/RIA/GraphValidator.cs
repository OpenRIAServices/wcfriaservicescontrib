using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client;

namespace EntityGraph.RIA
{
    public partial class EntityGraph<TEntity>
    {
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
        protected override void SetValidationResult(Entity entity, string[] membersInError, ValidationResult validationResult)
        {
            if(validationResult != ValidationResult.Success)
            {
                ValidationResult vResult = new ValidationResult(validationResult.ErrorMessage, membersInError);
                entity.ValidationErrors.Add(vResult);
            }
        }
        protected override bool HasValidationResult(Entity entity, string[] membersInError, ValidationResult validationResult)
        {
            if(validationResult == ValidationResult.Success)
                return false;
            var validationError = entity.ValidationErrors.SingleOrDefault(ve => ve.ErrorMessage == validationResult.ErrorMessage && ve.MemberNames.SequenceEqual(membersInError));
            return validationError != null;
        }
    }
}
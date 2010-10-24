using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;

namespace EntityGraph.RIA
{
    public partial class EntityGraph<TEntity>
    {
        protected override void ClearValidationResult(Entity entity, ValidationResult validationResult)
        {
            if(validationResult != ValidationResult.Success)
                entity.ValidationErrors.Remove(validationResult);
        }
        protected override void SetValidationResult(Entity entity, ValidationResult validationResult)
        {
            if(validationResult != ValidationResult.Success)
                entity.ValidationErrors.Add(validationResult);
        }
        protected override bool HasValidationResult(Entity entity, ValidationResult validationResult)
        {
            if(validationResult == ValidationResult.Success)
                return false;
            return entity.ValidationErrors.Contains(validationResult);
        }
    }
}
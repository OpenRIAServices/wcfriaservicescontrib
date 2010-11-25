using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using RIA.EntityValidator;

namespace EntityGraph.EntityValidator.RIA
{
    public abstract class EntityValidator<TEntity> : EntityValidator<TEntity, Entity, ValidationResult> 
        where TEntity : Entity
    {
        public EntityValidator(IValidationRulesProvider<TEntity, Entity, ValidationResult> provider, TEntity entity) :
            base(provider, entity)
        {
        }
        protected override void ClearValidationResult(TEntity entity, ValidationResult validationResult)
        {
            if(validationResult != ValidationResult.Success)
            {
                entity.ValidationErrors.Remove(validationResult);
            }
        }
        protected override void SetValidationResult(TEntity entity, ValidationResult validationResult)
        {
            if(validationResult != ValidationResult.Success)
                entity.ValidationErrors.Add(validationResult);
        }
        protected override bool HasValidationResult(TEntity entity, ValidationResult validationResult)
        {
            if(validationResult == ValidationResult.Success)
                return false;
            return entity.ValidationErrors.Contains(validationResult);
        }
    }
}

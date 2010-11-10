using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using RIA.EntityValidator;

namespace EntityGraph.EntityValidator.RIA
{
    public class EntityValidator<TEntity> : EntityValidator<TEntity, ValidationResult> where TEntity : Entity
    {
        private static ClassValidationRulesProvider<TEntity,ValidationResult> Provider = 
            new ClassValidationRulesProvider<TEntity,ValidationResult>();

        public EntityValidator(IValidationRulesProvider<TEntity, ValidationResult> provider, TEntity entity) :
            base(provider, entity)
        {
        }
        public EntityValidator(TEntity entity) : base(Provider, entity)
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

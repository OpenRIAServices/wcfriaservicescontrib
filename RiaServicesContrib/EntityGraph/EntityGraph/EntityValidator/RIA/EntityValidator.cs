using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using RIA.EntityValidator;

namespace EntityGraph.EntityValidator.RIA
{
    public class EntityValidator
    {
        private static ClassValidationRulesProvider<Entity,ValidationResult> Provider = 
            new ClassValidationRulesProvider<Entity,ValidationResult>();
        
        private Entity Entity;

        public EntityValidator(Entity entity)
        {
            this.Entity = entity;
            var engine = new ValidationEngine<Entity, ValidationResult>(Provider, entity);
            engine.ValidationResultChanged += engine_ValidationResultChanged;
            entity.PropertyChanged += (sender, args) => engine.Validate(sender, args.PropertyName);
        }

        void engine_ValidationResultChanged(object sender, ValidationResultChangedEventArgs<ValidationResult> e)
        {
            if(e.OldResult != ValidationResult.Success)
            {
                if(Entity.ValidationErrors.Contains(e.OldResult))
                    Entity.ValidationErrors.Remove(e.OldResult);
            }
            if(e.Result != ValidationResult.Success)
            {
                Entity.ValidationErrors.Add(e.Result);
            }
        }
    }
}

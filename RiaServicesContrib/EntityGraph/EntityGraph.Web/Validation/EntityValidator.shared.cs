using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace EntityGraph.Validation
{
    /// <summary>
    /// Class that implements signature-bsaed validation for single entities.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class EntityValidator<TEntity, TResult> : IDisposable
        where TResult : class
    {
        private ValidationEngine<TEntity, TResult> Validator;

        /// <summary>
        /// Gets or sets the entity this validator operates on.
        /// </summary>
        private TEntity Entity { get; set; }
        /// <summary>
        /// Initializes a new instance of the EntityValidator class.
        /// </summary>
        /// <param name="entity"></param>
        public EntityValidator(TEntity entity)
        {
            var provider = new ClassValidationRulesProvider<TResult>(entity.GetType());
            Validator = new ValidationEngine<TEntity, TResult>(provider);
            Validator.ValidationResultChanged += Validator_ValidationResultChanged;
            Entity = entity;
            if(Entity is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)Entity).PropertyChanged += EntityValidator_PropertyChanged;
            }
            foreach(var propInfo in Entity.GetType().GetProperties())
            {
                if(typeof(INotifyCollectionChanged).IsAssignableFrom(propInfo.PropertyType))
                {
                    var collection = propInfo.GetValue(Entity, null) as INotifyCollectionChanged;
                    if(collection != null)
                    {
                        collection.CollectionChanged += EntityValidator_CollectionChanged;
                    }
                }
            }
        }
        /// <summary>
        /// Disposes allocated resources.
        /// </summary>
        public void Dispose()
        {
            Validator.ValidationResultChanged -= Validator_ValidationResultChanged;
            if(Entity is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)Entity).PropertyChanged -= EntityValidator_PropertyChanged;
            }
            foreach(var propInfo in Entity.GetType().GetProperties())
            {
                if(typeof(INotifyCollectionChanged).IsAssignableFrom(propInfo.PropertyType))
                {
                    var collection = propInfo.GetValue(Entity, null) as INotifyCollectionChanged;
                    if(collection != null)
                    {
                        collection.CollectionChanged -= EntityValidator_CollectionChanged;
                    }
                }
            }
        }
        /// <summary>
        /// Callback method that is called when a ValidationResultChanged event is received from the
        /// validation rule engine.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Validator_ValidationResultChanged(object sender, ValidationResultChangedEventArgs<TResult> e)
        {
            var rule = (ValidationRule<TResult>)sender;
            var membersInError = rule.RuleBinding.DependencyBindings.Select(b => b.ValidationRuleDependency.TargetProperty.Name).Distinct().ToArray();

            if(HasValidationResult(Entity, membersInError, e.OldValidationResult))
                ClearValidationResult(Entity, membersInError, e.OldValidationResult);
            if(HasValidationResult(Entity, membersInError, e.ValidationResult) == false)
                SetValidationResult(Entity, membersInError, e.ValidationResult);

        }
        /// <summary>
        /// Callback method that is called when a CollectionChanged event is received from the entity.
        /// We synthesize what the corresponding entity property is and then call the validate method 
        /// of the validation engine for the entity and the property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EntityValidator_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var senderType = sender.GetType();
            foreach(var propInfo in Entity.GetType().GetProperties())
            {
                if(propInfo.PropertyType.IsAssignableFrom(senderType))
                {
                    if(propInfo.GetValue(Entity, null) == sender)
                    {
                        Validator.Validate(Entity, propInfo.Name);
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// Callback method that is called when a PropertyChanged event is received from the entity.
        /// We call the Validate method of the validation engine for the object and property name of the 
        /// changed property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EntityValidator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Validator.Validate((TEntity)sender, e.PropertyName);
        }
        /// <summary>
        /// Method that checks if the entity has a validation error for the given set of members.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        protected abstract bool HasValidationResult(TEntity entity, string[] membersInError, TResult validationResult);
        /// <summary>
        /// Method that clears the validation result of the given entity, for the given members.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        protected abstract void ClearValidationResult(TEntity entity, string[] membersInError, TResult validationResult);
        /// <summary>
        /// Method that sets a validation error for the given memebrs of the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="membersInError"></param>
        /// <param name="validationResult"></param>
        protected abstract void SetValidationResult(TEntity entity, string[] membersInError, TResult validationResult);
    }
}

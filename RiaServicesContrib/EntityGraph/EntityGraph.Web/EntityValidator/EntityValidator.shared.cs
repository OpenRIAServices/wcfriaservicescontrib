using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace RIA.EntityValidator
{
    public abstract class EntityValidator<TEntity, TResult> : IDisposable where TResult : class
    {
        private ValidationEngine<TEntity, TResult> validator;
        public EntityValidator(IValidationRulesProvider<TEntity, TResult> provider, TEntity root)
        {
            validator = new ValidationEngine<TEntity, TResult>(provider, root);
            validator.ValidationResultChanged += validator_ValidationResultChanged;

            foreach(var entity in validator.ObjectsInvolved())
            {
                if(entity is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)entity).PropertyChanged += EntityValidator_PropertyChanged;
                }
                var entityType = entity.GetType();
                foreach(var propName in validator.PropertiesInvolved(entity))
                {
                    var propInfo = entityType.GetProperty(propName);
                    if(typeof(INotifyCollectionChanged).IsAssignableFrom(propInfo.PropertyType))
                    {
                        var collection = propInfo.GetValue(entity, null) as INotifyCollectionChanged;
                        if(collection != null)
                            collection.CollectionChanged += EntityValidator_CollectionChanged;
                    }
                }
            }
        }

        void EntityValidator_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var senderType = sender.GetType();
            foreach(var entity in validator.ObjectsInvolved())
            {
                foreach(var propInfo in entity.GetType().GetProperties())
                {
                    if(propInfo.PropertyType.IsAssignableFrom(senderType))
                    {
                        if(propInfo.GetValue(entity, null) == sender)
                        {
                            validator.Validate(entity, propInfo.Name);
                            return;
                        }
                    }
                }
            }
        }

        void EntityValidator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            validator.Validate(sender, e.PropertyName);
        }

        void validator_ValidationResultChanged(object sender, ValidationResultChangedEventArgs<TResult> e)
        {
            var rule = (IValidationRule<TResult>)sender;
            foreach(var entity in validator.ObjectsInvolved(rule).Cast<TEntity>())
            {
                if(HasValidationResult(entity, e.OldResult))
                    ClearValidationResult(entity, e.OldResult);
                if(HasValidationResult(entity, e.Result) == false)
                    SetValidationResult(entity, e.Result);
            }
        }

        protected abstract bool HasValidationResult(TEntity entity, TResult validationResult);
        protected abstract void ClearValidationResult(TEntity entity, TResult validationResult);
        protected abstract void SetValidationResult(TEntity entity, TResult validationResult);

        public void Dispose()
        {
            validator.ValidationResultChanged -= validator_ValidationResultChanged;
            foreach(var entity in validator.ObjectsInvolved())
            {
                if(entity is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)entity).PropertyChanged -= EntityValidator_PropertyChanged;
                }
                var entityType = entity.GetType();
                foreach(var propName in validator.PropertiesInvolved(entity))
                {
                    var propInfo = entityType.GetProperty(propName);
                    if(typeof(INotifyCollectionChanged).IsAssignableFrom(propInfo.PropertyType))
                    {
                        var collection = propInfo.GetValue(entity, null) as INotifyCollectionChanged;
                        if(collection != null)
                            collection.CollectionChanged -= EntityValidator_CollectionChanged;
                    }
                }
            }
        }
    }
}

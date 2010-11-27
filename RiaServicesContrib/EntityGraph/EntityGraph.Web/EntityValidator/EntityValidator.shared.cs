using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace RIA.EntityValidator
{
    public abstract class EntityValidator<TRoot, TEntity, TResult> : IDisposable 
        where TEntity : class 
        where TRoot : TEntity
        where TResult : class
    {
        private ValidationEngine<TRoot, TEntity, TResult> validator;

        public EntityValidator(IValidationRulesProvider<TRoot, TEntity, TResult> provider, TRoot root)
        {
            validator = new ValidationEngine<TRoot, TEntity, TResult>(provider, root);
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
            validator.Validate((TEntity)sender, e.PropertyName);
        }

        void validator_ValidationResultChanged(object sender, ValidationResultChangedEventArgs<TResult> e)
        {
            var rule = (IValidationRule<TRoot,TResult>)sender;
            foreach(var entity in validator.ObjectsInvolved(rule).Cast<TRoot>())
            {
                if(HasValidationResult(entity, e.OldResult))
                    ClearValidationResult(entity, e.OldResult);
                if(HasValidationResult(entity, e.Result) == false)
                    SetValidationResult(entity, e.Result);
            }
        }

        protected abstract bool HasValidationResult(TRoot entity, TResult validationResult);
        protected abstract void ClearValidationResult(TRoot entity, TResult validationResult);
        protected abstract void SetValidationResult(TRoot entity, TResult validationResult);

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

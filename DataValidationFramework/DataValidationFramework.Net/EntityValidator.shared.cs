using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace RiaServicesContrib.DataValidation
{
    /// <summary>
    /// Class that implements signature-based validation for single entities.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class EntityValidator<TEntity, TResult> : IDisposable
        where TEntity : class
        where TResult : class
    {
        private IValidationEngine<TEntity> _validator;
        protected IValidationEngine<TEntity> Validator
        {
            get
            {
                return _validator;
            }
            set
            {
                if(_validator != value)
                {
                    if(_validator != null)
                    {
                        ClearValidatorEventHandlers();
                    }
                    _validator = value;
                    if(_validator != null)
                    {
                        SetValidatorEventHandlers();
                    }
//                    _validator.Validate(Entity);
                }
            }
        }
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
            Entity = entity;
        }
        private void SetValidatorEventHandlers()
        {
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
        private void ClearValidatorEventHandlers()
        {
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
        /// Disposes allocated resources.
        /// </summary>
        public void Dispose()
        {
            if(Validator != null)
            {
                Validator.Dispose();
                Validator = null;
            }
        }
        /// <summary>
        /// Callback method that is called when a CollectionChanged event is received from the entity.
        /// We synthesize what the corresponding entity property is and then call the validate method 
        /// of the validation engine for the entity and the property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntityValidator_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
        private void EntityValidator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Validator.Validate((TEntity)sender, e.PropertyName);
        }
    }
}

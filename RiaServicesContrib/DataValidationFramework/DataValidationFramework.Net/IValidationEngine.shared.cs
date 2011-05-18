using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;

namespace RiaServicesContrib.DataValidation
{
    public interface IValidationEngine<TEntity> : IDisposable
        where TEntity : class
    {
        /// <summary>
        /// Occurs when the collection of validation rules changes. 
        /// </summary>
        event NotifyCollectionChangedEventHandler ValidationRuleSetChanged;
        /// <summary>
        /// Occurs when the collection of entities in error changes. 
        /// </summary>
        event NotifyCollectionChangedEventHandler EntitiesInErrorChanged;
        /// <summary>
        /// Returns the collection of registered validation rules.
        /// </summary>
        IEnumerable ValidationRules { get; }
        /// <summary>
        /// Returns the collection of entities for which a validation rule has failed.
        /// </summary>
        IEnumerable<TEntity> EntitiesInError { get; }
        /// <summary>
        /// Method that invokes all matching validation rules for the given object
        /// </summary>
        /// <param name="obj"></param>
        void Validate(TEntity obj);
        /// <summary>
        /// Method that invokes all matching validation rules for the given object and 
        /// property name.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        void Validate(TEntity obj, string propertyName);
        /// <summary>
        /// Method that invokes all matching validation rules for all possible bindings given
        /// a collection of objects, an object 'obj' that should be present in any bindings, and a 
        /// (changed) property with name 'propertyName' that should be part in any signature.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="objects"></param>
        void Validate(object obj, string propertyName, IEnumerable<TEntity> objects);
        /// <summary>
        /// Method that invokes all matching validation rules for all possible bindings given a 
        /// collection of entities.
        /// </summary>
        /// <param name="objects"></param>
        void Validate(IEnumerable<TEntity> objects);
    }
}
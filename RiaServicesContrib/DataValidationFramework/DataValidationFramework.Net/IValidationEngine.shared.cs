using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace RiaServicesContrib.DataValidation
{
    public interface IValidationEngine<TEntity> : INotifyCollectionChanged, IDisposable
        where TEntity : class
    {
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
        /// <summary>
        /// Gets the number of registered validation rules.
        /// </summary>
        int Count { get; }
    }
}
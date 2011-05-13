using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace RiaServicesContrib.DataValidation
{
    public interface IValidationEngine<TEntity> : INotifyCollectionChanged, IDisposable
        where TEntity : class
    {
        void Validate(TEntity obj);
        void Validate(TEntity obj, string propertyName);
        void Validate(object obj, string propertyName, IEnumerable<TEntity> objects);
        void Validate(IEnumerable<TEntity> objects);
        /// <summary>
        /// gets the number of registered validation rules.
        /// </summary>
        int Count { get; }
    }
}
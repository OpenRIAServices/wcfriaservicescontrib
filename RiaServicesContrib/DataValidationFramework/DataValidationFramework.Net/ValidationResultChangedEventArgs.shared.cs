using System;
using System.Collections.Generic;

namespace RiaServicesContrib.DataValidation
{
    /// <summary>
    /// Class that is used to notify that the result of a validation rule has changed. 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
	public class ValidationResultChangedEventArgs<TEntity, TResult> : EventArgs
    {
        /// <summary>
        /// gets the new result of the validation rule.
        /// </summary>
        public TResult ValidationResult { get; private set; }
        /// <summary>
        /// Gets all the entities that are involved by the validation rule represented
        /// by this instance of ValidationResultChangedEventArgs.
        /// </summary>
        public IEnumerable<TEntity> Entities { get; private set; }
        /// <summary>
        /// Initializes a new instance of the ValidationResultChangedEventArgs class
        /// </summary>
        /// <param name="ValidationResult"></param>
        internal ValidationResultChangedEventArgs(TResult ValidationResult, IEnumerable<TEntity> entities)
        {
            this.ValidationResult = ValidationResult;
            this.Entities = entities;
        }
	}
}

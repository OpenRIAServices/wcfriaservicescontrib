using System;

namespace RiaServicesContrib.DataValidation
{
    /// <summary>
    /// Class that is used to notify that the result of a validation rule has changed. 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
	public class ValidationResultChangedEventArgs<TResult> : EventArgs
    {
        /// <summary>
        /// gets the new result of the validation rule.
        /// </summary>
        public TResult ValidationResult { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ValidationResultChangedEventArgs class
        /// </summary>
        /// <param name="ValidationResult"></param>
        internal ValidationResultChangedEventArgs(TResult ValidationResult)
        {
            this.ValidationResult = ValidationResult;
        }
	}
}

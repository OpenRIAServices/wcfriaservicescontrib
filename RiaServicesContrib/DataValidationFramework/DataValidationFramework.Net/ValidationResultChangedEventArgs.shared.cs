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
        /// Gets the last result of the validation rule.
        /// </summary>
        public TResult OldValidationResult { get; private set; }
        /// <summary>
        /// gets the new result of the validation rule.
        /// </summary>
        public TResult ValidationResult { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ValidationResultChangedEventArgs class with given old and new 
        /// validation results.
        /// </summary>
        /// <param name="OldValidationResult"></param>
        /// <param name="ValidationResult"></param>
        internal ValidationResultChangedEventArgs(TResult OldValidationResult, TResult ValidationResult)
        {
            this.OldValidationResult = OldValidationResult;
            this.ValidationResult = ValidationResult;
        }
	}
}

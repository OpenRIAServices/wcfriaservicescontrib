﻿namespace RiaServicesContrib.DataValidation
{
    /// <summary>
    /// Abstract base class for attribute validators.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public abstract class AttributeValidator<TResult> : ValidationRule<TResult>, IAttributeValidator<TResult>
        where TResult : class
    {
        /// <summary>
        /// Initializes a new instance of the AttributeValidator class.
        /// </summary>
        /// <param name="signature"></param>
        protected AttributeValidator(params ValidationRuleDependency[] signature) : base(signature) { }
        /// <summary>
        /// Invokes this validator on the given object
        /// </summary>
        /// <param name="value"></param>
        public abstract TResult Validate(object value);
    }
}
